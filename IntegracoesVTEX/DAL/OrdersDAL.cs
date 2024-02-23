using System;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using IntegracoesVETX.Entity;
using IntegracoesVETX.Util;
using SAPbobsCOM;

namespace IntegracoesVETX.DAL
{
    public class OrdersDAL
    {
        private Company oCompany;

        private Log log;

        internal OrdersDAL(Company company)
        {
            oCompany = company;
        }

        public int InsertOrder(Pedido pedido, out string messageError)
        {
            log = new Log();
            try
            {
                int oOrderNum = 0;
                log.WriteLogPedido("Inserindo Pedido de Venda");
                Documents oOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
                int filial = Convert.ToInt32(ConfigurationManager.AppSettings["Empresa"]);
                string usage = ConfigurationManager.AppSettings["Usage"];
                string WhsCode = ConfigurationManager.AppSettings["WhsCode"];
                int SlpCode = Convert.ToInt32(ConfigurationManager.AppSettings["SlpCode"]);
                string comments = ConfigurationManager.AppSettings["Comments"];
                string plataforma = ConfigurationManager.AppSettings["Plataforma"];
                string carrier = ConfigurationManager.AppSettings["Carrier"];
                string packDesc = ConfigurationManager.AppSettings["PackDesc"];
                int qoP = Convert.ToInt32(ConfigurationManager.AppSettings["QoP"]);
                int expnsCode = Convert.ToInt32(ConfigurationManager.AppSettings["ExpnsCode"]);
                string expnsTax = ConfigurationManager.AppSettings["ExpnsTax"];
                string cardCodePrefix = ConfigurationManager.AppSettings["CardCodePrefix"];
                string text = ConfigurationManager.AppSettings["PickRemark"];
                oOrder.BPL_IDAssignedToInvoice = filial;
                oOrder.NumAtCard = pedido.orderId;
                oOrder.SalesPersonCode = SlpCode;
                oOrder.Comments = comments;
                oOrder.UserFields.Fields.Item("U_PLATF").Value = plataforma;
                oOrder.UserFields.Fields.Item("U_NumPedEXT").Value = pedido.orderId;
                oOrder.TaxExtension.Carrier = carrier;
                oOrder.TaxExtension.PackDescription = packDesc;
                oOrder.TaxExtension.PackQuantity = qoP;
                oOrder.Expenses.ExpenseCode = expnsCode;
                oOrder.Expenses.TaxCode = expnsTax;
                if (!string.IsNullOrEmpty(pedido.clientProfileData.document))
                {
                    if (!string.IsNullOrEmpty(pedido.clientProfileData.corporateDocument))
                    {
                        oOrder.CardCode = cardCodePrefix + pedido.clientProfileData.corporateDocument;
                    }
                    else
                    {
                        oOrder.CardCode = cardCodePrefix + pedido.clientProfileData.document;
                    }
                }
                if (string.IsNullOrEmpty(pedido.sequence))
                {
                    log.WriteLogPedido("Código Sequence em branco.");
                }
                else
                {
                    oOrder.UserFields.Fields.Item("U_IdPedidoVtex").Value = pedido.sequence;
                }
                Transaction[] transactions = pedido.paymentData.transactions;
                for (int i = 0; i < transactions.Length; i++)
                {
                    Payment[] payments = transactions[i].payments;
                    foreach (Payment paym in payments)
                    {
                        string tid = paym.tid;
                        if (string.IsNullOrEmpty(paym.tid))
                        {
                            log.WriteLogPedido("Código TID em branco.");
                            oOrder.UserFields.Fields.Item("U_IdTidVtex").Value = "";
                        }
                        else
                        {
                            oOrder.UserFields.Fields.Item("U_IdTidVtex").Value = paym.tid;
                        }
                    }
                }
                if (pedido.shippingData.logisticsInfo.Length != 0)
                {
                    Logisticsinfo[] logisticsInfo = pedido.shippingData.logisticsInfo;
                    foreach (Logisticsinfo logInfo in logisticsInfo)
                    {
                        oOrder.DocDueDate = DateTime.Parse(logInfo.shippingEstimateDate);
                        if (!string.IsNullOrEmpty(logInfo.deliveryCompany))
                        {
                            oOrder.PickRemark = logInfo.deliveryCompany;
                        }
                    }
                }
                double _valorFrete = 0.0;
                if (pedido.totals.Length != 0)
                {
                    Total[] totals = pedido.totals;
                    foreach (Total total in totals)
                    {
                        if (total.id.Equals("Discounts") && total.value != 0)
                        {
                            Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                        }
                        if (total.id.Equals("Shipping") && total.value != 0)
                        {
                            _valorFrete = Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                        }
                        if (total.id.Equals("Tax") && total.value != 0)
                        {
                            Convert.ToDouble(total.value.ToString().Insert(total.value.ToString().Length - 2, ","));
                        }
                    }
                }
                oOrder.Expenses.LineGross = _valorFrete;
                if (pedido.items.Length != 0)
                {
                    int _lineNum = 0;
                    ItemVtex[] items = pedido.items;
                    foreach (ItemVtex item in items)
                    {
                        if (item.refId != null)
                        {
                            oOrder.Lines.ItemCode = item.refId;
                            oOrder.Lines.Quantity = item.quantity;
                            oOrder.Lines.WarehouseCode = WhsCode;
                            oOrder.Lines.Usage = usage;

                            if (item.PriceTags != null && item.PriceTags.Any(p => p.name.StartsWith("DISCOUNT@MARKETPLACE")))
                            {
                                var tag = item.PriceTags.Where(p => p.name.StartsWith("DISCOUNT@MARKETPLACE")).First();
                                int desconto = tag.value * -1; // Converte negativo em positivo
                                double percentual = (desconto / item.price) * 100;
                                oOrder.Lines.DiscountPercent = percentual;
                            }

                            oOrder.Lines.SetCurrentLine(_lineNum);
                            oOrder.Lines.Add();
                        }
                        _lineNum++;
                    }
                }
                oOrderNum = oOrder.Add();

                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string sql = string.Empty;

                if (oOrderNum != 0)
                {
                    messageError = oCompany.GetLastErrorDescription();
                    log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, "", EnumStatusIntegracao.Erro, messageError);
                    log.WriteLogPedido("InsertOrder error SAP: " + messageError);
                    Marshal.ReleaseComObject(oOrder);

                    // modifica status para 2 = Erro
                    sql = string.Format(DAL.SQL.Queries.VTEX_PedidoStatus, pedido.orderId, 2);
                    oRS.DoQuery(sql);
                    Marshal.ReleaseComObject(oRS);

                    return oOrderNum;
                }

                messageError = "";
                string docNum = oCompany.GetNewObjectKey();
                log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, docNum, EnumStatusIntegracao.Sucesso, "Pedido de venda inserido com sucesso.");
                log.WriteLogPedido("Pedido de venda inserido com sucesso.");
                Marshal.ReleaseComObject(oOrder);

