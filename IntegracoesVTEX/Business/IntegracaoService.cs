using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IntegracoesVETX.DAL;
using IntegracoesVETX.Entity;
using IntegracoesVETX.Util;
using IntegracoesVTEX.Util;
using Newtonsoft.Json;
using SAPbobsCOM;

namespace IntegracoesVETX.Business
{
    public class IntegracaoService
    {
        private Log log;

        public IntegracaoService()
        {
            log = new Log();
        }

        public void IniciarIntegracaoEstoque(Company oCompany)
        {
            try
            {
                Repositorio repositorio = new Repositorio();
                log.WriteLogEstoque("Inicio do Processo de Integração de Estoque");
                WarehouseDAL warehouseDAL = new WarehouseDAL();
                Recordset recordset = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset = warehouseDAL.RecuperarSaldoEstoqueSAP(oCompany);
                if (recordset != null && recordset.RecordCount > 0)
                {
                    for (int i = 0; i < recordset.RecordCount; i++)
                    {
                        try
                        {
                            string _itemCode = recordset.Fields.Item("ItemCode").Value.ToString();
                            short _onHand = Convert.ToInt16(recordset.Fields.Item("OnHand").Value.ToString());
                            string warehouseId = ConfigurationManager.AppSettings["warehouseId"];
                            if (_itemCode.Equals("003179-055"))
                            {
                                string empty = string.Empty;
                            }
                            Task<HttpResponseMessage> response = repositorio.BuscarItemPorSKU(_itemCode, _onHand, oCompany);
                            if (response.Result.IsSuccessStatusCode)
                            {
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, _itemCode, _itemCode, EnumStatusIntegracao.Sucesso, "Estoque atualizado com sucesso.");
                                log.WriteLogEstoque("Quantidade de estoque do Produto " + _itemCode + " para o depósito " + warehouseId + " atualizada com sucesso.");
                            }
                            if (Convert.ToInt16(response.Result.StatusCode) == 400)
                            {
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, _itemCode, _itemCode, EnumStatusIntegracao.Erro, response.Result.ReasonPhrase);
                                log.WriteLogEstoque("Não foi possível atualizar a quantidade de estoque para o produto " + _itemCode + ". Retorno API Vtex: " + response.Result.ReasonPhrase);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        recordset.MoveNext();
                    }
                }
                if (recordset != null)
                {
                    Marshal.ReleaseComObject(recordset);
                }
            }
            catch (Exception e)
            {
                log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, "", "", EnumStatusIntegracao.Erro, e.Message);
                log.WriteLogEstoque("Exception IniciarProcessoEstoque " + e.Message);
                throw;
            }
        }

