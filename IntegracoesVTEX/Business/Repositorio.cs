using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IntegracoesVETX.Entity;
using IntegracoesVETX.Service;
using IntegracoesVETX.Util;
using Newtonsoft.Json;
using SAPbobsCOM;

namespace IntegracoesVETX.Business
{
    internal class Repositorio : BaseService
    {
        private Item item;

        public async Task<HttpResponseMessage> BuscarClientePorDocumento(string document)
        {
            //Log.WriteLogCliente("Método BuscarNovosClientes");
            Log.WriteLogCliente("Buscando Novos Clientes VTEX");
            try
            {
                string _fiedParam = "_fields=_all";

                string _whereParam = "&_where=document='#DOCUMENT'".Replace("#DOCUMENT", document);

                string uriClientes = "api/dataentities/CL/search?" + _fiedParam + _whereParam;

                Log.WriteLogCliente("URI: " + uriClientes);

                HttpResponseMessage responseCliente = await BuildClient().GetAsync(uriClientes);

                //Log.WriteLogCliente("Feita a busca de clientes - status:" + responseClientes.StatusCode);

                return responseCliente;

            }
            catch (HttpRequestException h)
            {
                Log.WriteLogCliente("Exception BuscarNovosClientes : " + h.InnerException.Message);
                throw h;
            }

        }

        public async Task<HttpResponseMessage> BuscarEnderecoPorUserId(string userId)
        {
            try
            {
                //Log.WriteLog("Buscando Novos Endereços VTEX");
                ///Log.WriteLogCliente("Método BuscarEnderecos");

                string _fiedParam = "_fields=_all";

                string _whereParam = "&_where=userId='#USERID'".Replace("#USERID", userId);

                string uri = "api/dataentities/AD/search?" + _fiedParam + _whereParam;

                Log.WriteLogCliente("URI: " + uri);

                HttpResponseMessage responseEndereco = await BuildClient().GetAsync(uri);

                //Log.WriteLogCliente("Feita a busca de endereços - result: "+ responseEndereco.StatusCode);

                return responseEndereco;
            }
            catch (HttpRequestException e)
            {
                Log.WriteLogCliente("Exception BuscarEnderecos : " + e.InnerException.Message);
                throw e;
            }
        }

        //Método responsável por buscar item por SKU
        public async Task<HttpResponseMessage> BuscarItemPorSKU(string _itemCode, int _onHand, SAPbobsCOM.Company oCompany)
        {
            Log log = new Log();
            try
            {
                //log.WriteLogEstoque("Buscando Item VTEX por ManufacturerCode - Código Item SAP: "+_itemCode);

                string uri = "api/catalog_system/pvt/sku/stockkeepingunitbyalternateId/" + _itemCode;

                //Log.WriteLog("URI: " + uri);

                HttpResponseMessage response = await BuildClient().GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync();

                    this.item = JsonConvert.DeserializeObject<Item>(jsonResponse.Result);

                    if (this.item != null)
                    {
                        //Log.WriteLog("Item " + item.ManufacturerCode + " localizado.");

                        int _skuId = this.item.Id;

                        if (this.item.IsActive)
                        {
                            string warehouseId = ConfigurationManager.AppSettings["warehouseId"];

                            Task<HttpResponseMessage> responseAtualizacaoEstoque = this.AtualizarQuantidadeEstoque(_skuId, _onHand);

                            return await responseAtualizacaoEstoque;
                        }
                    }
                }
                else
                {
                    //Log.WriteLogTable(oCompany, EnumTipoIntegracao.Estoque, _itemCode, "", EnumStatusIntegracao.Erro, "Item não localizado na VTEX");
                    //this.log.WriteLogEstoque("Item "+_itemCode+" não localizado na Vtex. ");
                    return response;
                }

            }
            catch (HttpRequestException e)
            {
                log.WriteLogEstoque("Exception BuscarItemVtexPorSKU " + e.InnerException.Message);
                //throw;
            }
            return null;
        }