                // modifica status para 1 = OK
                sql = string.Format(DAL.SQL.Queries.VTEX_PedidoStatus, pedido.orderId, 1);
                oRS.DoQuery(sql);
                Marshal.ReleaseComObject(oRS);

                return oOrderNum;
            }
            catch (Exception e)
            {
                log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, "", EnumStatusIntegracao.Erro, e.Message);
                log.WriteLogPedido("Excpetion InsertOrder. " + e.Message);
                throw;
            }
        }

        public Recordset RecuperarNumeroNF()
        {
            string _query = string.Empty;
            Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                if (oCompany.Connected)
                {
                    recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    _query = string.Format(DAL.SQL.Queries.SAP_DadosNF, ConfigurationManager.AppSettings["Plataforma"]);
                    recordSet.DoQuery(_query);
                    if (recordSet.RecordCount > 0)
                    {
                        return recordSet;
                    }
                }
            }
            catch (Exception e)
            {
                log = new Log();
                log.WriteLogRetornoNF("Exception recuperar dados da NF SAP " + e.Message);
                throw;
            }
            return recordSet;
        }

        public Recordset RecuperarCodigoRastreamento()
        {
            string _query = string.Empty;
            Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                if (oCompany.Connected)
                {
                    recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    _query = string.Format(DAL.SQL.Queries.SAP_BuscaCodRastreio, ConfigurationManager.AppSettings["Plataforma"]);
                    recordSet.DoQuery(_query);
                    if (recordSet.RecordCount > 0)
                    {
                        return recordSet;
                    }
                }
            }
            catch (Exception e)
            {
                log = new Log();
                log.WriteLogRetornoCodRastreio("Exception - recuperar dados do Cód.Rastreio no SAP" + e.Message);
                throw;
            }
            return recordSet;
        }

        public int AtualizarPedidoVenda(Company company, int docEntry)
        {
            log = new Log();
            try
            {
                oCompany = company;
                log.WriteLogRetornoNF("Atualizando Pedido de Venda - NF enviada p/ VTEX");
                Documents oInvoice = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                if (oInvoice.GetByKey(docEntry))
                {
                    oInvoice.UserFields.Fields.Item("U_EnvioNFVTEX").Value = "S";
                    if (oInvoice.Update() != 0)
                    {
                        string messageError = oCompany.GetLastErrorDescription();
                        log.WriteLogRetornoNF("Erro ao atualizar NF no SAP: " + messageError);
                        Marshal.ReleaseComObject(oInvoice);
                        return 1;
                    }
                    Marshal.ReleaseComObject(oInvoice);
                    return 0;
                }
                return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public int AtualizarPedidoVendaIntRastreamento(Company company, int docEntry, string codRastreio)
        {
            log = new Log();
            try
            {
                oCompany = company;
                log.WriteLogRetornoCodRastreio("Atualizando NF - Com Código de Rastreio enviado p/ VTEX");
                Documents oInvoice = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                if (oInvoice.GetByKey(docEntry))
                {
                    oInvoice.UserFields.Fields.Item("U_ValidaEnvioCodRastreio").Value = "S";
                    oInvoice.UserFields.Fields.Item("U_CodRastreio").Value = codRastreio;
                    if (oInvoice.Update() != 0)
                    {
                        string messageError = oCompany.GetLastErrorDescription();
                        log.WriteLogRetornoCodRastreio("Erro ao atualizar no SAP: " + messageError);
                        Marshal.ReleaseComObject(oInvoice);
                        return 1;
                    }
                    Marshal.ReleaseComObject(oInvoice);
                    return 0;
                }
                return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