        public void ProcessarNovosClientes(List<Cliente> clientes, List<Endereco> enderecos)
        {
            try
            {
                Log.WriteLogCliente("Processando Novos Clientes.");
                if (clientes.Count <= 0)
                {
                    return;
                }
                Company oCompany = CommonConn.InitializeCompany();
                if (!oCompany.Connected)
                {
                    return;
                }
                string document = string.Empty;
                foreach (Cliente cliente in clientes)
                {
                    foreach (Endereco endereco in enderecos)
                    {
                        if (cliente.document != null || cliente.corporateDocument != null)
                        {
                            document = ((!cliente.isCorporate.Equals("true")) ? cliente.document : cliente.corporateDocument);
                        }
                        if (document != null)
                        {
                            if (cliente.id.Equals(endereco.userId))
                            {
                                InserirClientes(oCompany, cliente, endereco, null);
                            }
                            document = null;
                            continue;
                        }
                        log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, cliente.id, "", EnumStatusIntegracao.Erro, "Cliente não cadastrado pois o número do documento VTEX é inválido.");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLogCliente("ProcessarNovosClientes Exception:" + e.Message);
                throw;
            }
        }

        private void InserirClientes(Company company, Cliente cliente, Endereco endereco, Pedido pedido)
        {
            try
            {
                string errorMessage;
                new BusinessPartnersDAL().InserirBusinessPartner(company, cliente, endereco, pedido, out errorMessage);
            }
            catch (Exception e)
            {
                Log.WriteLogCliente("Exception inserirClientes " + e.Message);
                throw;
            }
        }

        public void IniciarImportacaoPedidos(Company oCompany)
        {
            string idPedidoVtex;
            try
            {
                log.WriteLogPedido("Inicio do Processo de Importação de Pedido.");
                Repositorio repositorioPedido = new Repositorio();
                List<Feed> listaEnveto = new List<Feed>();
                Pedido pedidoVtex = new Pedido();

                Task<HttpResponseMessage> responsePedido = repositorioPedido.ConsultarFilaDeEventos();
                if (responsePedido.Result.IsSuccessStatusCode)
                {
                    listaEnveto = JsonConvert.DeserializeObject<List<Feed>>(responsePedido.Result.Content.ReadAsStringAsync().Result);
                    if (listaEnveto.Count > 0)
                    {
                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                        foreach (Feed evento in listaEnveto)
                        {
                            if (!evento.state.Equals("ready-for-handling"))
                            {
                                continue;
                            }

                            string sql = string.Format(DAL.SQL.Queries.VTEX_InserirPedido, evento.orderId, evento.handle);
                            oRS.DoQuery(sql);
                        }

                        oRS = null;
                    }
                }
                else
                {
                    log.WriteLogPedido("Não foi possível consultar Fila de Enventos Vtex. " + responsePedido.Result.ReasonPhrase);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception método IniciarIntegracaoPedido - " + e.Message);
                throw;
            }
        }

        public void IniciarIntegracaoPedido(Company oCompany)
        {
            string idPedidoVtex;
            try
            {
                log.WriteLogPedido("Inicio do Processo de Integração de Pedido.");
                Repositorio repositorioPedido = new Repositorio();
                //List<Feed> listaEnveto = new List<Feed>();
                Pedido pedidoVtex = new Pedido();

                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRS.DoQuery(DAL.SQL.Queries.VTEX_PedidosIntegrar);

                if (oRS.RecordCount > 0)
                {

                    for (int i = 0; i < oRS.RecordCount; i++)
                    {
                        string numeroPedido = oRS.Fields.Item("U_NUM_VTEX").Value.ToString();
                        string evento = oRS.Fields.Item("U_EVENTO").Value.ToString();

                        Task<HttpResponseMessage> responseOrder = repositorioPedido.BuscarPedido(numeroPedido);
                        if (!responseOrder.Result.IsSuccessStatusCode)
                        {
                            continue;
                        }
                        pedidoVtex = JsonConvert.DeserializeObject<Pedido>(responseOrder.Result.Content.ReadAsStringAsync().Result);
                        if (!pedidoVtex.storePreferencesData.countryCode.Equals("BRA"))
                        {
                            continue;
                        }
                        if (pedidoVtex.origin.Equals("Fulfillment"))
                        {
                            Cliente clienteMkt = new Cliente();
                            Endereco enderecoMkt = new Endereco();
                            InserirClientes(oCompany, clienteMkt, enderecoMkt, pedidoVtex);
                        }
                        else
                        {
                            new List<Cliente>();
                            List<Endereco> enderecols = new List<Endereco>();
                            if (!string.IsNullOrEmpty(pedidoVtex.clientProfileData.document))
                            {
                                Task<HttpResponseMessage> responseCliente = repositorioPedido.BuscarClientePorDocumento(pedidoVtex.clientProfileData.document);
                                if (responseCliente.Result.IsSuccessStatusCode)
                                {
                                    foreach (Cliente cliente in JsonConvert.DeserializeObject<List<Cliente>>(responseCliente.Result.Content.ReadAsStringAsync().Result))
                                    {
                                        Task<HttpResponseMessage> responseEndereco = repositorioPedido.BuscarEnderecoPorUserId(cliente.id);
                                        if (responseEndereco.Result.IsSuccessStatusCode)
                                        {
                                            enderecols = JsonConvert.DeserializeObject<List<Endereco>>(responseEndereco.Result.Content.ReadAsStringAsync().Result);
                                        }
                                        foreach (Endereco endereco in enderecols)
                                        {
                                            if (oCompany.Connected)
                                            {
                                                string document = string.Empty;
                                                if (cliente.document != null || cliente.corporateDocument != null)
                                                {
                                                    document = ((!cliente.isCorporate.Equals("true")) ? cliente.document : cliente.corporateDocument);
                                                }
                                                if (document != null)
                                                {
                                                    InserirClientes(oCompany, cliente, endereco, pedidoVtex);
                                                }
                                                else
                                                {
                                                    log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, cliente.id, "", EnumStatusIntegracao.Erro, "Cliente não cadastrado pois o número do documento VTEX é inválido.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        InserirPedidoVenda(oCompany, pedidoVtex, evento);
                    }

                }
                else
                {
                    log.WriteLogPedido("Nenhum pedido importado para integrar ");
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception método IniciarIntegracaoPedido - " + e.Message);
                throw;
            }
        }



        private int InserirPedidoVenda(Company oCompany, Pedido pedidoVtex, string handle)
        {
            try
            {
                if (oCompany.Connected)
                {
                    OrdersDAL order = new OrdersDAL(oCompany);
                    string messageError = "";
                    bool inserir = true;
                    ItemVtex[] items = pedidoVtex.items;
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].refId == null && inserir)
                        {
                            log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedidoVtex.orderId, "", EnumStatusIntegracao.Erro, "Um ou mais item(s) do pedido está com o código de referência inválido.");
                            inserir = false;
                        }
                    }
                    if (inserir && order.InsertOrder(pedidoVtex, out messageError) == 0)
                    {
                        Repositorio repositorio = new Repositorio();
                        Task<HttpResponseMessage> response = repositorio.AtualizaFilaEnvetoPedido(handle);
                        if (response.Result.IsSuccessStatusCode)
                        {
                            log.WriteLogPedido("Pedido " + pedidoVtex.orderId + " removido da fila de eventos (Feed).");
                        }
                        else
                        {
                            log.WriteLogPedido("Não foi possível remover o pedido " + pedidoVtex.orderId + " da fila de eventos (Feed)." + response.Result.ReasonPhrase);
                        }
                        if (!repositorio.InciarManuseio(pedidoVtex.orderId).Result.IsSuccessStatusCode)
                        {
                            log.WriteLogPedido("Não foi possível alterar status do pedido " + pedidoVtex.orderId + " para Iniciar Manuseio." + response.Result.ReasonPhrase);
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception InserirPedidoVenda " + e.Message);
                throw;
            }
        }

        public void RetornoNotaFiscal(Company oCompany)
        {
            try
            {
                if (!oCompany.Connected)
                {
                    return;
                }
                OrdersDAL orders = new OrdersDAL(oCompany);
                Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordSet = orders.RecuperarNumeroNF();
                Recordset tempRecordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                tempRecordSet = orders.RecuperarNumeroNF();
                if (recordSet != null && recordSet.RecordCount > 0)
                {
                    while (!recordSet.EoF)
                    {
                        if (recordSet.EoF)
                        {
                            continue;
                        }
                        Repositorio repositorio = new Repositorio();
                        Invoice invoice = new Invoice();
                        invoice.type = "Output";
                        invoice.issuanceDate = recordSet.Fields.Item("invoiceDate").Value.ToString();
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        invoice.issuanceDate = DateTime.ParseExact(invoice.issuanceDate, "dd/MM/yyyy 00:00:00", provider).ToString("yyyy-MM-dd");
                        invoice.invoiceNumber = recordSet.Fields.Item("invoiceNumber").Value.ToString();
                        invoice.invoiceKey = recordSet.Fields.Item("nfeKey").Value.ToString();
                        string externalId = string.Empty;
                        string idOrderVtex = string.Empty;
                        string idOrderVtex2 = string.Empty;
                        string docSAP = string.Empty;
                        string docNPV = string.Empty;
                        externalId = recordSet.Fields.Item("externalId").Value.ToString();
                        idOrderVtex = recordSet.Fields.Item("idOrderVtex").Value.ToString();
                        idOrderVtex2 = recordSet.Fields.Item("idOrderVtex2").Value.ToString();
                        docSAP = recordSet.Fields.Item("docSAP").Value.ToString();
                        docNPV = recordSet.Fields.Item("docNPV").Value.ToString();

                        double totalNF = (double)recordSet.Fields.Item("totalNF").Value;
                        invoice.invoiceValue = Convert.ToInt32(totalNF * 100).ToString();

                        invoice.courier = recordSet.Fields.Item("shippingMethod").Value.ToString();
                        string idPedidoVTEX = string.Empty;
                        string tempDocNPV = string.Empty;
                        List<ItemNF> listaItem = new List<ItemNF>();
                        for (int i = 0; i < tempRecordSet.RecordCount; i++)
                        {
                            if (!tempRecordSet.EoF)
                            {
                                tempDocNPV = tempRecordSet.Fields.Item("docNPV").Value.ToString();
                                if (docNPV.Equals(tempDocNPV))
                                {
                                    ItemNF item = new ItemNF();
                                    item.id = tempRecordSet.Fields.Item("codItem").Value.ToString();
                                    double price = (double)tempRecordSet.Fields.Item("precoItem").Value;
                                    item.price = Convert.ToInt32(price * 100);
                                    item.quantity = Convert.ToInt32(tempRecordSet.Fields.Item("qtdItem").Value.ToString());
                                    listaItem.Add(item);
                                }
                                if (i >= 10)
                                {
                                    break;
                                }
                                tempRecordSet.MoveNext();
                            }
                        }
                        invoice.items = listaItem;
                        if (!string.IsNullOrEmpty(idOrderVtex))
                        {
                            idPedidoVTEX = idOrderVtex;
                        }
                        else if (!string.IsNullOrEmpty(idOrderVtex2))
                        {
                            idPedidoVTEX = idOrderVtex2;
                        }
                        if (!string.IsNullOrEmpty(idOrderVtex) && !string.IsNullOrEmpty(idOrderVtex2))
                        {
                            Task<HttpResponseMessage> responseOrder = repositorio.BuscarPedido(idOrderVtex);
                            if (responseOrder.Result.IsSuccessStatusCode)
                            {
                                if (!JsonConvert.DeserializeObject<Pedido>(responseOrder.Result.Content.ReadAsStringAsync().Result).status.Equals("canceled"))
                                {
                                    Task<HttpResponseMessage> response = repositorio.RetornoNotaFiscal(invoice, idPedidoVTEX);
                                    if (response.Result.IsSuccessStatusCode)
                                    {
                                        log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Sucesso, "Número NF " + invoice.invoiceNumber + " enviado para a Vtex com sucesso.");
                                        log.WriteLogPedido("Número NF para o Pedido de Venda " + docSAP + " enviado para a Vtex com sucesso.");
                                        if (orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId)) != 0)
                                        {
                                            log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Erro, "Número NF " + invoice.invoiceNumber + " retornado porém não foi possivél atualizar campo de usuário (U_EnvioNFVTEX) do Pedido de Venda");
                                            log.WriteLogPedido("Falha ao atualizar Pedido de Venda " + docSAP);
                                        }
                                    }
                                    else
                                    {
                                        ErrorResponseNF errorResponse = JsonConvert.DeserializeObject<ErrorResponseNF>(response.Result.Content.ReadAsStringAsync().Result);
                                        if (errorResponse != null)
                                        {
                                            log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Erro, errorResponse.error.message);
                                            log.WriteLogPedido("Falha ao retornar número da Nota Fiscal " + docSAP + " para a Vtex");
                                        }
                                    }
                                }
                                else
                                {
                                    log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Erro, "Pedido com status de \"cancelado\" na VTEX.");
                                    log.WriteLogPedido("Pedido com status de \"cancelado\" na VTEX.");
                                    orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId));
                                }
                            }
                        }
                        else
                        {
                            log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Erro, "Id do Pedido VTEX (NumAtCard e U_NumPedEXT) do Pedido de Venda " + docNPV + " em branco.");
                            log.WriteLogPedido("Falha ao retornar número da Nota Fiscal " + docSAP + " para a Vtex - Id do Pedido VTEX (NumAtCard) do Pedido de Venda " + docNPV + " em branco.");
                            if (orders.AtualizarPedidoVenda(oCompany, Convert.ToInt32(externalId)) != 0)
                            {
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idPedidoVTEX, docSAP, EnumStatusIntegracao.Erro, "Número NF " + invoice.invoiceNumber + " retornado porém não foi possivél atualizar campo de usuário (U_EnvioNFVTEX) do Pedido de Venda");
                                log.WriteLogPedido("Falha ao atualizar Pedido de Venda " + docSAP);
                            }
                        }
                        recordSet.MoveNext();
                    }
                }
                if (recordSet != null)
                {
                    Marshal.ReleaseComObject(recordSet);
                }
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception RetornoNotaFiscal " + e.Message);
            }
        }

        public void IniciarIntegracaoCancelamentoPedido(Company oCompany)
        {
            try
            {
                Repositorio repositorioCancelPedido = new Repositorio();
                OrderFiltered orders = new OrderFiltered();
                Task<HttpResponseMessage> responseOrderFiltered = repositorioCancelPedido.PedidosACancelar();
                if (responseOrderFiltered.Result.IsSuccessStatusCode)
                {
                    orders = JsonConvert.DeserializeObject<OrderFiltered>(responseOrderFiltered.Result.Content.ReadAsStringAsync().Result);
                    if (orders.list.Length == 0)
                    {
                        return;
                    }
                    List[] list = orders.list;
                    foreach (List item in list)
                    {
                        if (item.currencyCode.Equals("BRL") && item.status.Equals("payment-pending") && (DateTime.Now - item.creationDate).Days > Convert.ToInt32(ConfigurationManager.AppSettings["qtdDiasCancelemtno"]))
                        {
                            Task<HttpResponseMessage> responseCacelPedido = repositorioCancelPedido.CancelarPedido(item.orderId);
                            if (responseCacelPedido.Result.IsSuccessStatusCode)
                            {
                                log.WriteLogPedido("Pedido " + item.orderId + " cancelado com sucesso." + responseCacelPedido.Result.ReasonPhrase);
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.Cancel, item.orderId, "", EnumStatusIntegracao.Sucesso, "Pedido " + item.orderId + " cancelado com sucesso.");
                            }
                            else
                            {
                                log.WriteLogPedido("Não foi possível cancelar pedido " + item.orderId + "." + responseCacelPedido.Result.ReasonPhrase);
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.Cancel, item.orderId, "", EnumStatusIntegracao.Erro, "Não foi possível cancelar pedido " + item.orderId + "." + responseCacelPedido.Result.ReasonPhrase);
                            }
                        }
                    }
                }
                else
                {
                    log.WriteLogPedido("Nenhum Pedido pendente a ser cancelado.");
                }
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception IntegracaoCancelamentoPedido " + e.Message);
                throw;
            }
        }

        public void IniciarIntegracaoRetornoRastreamento(Company oCompany)
        {
            try
            {
                if (!oCompany.Connected)
                {
                    return;
                }
                OrdersDAL orders = new OrdersDAL(oCompany);
                Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordSet = orders.RecuperarCodigoRastreamento();
                if (recordSet != null && recordSet.RecordCount > 0)
                {
                    while (!recordSet.EoF)
                    {
                        if (recordSet.EoF)
                        {
                            continue;
                        }
                        string idOrderVTEX = string.Empty;
                        if (!string.IsNullOrEmpty(recordSet.Fields.Item("idOrderVtex").Value.ToString()))
                        {
                            idOrderVTEX = recordSet.Fields.Item("idOrderVtex").Value.ToString();
                        }
                        else if (!string.IsNullOrEmpty(recordSet.Fields.Item("idOrderVtex2").ToString()))
                        {
                            idOrderVTEX = recordSet.Fields.Item("idOrderVtex2").Value.ToString();
                        }
                        string invoiceNumber = recordSet.Fields.Item("invoiceNumber").Value.ToString();
                        var invoiceBody = new
                        {
                            trackingNumber = recordSet.Fields.Item("codigoRastreamento").Value.ToString(),
                            trackingUrl = "",
                            courier = ""
                        };
                        string docEntry = recordSet.Fields.Item("docEntry").Value.ToString();
                        string docNum = recordSet.Fields.Item("docNum").Value.ToString();
                        Repositorio repositorio = new Repositorio();
                        Task<HttpResponseMessage> responseOrderVTEX = repositorio.BuscarPedido(idOrderVTEX);
                        if (responseOrderVTEX.Result.IsSuccessStatusCode && JsonConvert.DeserializeObject<Pedido>(responseOrderVTEX.Result.Content.ReadAsStringAsync().Result).status.Equals("invoiced"))
                        {
                            Task<HttpResponseMessage> responseCodigoRastreio = repositorio.RetornoCodigoRastreio(invoiceBody, idOrderVTEX, invoiceNumber);
                            if (responseCodigoRastreio.Result.IsSuccessStatusCode)
                            {
                                string nomeDestinatario = recordSet.Fields.Item("nomeDestinatario").Value.ToString();
                                string emailDestinatario = recordSet.Fields.Item("emailDestinatario").Value.ToString();
                                log.WriteLogTable(oCompany, EnumTipoIntegracao.CodRastreamento, idOrderVTEX, docNum, EnumStatusIntegracao.Sucesso, "Cód. Rastreamento " + recordSet.Fields.Item("codigoRastreamento").Value.ToString() + " da NF " + invoiceNumber + " enviado para VTEX com sucesso.");
                                if (EnviarEmailCodRastreamento(nomeDestinatario, emailDestinatario, recordSet.Fields.Item("codigoRastreamento").Value.ToString(), invoiceNumber))
                                {
                                    log.WriteLogPedido("E-mail de Cód. Rastreamento " + recordSet.Fields.Item("codigoRastreamento").Value.ToString() + " da NF " + invoiceNumber + " enviado com sucesso.");
                                }
                                else
                                {
                                    log.WriteLogPedido("Falha ao enviar e-mail com Cód. Rastreio " + recordSet.Fields.Item("codigoRastreamento").Value.ToString() + " da NF " + invoiceNumber + " para o destinaário " + emailDestinatario);
                                }
                                if (orders.AtualizarPedidoVendaIntRastreamento(oCompany, Convert.ToInt32(docEntry)) != 0)
                                {
                                    log.WriteLogTable(oCompany, EnumTipoIntegracao.NF, idOrderVTEX, docNum, EnumStatusIntegracao.Erro, "Código de rastreamento do Pedido " + docNum + " retornado porém não foi possível atualizar o campo de usuário U_ValidaEnvioCodRastreio. " + oCompany.GetLastErrorDescription());
                                    log.WriteLogPedido("Falha ao atualizar Pedido de Venda " + recordSet.Fields.Item("cardCode").Value.ToString());
                                }
                            }
                            else if (responseCodigoRastreio.Result.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                log.WriteLogPedido("Sem autorização ao retornar Cód.Rastreio p/ idOrderVTEX: " + idOrderVTEX);
                            }
                            else if (responseCodigoRastreio.Result.StatusCode == HttpStatusCode.BadRequest)
                            {
                                log.WriteLogPedido("Requisicao mal formatada ao retornar Cód.Rastreio p/ idOrderVTEX: " + idOrderVTEX);
                            }
                        }
                        recordSet.MoveNext();
                    }
                }
                else
                {
                    log.WriteLogPedido("Nenhum código de rastreio a ser enviado.");
                }
                if (recordSet != null)
                {
                    Marshal.ReleaseComObject(recordSet);
                }
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Exception EnvioCod.Rastreamento " + e.Message);
            }
        }

        public bool EnviarEmailCodRastreamento(string nomeDestinatario, string emailDestinatario, string codigoRastreamento, string notaFiscal)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = ConfigurationManager.AppSettings["host"];
                smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["credencialMail"], ConfigurationManager.AppSettings["credencialSenha"]);
                MailMessage mailMessage = new MailMessage();
                mailMessage.Sender = new MailAddress(ConfigurationManager.AppSettings["credencialMail"], "WAHL CLIPPER");
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["credencialMail"], "WAHL CLIPPER");
                mailMessage.To.Add(new MailAddress(emailDestinatario ?? ""));
                mailMessage.Subject = "WAHL CLIPPER | Código de Rastreio " + codigoRastreamento + " da NF N° " + notaFiscal;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;
                mailMessage.Body = "Prezado(a) " + nomeDestinatario + ".<br/><br/> Segue o código de rastreio " + codigoRastreamento + ", referente a sua Nota Fiscal N.o " + notaFiscal + ".<br/><br/><i><b>Equipe Wahl Clipper</b></i><br/><br/><h5>Este e-mail é gerado automático não responda, por favor.</h5> ";
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                log.WriteLogPedido("Excpetion EnviarEmailCodRastreamento " + e.Message);
                return false;
            }
        }
    }
}