        //Método responsável por Atualizar quantidade em estoque por produto
        public async Task<HttpResponseMessage> AtualizarQuantidadeEstoque(int _skuId, int _onHand)
        {
            Log log = new Log();
            try
            {
                string warehouseId = ConfigurationManager.AppSettings["warehouseId"];
                string accountName = ConfigurationManager.AppSettings["accountName"];

                log.WriteLogEstoque("Atualizando quantidade em estoque para o Item " + _skuId + " - Estoque " + warehouseId);

                UpdateInventory updateInventory = new UpdateInventory();

                updateInventory.quantity = _onHand;

                string jsonUpdateInventory = JsonUtil.ConvertToJsonString(updateInventory);

                string _param = "?an=" + accountName;

                string uri = "api/logistics/pvt/inventory/skus/" + _skuId + "/warehouses/" + warehouseId + _param;

                //Log.WriteLog("URI: "+uri);

                HttpResponseMessage response = await BuildClientLogistics().PutAsync(uri, new StringContent(jsonUpdateInventory, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogEstoque("Exception AtualizarQuantidadeEstoque " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por consultar o Feed de Eventos de Pedidos
        public async Task<HttpResponseMessage> ConsultarFilaDeEventos()
        {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Consultando Fila de Eventos de Pedidos");

                string maxLot = ConfigurationManager.AppSettings["maxLot"];

                string _paramFeed = "?maxlot=" + maxLot;

                string uriFeed = "api/orders/feed" + _paramFeed;

                HttpResponseMessage responseFeed = await BuildClientPedido().GetAsync(uriFeed);

                log.WriteLogPedido("Consulta realizada.");

                return responseFeed;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception ConsultarFilaDeEventos " + e.InnerException.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PedidosACancelar()
        {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Consultando Pedidos Pendentes");

                string _param = "?orderBy=creationDate,asc&per_page=30&f_status=payment-pending&f_paymentNames=Boleto Bancário";

                string uri = "api/oms/pvt/orders" + _param;

                HttpResponseMessage responseOrderFiltered = await BuildClientPedido().GetAsync(uri);

                return responseOrderFiltered;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception PedidosACancelar " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por remover Pedido da fila de Eventos.
        public async Task<HttpResponseMessage> AtualizaFilaEnvetoPedido(string _handle)
        {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Atualizando Fila de Eventos de Pedidos.");

                string maxLot = ConfigurationManager.AppSettings["maxLot"];

                string postContent = "{\"handles\":[\"" + _handle + "\"]}";

                //Handle handle = new Handle();
                //handle.insertHandle(_handle);
                //var jsonUpdate = JsonConvert.SerializeObject(handle);

                string uri = "api/orders/feed";

                HttpResponseMessage response = await BuildClientPedido().PostAsync(uri, new StringContent(postContent, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception AtualizaFilaEnvetoPedido " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por recuperar Pedido Vtex
        public async Task<HttpResponseMessage> BuscarPedido(string orderId)
        {
            Log log = new Log();
            try
            {
                log.WriteLogPedido("Buscando Pedido " + orderId);

                string uriOrder = "api/oms/pvt/orders/" + orderId;

                HttpResponseMessage responseOrder = await BuildClientPedido().GetAsync(uriOrder);

                return responseOrder;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception BuscarPedido " + e.InnerException.Message);
                throw;
            }
        }


        public async Task<HttpResponseMessage> CancelarPedido(string orderId)
        {
            Log log = new Log();
            try
            {
                //log.WriteLogPedido("Buscando Pedido " + orderId);

                string uriOrder = "api/oms/pvt/orders/" + orderId + "/cancel";

                HttpResponseMessage responseOrder = await BuildClientPedido().PostAsync(uriOrder, null);

                return responseOrder;
            }
            catch (HttpRequestException)
            {
                //log.WriteLogPedido("Exception dido " + e.InnerException.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> RetornoNotaFiscal(Invoice invoice, string idOrderVtex)
        {
            Log log = new Log();
            try
            {
                //log.WriteLogPedido("Fazendo Post número NF");

                string uri = "api/oms/pvt/orders/" + idOrderVtex + "/invoice";

                var jsonInvoice = JsonConvert.SerializeObject(invoice);

                HttpResponseMessage response = await BuildClient().PostAsync(uri, new StringContent(jsonInvoice, UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception RetornoNotaFiscal " + e.InnerException.Message);
                throw;
            }
        }

        //Método responsável por Iniciar Manuseio pedido
        public async Task<HttpResponseMessage> InciarManuseio(string _orderId)
        {
            Log log = new Log();

            try
            {
                log.WriteLogPedido("Setando status Iniciar Manuseio pedido: " + _orderId);

                string uri = "/api/oms/pvt/orders/{{orderId}}/start-handling".Replace("{{orderId}}", _orderId);

                HttpResponseMessage response = await BuildClientPedido().PostAsync(uri, new StringContent("", UnicodeEncoding.UTF8, "application/json"));

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception AtualizaFilaEnvetoPedido " + e.InnerException.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> RetornoCodigoRastreio(object invoice, string idOrderVtex, string invoiceNumber)
        {
            Log log = new Log();

            try
            {
                log.WriteLogPedido("Retorno codigo rastreio: " + idOrderVtex);

                string uri = "api/oms/pvt/orders/" + idOrderVtex + "/invoice/" + invoiceNumber;

                HttpContent httpContent = default(HttpContent);
                httpContent = (HttpContent)new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");

                HttpRequestMessage httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), string.Concat(BaseService.BuildClient().BaseAddress, uri));
                httpRequest.Headers.TryAddWithoutValidation("accept", "application/json");
                httpRequest.Headers.Add("x-vtex-api-appKey", ConfigurationManager.AppSettings["appKey"]);
                httpRequest.Headers.Add("x-vtex-api-appToken", ConfigurationManager.AppSettings["appToken"]);
                httpRequest.Content = httpContent;

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = default(HttpResponseMessage);

                try
                {
                    TaskAwaiter<HttpResponseMessage> awaiter = httpClient.SendAsync(httpRequest).GetAwaiter();
                    response = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    httpClient = null;
                }

                return response;
            }
            catch (HttpRequestException e)
            {
                log.WriteLogPedido("Exception RetornoCodigoRastreio " + e.InnerException.Message);
                throw;
            }
            finally
            {

            }
        }
    }
}
