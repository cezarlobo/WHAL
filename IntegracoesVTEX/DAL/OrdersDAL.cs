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
                if (oOrderNum != 0)
                {
                    messageError = oCompany.GetLastErrorDescription();
                    log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, "", EnumStatusIntegracao.Erro, messageError);
                    log.WriteLogPedido("InsertOrder error SAP: " + messageError);
                    Marshal.ReleaseComObject(oOrder);
                    return oOrderNum;
                }
                messageError = "";
                string docNum = oCompany.GetNewObjectKey();
                log.WriteLogTable(oCompany, EnumTipoIntegracao.PedidoVenda, pedido.orderId, docNum, EnumStatusIntegracao.Sucesso, "Pedido de venda inserido com sucesso.");
                log.WriteLogPedido("Pedido de venda inserido com sucesso.");
                Marshal.ReleaseComObject(oOrder);
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
                    _query = string.Format("SELECT T0.DocNum AS docNPV ,T0.NumAtCard AS idOrderVtex , T0.U_NumPedEXT AS idOrderVtex2 ,T2.DocEntry AS externalId ,T2.DocNum AS docSAP ,T2.Serial AS invoiceNumber ,T2.DocDate AS invoiceDate ,T3.KeyNfe AS nfeKey ,T0.PickRmrk AS shippingMethod ,T2.SeriesStr AS invoiceOrderSeries ,T1.ItemCode AS codItem ,T1.Price AS precoItem ,T1.Quantity AS qtdItem ,T0.DocTotal AS totalNF FROM    ORDR T0 INNER JOIN INV1 T1 ON T0.DocEntry = T1.BaseEntry  INNER JOIN OINV T2 ON T1.DocEntry = T2.DocEntry and T0.BPLId = T2.BPLId  INNER JOIN [DBInvOne].[dbo].[Process] T3 on T3.DocEntry = T2.DocEntry WHERE T0.U_PLATF = '{0}' AND    T2.U_EnvioNFVTEX IS NULL ORDER BY docNPV desc ", ConfigurationManager.AppSettings["Plataforma"]);
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
                log.WriteLogEstoque("Exception recuperarSaldoEstoqueSAP " + e.Message);
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
                    _query = string.Format("SELECT DISTINCT U.[U_CodRastreio] AS codigoRastreamento , R.CardCode AS cardCode , R.CardName AS nomeDestinatario , R.E_Mail AS emailDestinatario , T2.Serial AS invoiceNumber , T0.DocNum AS docNPV  , T0.NumAtCard AS idOrderVtex  , T0.U_NumPedEXT AS idOrderVtex2 , T2.DocEntry AS docEntry  , T2.DocNum AS docNum FROM    ORDR T0  INNER JOIN INV1 T1 ON T0.DocEntry = T1.BaseEntry   INNER JOIN OINV T2 ON T1.DocEntry = T2.DocEntry and T0.BPLId = T2.BPLId   INNER JOIN [DBInvOne].[dbo].[Process] T3 on T3.DocEntry = T2.DocEntry  INNER JOIN OCRD R ON R.CardCode = T0.CardCode INNER JOIN [WahlClipper].[dbo].[@WAHL_COD_RASTREIO] U ON U.[U_NumNF] = T2.Serial WHERE T0.U_PLATF = '{0}' AND T2.U_EnvioNFVTEX = 'S' AND (T2.U_ValidaEnvioCodRastreio IS NULL OR T2.U_ValidaEnvioCodRastreio = '') AND T2.Serial IN (SELECT DISTINCT [U_NumNF] FROM [WahlClipper].[dbo].[@WAHL_COD_RASTREIO]) AND (R.E_Mail IS NOT NULL OR R.E_Mail <> '') AND YEAR (T2.TaxDate) >= '2023' ORDER BY docNPV DESC  ", ConfigurationManager.AppSettings["Plataforma"]);
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
                log.WriteLogEstoque("Exception recuperarCód.Rastreio " + e.Message);
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
                log.WriteLogPedido("Atualizando Pedido de Venda - NF enviada p/ VTEX");
                Documents oInvoice = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                if (oInvoice.GetByKey(docEntry))
                {
                    oInvoice.UserFields.Fields.Item("U_EnvioNFVTEX").Value = "S";
                    if (oInvoice.Update() != 0)
                    {
                        string messageError = oCompany.GetLastErrorDescription();
                        log.WriteLogPedido("AtualizarPedidoVenda error SAP: " + messageError);
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

        public int AtualizarPedidoVendaIntRastreamento(Company company, int docEntry)
        {
            log = new Log();
            try
            {
                oCompany = company;
                log.WriteLogPedido("Atualizando Pedido de Venda - Código de Rastreio enviado p/ VTEX");
                Documents oInvoice = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                if (oInvoice.GetByKey(docEntry))
                {
                    oInvoice.UserFields.Fields.Item("U_ValidaEnvioCodRastreio").Value = "S";
                    if (oInvoice.Update() != 0)
                    {
                        string messageError = oCompany.GetLastErrorDescription();
                        log.WriteLogPedido("AtualizarPedidoVenda error SAP: " + messageError);
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
