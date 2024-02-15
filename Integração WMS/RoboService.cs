// Decompiled with JetBrains decompiler
// Type: Integração.WMS.RoboService
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe

using Integração.WMS.Modelos;
using Integração.WMS.Properties;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using SAPbobsCOM;
using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;


namespace Integração.WMS
{
    public class RoboService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int sleepTime;
        private static string JsonEvioProduto = string.Empty;
        private static string JsonRetornoProduto = string.Empty;
        private static string JsonEvioGrupoProduto = string.Empty;
        private static string JsonRetornoGrupoProduto = string.Empty;
        private static string JsonEvioEmresa = string.Empty;
        private static string JsonRetornoEmresa = string.Empty;

        public RoboService()
        {
            this.sleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["SleepTime"]);
        }

        public void Run()
        {
            try
            {
                RoboService.log.Info((object)"=============================================================================");
                RoboService.log.Info((object)"=============================================================================");
                RoboService.log.Info((object)"Start Service");
                new Task((Action)(() =>
               {
                   SAP.Connect();
                   while (true)
                   {
                       try
                       {
                           LoginRet loginRet = this.Login();
                           if (!loginRet.success)
                           {
                               RoboService.log.Error((object)("Falha no Login do WebService: " + loginRet.message));
                           }
                           else
                           {
                               string token = loginRet.data.data.token;
                               RoboService.log.Info((object)"início da Integração:");
                               this.IntegraOPLiberada(token);
                               this.IntegraOPFechada(token);
                               this.IntegraNotaEntrada(token);
                               this.IntegraDevSaida(token);
                               this.IntegraDevolucao(token);
                               this.IntegraNotaSaida(token);
                               RoboService.log.Info((object)"Fim da Integração:");
                           }
                           Thread.Sleep(new TimeSpan(0, 0, this.sleepTime));
                       }
                       catch (Exception ex)
                       {
                           SAP.Disconnect();
                           RoboService.log.Fatal((object)("Erro Serviço: = " + ex.Message));
                           Thread.Sleep(new TimeSpan(0, 0, this.sleepTime));
                       }
                   }
               })).Start();
            }
            catch (Exception ex)
            {
                RoboService.log.Fatal((object)("Erro Geral: = " + ex.Message));
            }
        }

        private void IntegrarFornecedor(string Token)
        {
            Recordset businessObject = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr = string.Format(Resources.ConsultaPN, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)"S");
                businessObject.DoQuery(QueryStr);
                while (!businessObject.EoF)
                {
                    string CardCode = businessObject.Fields.Item((object)"CardCode").Value.ToString();
                    EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 67);
                    if (!empresaRet.success)
                        RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                    businessObject.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegrarCliente(string Token)
        {
            Recordset businessObject = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr = string.Format(Resources.ConsultaPN, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)"C");
                businessObject.DoQuery(QueryStr);
                while (!businessObject.EoF)
                {
                    string CardCode = businessObject.Fields.Item((object)"CardCode").Value.ToString();
                    EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 66);
                    if (!empresaRet.success)
                        RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                    businessObject.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraOPLiberada(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaOPS, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"DocEntry").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string str2 = "";
                    string QueryStr2 = string.Format(Resources.ConsultaOP, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    PedidoVenda pedidoVenda = new PedidoVenda();
                    bool flag = true;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    string str5 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string str6 = businessObject2.Fields.Item((object)"cliente_cnpj").Value.ToString();
                        string str7 = businessObject2.Fields.Item((object)"pedido_venda").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"data_entrega").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"prefixo").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"cod_rota").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"sequencia_entrega").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"transp_nome").Value.ToString();
                        string str13 = businessObject2.Fields.Item((object)"cod_ponto_entrega").Value.ToString();
                        string str14 = businessObject2.Fields.Item((object)"desc_ponto_entrega").Value.ToString();
                        string s = businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        string str15 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str16 = businessObject2.Fields.Item((object)"seria").Value.ToString();
                        businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str17 = businessObject2.Fields.Item((object)"canal").Value.ToString();
                        pedidoVenda.token = Token;
                        pedidoVenda.pedidos = new Pedido[1];
                        pedidoVenda.pedidos[0] = new Pedido();
                        pedidoVenda.pedidos[0].cliente_cnpj = str6.Replace("/", "").Replace("-", "").Replace(".", "");
                        pedidoVenda.pedidos[0].pedido_venda = str7;
                        pedidoVenda.pedidos[0].data_entrega = str8;
                        pedidoVenda.pedidos[0].prefixo = str9;
                        pedidoVenda.pedidos[0].cod_rota = str10;
                        pedidoVenda.pedidos[0].sequencia_entrega = str11;
                        pedidoVenda.pedidos[0].transp_nome = str12;
                        pedidoVenda.pedidos[0].cod_ponto_entrega = str13;
                        pedidoVenda.pedidos[0].desc_ponto_entrega = str14;
                        pedidoVenda.pedidos[0].qtde_itens = s;
                        pedidoVenda.pedidos[0].canal = str17;
                        pedidoVenda.pedidos[0].notas_fiscais = new Notas_FiscaisPedidoVenda[1];
                        pedidoVenda.pedidos[0].notas_fiscais[0] = new Notas_FiscaisPedidoVenda();
                        pedidoVenda.pedidos[0].notas_fiscais[0].nota_fiscal = str15;
                        pedidoVenda.pedidos[0].notas_fiscais[0].serie = str16;
                        pedidoVenda.pedidos[0].notas_fiscais[0].chave_acesso = str2;
                        string QueryStr3 = string.Format(Resources.ConsultaOPItens, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                        businessObject3.DoQuery(QueryStr3);
                        pedidoVenda.pedidos[0].itens = new Iten[int.Parse(s)];
                        int index = 0;
                        while (!businessObject3.EoF)
                        {
                            string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                            string str18 = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                            pedidoVenda.pedidos[0].itens[index] = new Iten();
                            pedidoVenda.pedidos[0].itens[index].pn = ItemCode;
                            pedidoVenda.pedidos[0].itens[index].qtde = str18;
                            GrupoProdutoRet grupoProdutoRet = this.IntegraGrupoProduto(ItemCode, Token);
                            if (!grupoProdutoRet.success)
                            {
                                RoboService.log.Error((object)("Cadastro de Grupo: " + grupoProdutoRet.message));
                                str3 = "Cadastro de Grupo: " + grupoProdutoRet.message;
                                str4 = RoboService.JsonEvioGrupoProduto;
                                str5 = RoboService.JsonRetornoGrupoProduto;
                                flag = false;
                            }
                            else
                            {
                                ProdutoRet produtoRet = this.IntegraProduto(ItemCode, Token);
                                if (!produtoRet.success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + produtoRet.message));
                                    str3 = "Cadastro de Produto: " + produtoRet.message;
                                    str4 = RoboService.JsonEvioProduto;
                                    str5 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                            }
                            ++index;
                            businessObject3.MoveNext();
                        }
                        if (flag)
                        {
                            string str19 = JsonConvert.SerializeObject((object)pedidoVenda, Formatting.Indented, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "pedidos");
                            RestRequest request = new RestRequest(Method.POST);
                            request.AddHeader("content-type", "application/json");
                            request.AddParameter("application/json", (object)str19, ParameterType.RequestBody);
                            string str20 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                            PedidoVendaRet pedidoVendaRet = JsonConvert.DeserializeObject<PedidoVendaRet>(str20, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            str4 = str19;
                            str5 = str20;
                            if (pedidoVendaRet == null)
                            {
                                pedidoVendaRet = new PedidoVendaRet();
                                pedidoVendaRet.success = false;
                                pedidoVendaRet.message = "Erro ao Cadastrar Pedido";
                            }
                            if (!pedidoVendaRet.success)
                            {
                                if (pedidoVendaRet.data == null)
                                {
                                    RoboService.log.Error((object)("Cadastro da OP " + str7 + ": " + pedidoVendaRet.message.Replace("'", "")));
                                    str3 = "Cadastro da OP " + str7 + ": " + pedidoVendaRet.message.Replace("'", "");
                                    flag = false;
                                }
                                else
                                {
                                    RoboService.log.Error((object)("Cadastro da OP " + str7 + ": " + pedidoVendaRet.data[0].error.Replace("'", "")));
                                    str3 = "Cadastro da OP " + str7 + ": " + pedidoVendaRet.data[0].error.Replace("'", "");
                                    flag = false;
                                }
                            }
                            else
                            {
                                string QueryStr4 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"OP", (object)"", (object)2, (object)str4, (object)str5.Replace("'", ""), (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                businessObject4.DoQuery(QueryStr4);
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"OP", (object)str3, (object)3, (object)str4, (object)str5.Replace("'", ""), (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraOPFechada(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaOPSFechada, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"DocEntry").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string QueryStr2 = string.Format(Resources.ConsultaOPFechada, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    Entrada entrada = new Entrada();
                    bool flag = true;
                    string str2 = string.Empty;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string str5 = businessObject2.Fields.Item((object)"DocNum").Value.ToString();
                        string str6 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str7 = businessObject2.Fields.Item((object)"data_emissao").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"emitente").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"chave_acesso").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"nr").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"tipo").Value.ToString();
                        businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"apontamento_producao").Value.ToString();
                        entrada.token = Token;
                        entrada.nota_fiscal = str6;
                        entrada.data_emissao = str7;
                        entrada.emitente = str8.Replace("/", "").Replace("-", "").Replace(".", "");
                        entrada.chave_acesso = str9;
                        entrada.nr = str10;
                        entrada.tipo = str11;
                        entrada.apontamento_producao = str12;
                        string QueryStr3 = string.Format(Resources.ConsultaOPItensFechada, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                        businessObject3.DoQuery(QueryStr3);
                        entrada.itens = new ItenEntrada[1];
                        int index = 0;
                        while (!businessObject3.EoF)
                        {
                            string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                            string s = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                            string str13 = businessObject3.Fields.Item((object)"valor_unitario").Value.ToString();
                            string str14 = businessObject3.Fields.Item((object)"peso").Value.ToString();
                            entrada.itens[index] = new ItenEntrada();
                            entrada.itens[index].pn = ItemCode;
                            entrada.itens[index].qtde = (double)int.Parse(s);
                            entrada.itens[index].valor_unitario = Convert.ToDouble(str13);
                            entrada.itens[index].peso = str14;
                            GrupoProdutoRet grupoProdutoRet = this.IntegraGrupoProduto(ItemCode, Token);
                            if (!grupoProdutoRet.success)
                            {
                                RoboService.log.Error((object)("Cadastro de Grupo: " + grupoProdutoRet.message));
                                str2 = "Cadastro de Grupo: " + grupoProdutoRet.message;
                                str3 = RoboService.JsonEvioGrupoProduto;
                                str4 = RoboService.JsonRetornoGrupoProduto;
                                flag = false;
                            }
                            else
                            {
                                ProdutoRet produtoRet = this.IntegraProduto(ItemCode, Token);
                                if (!produtoRet.success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + produtoRet.message));
                                    str2 = "Cadastro de Produto: " + produtoRet.message;
                                    str3 = RoboService.JsonEvioProduto;
                                    str4 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                            }
                            ++index;
                            businessObject3.MoveNext();
                        }
                        if (flag)
                        {
                            string str15 = JsonConvert.SerializeObject((object)entrada, Formatting.Indented, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "entrada");
                            RestRequest request = new RestRequest(Method.POST);
                            request.AddHeader("content-type", "application/json");
                            request.AddParameter("application/json", (object)str15, ParameterType.RequestBody);
                            string str16 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                            EntradaRet entradaRet = JsonConvert.DeserializeObject<EntradaRet>(str16, new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            str3 = str15;
                            str4 = str16;
                            if (entradaRet == null)
                            {
                                entradaRet = new EntradaRet();
                                entradaRet.success = false;
                                entradaRet.message = "Erro ao Cadastrar Nota de Entrada";
                            }
                            if (!entradaRet.success)
                            {
                                RoboService.log.Error((object)("Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message));
                                str2 = "Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message;
                                flag = false;
                            }
                            else
                            {
                                string QueryStr4 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"OPF", (object)"", (object)2, (object)str15, (object)str16, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                businessObject4.DoQuery(QueryStr4);
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"OPF", (object)str2.Replace("'", ""), (object)3, (object)str3.Replace("'", ""), (object)str4.Replace("'", ""), (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraNotaSaida(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaNotasSaida, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"DocEntry").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string str2 = businessObject1.Fields.Item((object)"KeyNfe").Value.ToString();
                    string QueryStr2 = string.Format(Resources.ConsultaNotaSaida, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    PedidoVenda pedidoVenda = new PedidoVenda();
                    bool flag = true;
                    string message = string.Empty;
                    string str4 = string.Empty;
                    string str5 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string docEntry = businessObject2.Fields.Item("DocEntry").Value.ToString();
                        string str6 = businessObject2.Fields.Item((object)"cliente_cnpj").Value.ToString();
                        string docNum = businessObject2.Fields.Item((object)"pedido_venda").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"data_entrega").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"prefixo").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"cod_rota").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"sequencia_entrega").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"transp_nome").Value.ToString();
                        string str13 = businessObject2.Fields.Item((object)"cod_ponto_entrega").Value.ToString();
                        string str14 = businessObject2.Fields.Item((object)"desc_ponto_entrega").Value.ToString();
                        string s = businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        string str15 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str16 = businessObject2.Fields.Item((object)"seria").Value.ToString();
                        string CardCode = businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str17 = businessObject2.Fields.Item((object)"canal").Value.ToString();
                        EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 66);
                        if (!empresaRet.success)
                        {
                            RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                            message = "Cadastro de Empresa: " + empresaRet.message.Replace("'", "").Replace("\"", "");

                            str4 = RoboService.JsonEvioEmresa;
                            str5 = RoboService.JsonRetornoEmresa;
                            flag = false;
                        }
                        else
                        {
                            pedidoVenda.token = Token;
                            pedidoVenda.pedidos = new Pedido[1];
                            pedidoVenda.pedidos[0] = new Pedido();
                            pedidoVenda.pedidos[0].cliente_cnpj = str6.Replace("/", "").Replace("-", "").Replace(".", "");
                            pedidoVenda.pedidos[0].pedido_venda = docNum;
                            pedidoVenda.pedidos[0].data_entrega = str8;
                            pedidoVenda.pedidos[0].prefixo = str9;
                            pedidoVenda.pedidos[0].cod_rota = str10;
                            pedidoVenda.pedidos[0].sequencia_entrega = str11;
                            pedidoVenda.pedidos[0].transp_nome = str12;
                            pedidoVenda.pedidos[0].cod_ponto_entrega = str13;
                            pedidoVenda.pedidos[0].desc_ponto_entrega = str14;
                            pedidoVenda.pedidos[0].qtde_itens = s;
                            pedidoVenda.pedidos[0].canal = str17;
                            pedidoVenda.pedidos[0].notas_fiscais = new Notas_FiscaisPedidoVenda[1];
                            pedidoVenda.pedidos[0].notas_fiscais[0] = new Notas_FiscaisPedidoVenda();
                            pedidoVenda.pedidos[0].notas_fiscais[0].nota_fiscal = str15;
                            pedidoVenda.pedidos[0].notas_fiscais[0].serie = str16;
                            pedidoVenda.pedidos[0].notas_fiscais[0].chave_acesso = str2;
                            string QueryStr3 = string.Format(Resources.ConsultaNotaSaidaItens, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                            businessObject3.DoQuery(QueryStr3);
                            pedidoVenda.pedidos[0].itens = new Iten[int.Parse(s)];
                            int index = 0;
                            while (!businessObject3.EoF)
                            {
                                string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                                string str18 = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                                pedidoVenda.pedidos[0].itens[index] = new Iten();
                                pedidoVenda.pedidos[0].itens[index].pn = ItemCode;
                                pedidoVenda.pedidos[0].itens[index].qtde = str18;
                                if (!this.IntegraGrupoProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Grupo: " + empresaRet.message));
                                    message = "Cadastro de Grupo: " + empresaRet.message;
                                    str4 = RoboService.JsonEvioGrupoProduto;
                                    str5 = RoboService.JsonRetornoGrupoProduto;
                                    flag = false;
                                }
                                else if (!this.IntegraProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + empresaRet.message));
                                    message = "Cadastro de Produto: " + empresaRet.message;
                                    str4 = RoboService.JsonEvioProduto;
                                    str5 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                                ++index;
                                businessObject3.MoveNext();
                            }
                        }
                        if (flag)
                        {
                            try
                            {
                                string str19 = JsonConvert.SerializeObject((object)pedidoVenda, Formatting.Indented, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "pedidos");
                                RestRequest request = new RestRequest(Method.POST);
                                request.AddHeader("content-type", "application/json");
                                request.AddParameter("application/json", (object)str19, ParameterType.RequestBody);
                                string retornoEnvio = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                                PedidoVendaRet pedidoVendaRet = JsonConvert.DeserializeObject<PedidoVendaRet>(retornoEnvio, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });

                                retornoEnvio = retornoEnvio.Replace("'", "").Replace("\"", "");

                                str4 = str19;
                                str5 = retornoEnvio;

                                if (pedidoVendaRet == null)
                                {
                                    pedidoVendaRet = new PedidoVendaRet();
                                    pedidoVendaRet.success = false;
                                    pedidoVendaRet.message = "Erro ao Cadastrar Pedido";
                                }
                                if (!pedidoVendaRet.success)
                                {
                                    string error = string.Empty;
                                    if (pedidoVendaRet.data != null)
                                    {
                                        error = pedidoVendaRet.data[0].error.Replace("'", "").Replace("\"", "");
                                    }
                                    else
                                    {
                                        error = pedidoVendaRet.message;
                                    }

                                    RoboService.log.Error((object)("Cadastro de Pedido " + docEntry + ": "
                                        + error));

                                    message = "Cadastro de Pedido " + error;
                                    flag = false;
                                }
                                else
                                {
                                    string QueryStr4 = string.Format(Resources.UpdateIntegracao,
                                        (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(),
                                        (object)str1, (object)"NF", (object)"",
                                        (object)2,
                                        (object)str19,
                                        (object)retornoEnvio,
                                        (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    businessObject4.DoQuery(QueryStr4);
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                message = "Erro no Envio do Web Service!";
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao,
                                (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(),
                                (object)str1, (object)"NF",
                                (object)message.Replace("'", "").Replace("\"", ""),
                                (object)3,
                                (object)str4,
                                (object)str5,
                                (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraNotaEntrada(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaNotasEntrada, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"U_Codigo").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string QueryStr2 = string.Format(Resources.ConsultaNotaEntrada, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    Entrada entrada = new Entrada();
                    bool flag = true;
                    string str2 = string.Empty;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string str5 = businessObject2.Fields.Item((object)"DocNum").Value.ToString();
                        string str6 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str7 = businessObject2.Fields.Item((object)"data_emissao").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"emitente").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"chave_acesso").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"nr").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"tipo").Value.ToString();
                        string s = businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        string CardCode = businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"apontamento_producao").Value.ToString();
                        EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 67);
                        if (!empresaRet.success)
                        {
                            RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                            str2 = "Cadastro de Empresa: " + empresaRet.message;
                            str3 = RoboService.JsonEvioEmresa;
                            str4 = RoboService.JsonRetornoEmresa;
                            flag = false;
                        }
                        else
                        {
                            entrada.token = Token;
                            entrada.nota_fiscal = str6;
                            entrada.data_emissao = str7;
                            entrada.emitente = str8.Replace("/", "").Replace("-", "").Replace(".", "");
                            entrada.chave_acesso = str9;
                            entrada.nr = str10;
                            entrada.tipo = str11;
                            entrada.apontamento_producao = str12;
                            string QueryStr3 = string.Format(Resources.ConsultaNotaEntradaItens, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                            businessObject3.DoQuery(QueryStr3);
                            entrada.itens = new ItenEntrada[int.Parse(s)];
                            int index = 0;
                            while (!businessObject3.EoF)
                            {
                                string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                                string str13 = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                                string str14 = businessObject3.Fields.Item((object)"valor_unitario").Value.ToString();
                                string str15 = businessObject3.Fields.Item((object)"peso").Value.ToString();
                                entrada.itens[index] = new ItenEntrada();
                                entrada.itens[index].pn = ItemCode;
                                entrada.itens[index].qtde = Convert.ToDouble(str13);
                                entrada.itens[index].valor_unitario = Convert.ToDouble(str14);
                                entrada.itens[index].peso = str15;
                                if (!this.IntegraGrupoProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Grupo: " + empresaRet.message));
                                    str2 = "Cadastro de Grupo: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioGrupoProduto;
                                    str4 = RoboService.JsonRetornoGrupoProduto;
                                    flag = false;
                                }
                                else if (!this.IntegraProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + empresaRet.message));
                                    str2 = "Cadastro de Produto: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioProduto;
                                    str4 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                                ++index;
                                businessObject3.MoveNext();
                            }
                        }
                        if (flag)
                        {
                            try
                            {
                                string str16 = JsonConvert.SerializeObject((object)entrada, Formatting.Indented, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "entrada");
                                RestRequest request = new RestRequest(Method.POST);
                                request.AddHeader("content-type", "application/json");
                                request.AddParameter("application/json", (object)str16, ParameterType.RequestBody);
                                string str17 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                                EntradaRet entradaRet = JsonConvert.DeserializeObject<EntradaRet>(str17, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                str3 = str16;
                                str4 = str17;
                                if (entradaRet == null)
                                {
                                    entradaRet = new EntradaRet();
                                    entradaRet.success = false;
                                    entradaRet.message = "Erro ao Cadastrar Nota de Entrada";
                                }
                                if (!entradaRet.success)
                                {
                                    RoboService.log.Error((object)("Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message));
                                    str2 = "Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message;
                                    flag = false;
                                }
                                else
                                {
                                    string QueryStr4 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"NE", (object)"", (object)2, (object)str16, (object)str17, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    businessObject4.DoQuery(QueryStr4);
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                str2 = "Erro no Envio do Web Service!";
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"NE", (object)str2, (object)3, (object)str3, (object)str4, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraDevSaida(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaNotasDevSaida, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"U_Codigo").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string QueryStr2 = string.Format(Resources.ConsultaNotaDevSaida, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    Entrada entrada = new Entrada();
                    bool flag = true;
                    string str2 = string.Empty;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string str5 = businessObject2.Fields.Item((object)"DocNum").Value.ToString();
                        string str6 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str7 = businessObject2.Fields.Item((object)"data_emissao").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"emitente").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"chave_acesso").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"nr").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"tipo").Value.ToString();
                        string s = businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        string CardCode = businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"apontamento_producao").Value.ToString();
                        EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 67);
                        if (!empresaRet.success)
                        {
                            RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                            str2 = "Cadastro de Empresa: " + empresaRet.message;
                            str3 = RoboService.JsonEvioEmresa;
                            str4 = RoboService.JsonRetornoEmresa;
                            flag = false;
                        }
                        else
                        {
                            entrada.token = Token;
                            entrada.nota_fiscal = str6;
                            entrada.data_emissao = str7;
                            entrada.emitente = str8.Replace("/", "").Replace("-", "").Replace(".", "");
                            entrada.chave_acesso = str9;
                            entrada.nr = str10;
                            entrada.tipo = str11;
                            entrada.apontamento_producao = str12;
                            string QueryStr3 = string.Format(Resources.ConsultaNotaDevSaidaItens, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                            businessObject3.DoQuery(QueryStr3);
                            entrada.itens = new ItenEntrada[int.Parse(s)];
                            int index = 0;
                            while (!businessObject3.EoF)
                            {
                                string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                                string str13 = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                                string str14 = businessObject3.Fields.Item((object)"valor_unitario").Value.ToString();
                                string str15 = businessObject3.Fields.Item((object)"peso").Value.ToString();
                                entrada.itens[index] = new ItenEntrada();
                                entrada.itens[index].pn = ItemCode;
                                entrada.itens[index].qtde = Convert.ToDouble(str13);
                                entrada.itens[index].valor_unitario = Convert.ToDouble(str14);
                                entrada.itens[index].peso = str15;
                                if (!this.IntegraGrupoProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Grupo: " + empresaRet.message));
                                    str2 = "Cadastro de Grupo: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioGrupoProduto;
                                    str4 = RoboService.JsonRetornoGrupoProduto;
                                    flag = false;
                                }
                                else if (!this.IntegraProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + empresaRet.message));
                                    str2 = "Cadastro de Produto: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioProduto;
                                    str4 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                                ++index;
                                businessObject3.MoveNext();
                            }
                        }
                        if (flag)
                        {
                            try
                            {
                                string str16 = JsonConvert.SerializeObject((object)entrada, Formatting.Indented, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "entrada");
                                RestRequest request = new RestRequest(Method.POST);
                                request.AddHeader("content-type", "application/json");
                                request.AddParameter("application/json", (object)str16, ParameterType.RequestBody);
                                string str17 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                                EntradaRet entradaRet = JsonConvert.DeserializeObject<EntradaRet>(str17, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                str3 = str16;
                                str4 = str17;
                                if (entradaRet == null)
                                {
                                    entradaRet = new EntradaRet();
                                    entradaRet.success = false;
                                    entradaRet.message = "Erro ao Cadastrar Nota de Entrada";
                                }
                                if (!entradaRet.success)
                                {
                                    RoboService.log.Error((object)("Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message));
                                    str2 = "Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message;
                                    flag = false;
                                }
                                else
                                {
                                    string QueryStr4 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"DS", (object)"", (object)2, (object)str16, (object)str17, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    businessObject4.DoQuery(QueryStr4);
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                str2 = "Erro no Envio do Web Service!";
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"DS", (object)str2, (object)3, (object)str3, (object)str4, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void IntegraDevolucao(string Token)
        {
            Recordset businessObject1 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject2 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject3 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset businessObject4 = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                string QueryStr1 = string.Format(Resources.ConsultaNotasDevolucao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString());
                businessObject1.DoQuery(QueryStr1);
                while (!businessObject1.EoF)
                {
                    string str1 = businessObject1.Fields.Item((object)"U_Codigo").Value.ToString();
                    businessObject1.Fields.Item((object)"Code").Value.ToString();
                    string QueryStr2 = string.Format(Resources.ConsultaNotaDevolucao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                    businessObject2.DoQuery(QueryStr2);
                    Entrada entrada = new Entrada();
                    bool flag = true;
                    string str2 = string.Empty;
                    string str3 = string.Empty;
                    string str4 = string.Empty;
                    while (!businessObject2.EoF)
                    {
                        string str5 = businessObject2.Fields.Item((object)"DocNum").Value.ToString();
                        string str6 = businessObject2.Fields.Item((object)"nota_fiscal").Value.ToString();
                        string str7 = businessObject2.Fields.Item((object)"data_emissao").Value.ToString();
                        string str8 = businessObject2.Fields.Item((object)"emitente").Value.ToString();
                        string str9 = businessObject2.Fields.Item((object)"chave_acesso").Value.ToString();
                        string str10 = businessObject2.Fields.Item((object)"nr").Value.ToString();
                        string str11 = businessObject2.Fields.Item((object)"tipo").Value.ToString();
                        string s1 = businessObject2.Fields.Item((object)"qtde_itens").Value.ToString();
                        string CardCode = businessObject2.Fields.Item((object)"CardCode").Value.ToString();
                        string str12 = businessObject2.Fields.Item((object)"apontamento_producao").Value.ToString();
                        EmpresaRet empresaRet = this.IntegraEmresa(Token, CardCode, 67);
                        if (!empresaRet.success)
                        {
                            RoboService.log.Error((object)("Cadastro de Empresa: " + empresaRet.message));
                            str2 = "Cadastro de Empresa: " + empresaRet.message;
                            str3 = RoboService.JsonEvioEmresa;
                            str4 = RoboService.JsonRetornoEmresa;
                            flag = false;
                        }
                        else
                        {
                            entrada.token = Token;
                            entrada.nota_fiscal = str6;
                            entrada.data_emissao = str7;
                            entrada.emitente = str8.Replace("/", "").Replace("-", "").Replace(".", "");
                            entrada.chave_acesso = str9;
                            entrada.nr = str10;
                            entrada.tipo = str11;
                            entrada.apontamento_producao = str12;
                            string QueryStr3 = string.Format(Resources.ConsultaNotaDevolucaoItens, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1);
                            businessObject3.DoQuery(QueryStr3);
                            entrada.itens = new ItenEntrada[int.Parse(s1)];
                            int index = 0;
                            while (!businessObject3.EoF)
                            {
                                string ItemCode = businessObject3.Fields.Item((object)"pn").Value.ToString();
                                string s2 = businessObject3.Fields.Item((object)"qtde").Value.ToString();
                                string str13 = businessObject3.Fields.Item((object)"valor_unitario").Value.ToString();
                                string str14 = businessObject3.Fields.Item((object)"peso").Value.ToString();
                                entrada.itens[index] = new ItenEntrada();
                                entrada.itens[index].pn = ItemCode;
                                entrada.itens[index].qtde = (double)int.Parse(s2);
                                entrada.itens[index].valor_unitario = Convert.ToDouble(str13);
                                entrada.itens[index].peso = str14;
                                if (!this.IntegraGrupoProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Grupo: " + empresaRet.message));
                                    str2 = "Cadastro de Grupo: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioGrupoProduto;
                                    str4 = RoboService.JsonRetornoGrupoProduto;
                                    flag = false;
                                }
                                else if (!this.IntegraProduto(ItemCode, Token).success)
                                {
                                    RoboService.log.Error((object)("Cadastro de Produto: " + empresaRet.message));
                                    str2 = "Cadastro de Produto: " + empresaRet.message;
                                    str3 = RoboService.JsonEvioProduto;
                                    str4 = RoboService.JsonRetornoProduto;
                                    flag = false;
                                }
                                ++index;
                                businessObject3.MoveNext();
                            }
                        }
                        if (flag)
                        {
                            try
                            {
                                string str15 = JsonConvert.SerializeObject((object)entrada, Formatting.Indented, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "entrada");
                                RestRequest request = new RestRequest(Method.POST);
                                request.AddHeader("content-type", "application/json");
                                request.AddParameter("application/json", (object)str15, ParameterType.RequestBody);
                                string str16 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                                EntradaRet entradaRet = JsonConvert.DeserializeObject<EntradaRet>(str16, new JsonSerializerSettings()
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                                str3 = str15;
                                str4 = str16;
                                if (entradaRet == null)
                                {
                                    entradaRet = new EntradaRet();
                                    entradaRet.success = false;
                                    entradaRet.message = "Erro ao Cadastrar Nota de Entrada";
                                }
                                if (!entradaRet.success)
                                {
                                    RoboService.log.Error((object)("Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message));
                                    str2 = "Erro ao Cadastrar Nota de Entrada " + str5 + ": " + entradaRet.message;
                                    flag = false;
                                }
                                else
                                {
                                    string QueryStr4 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"DE", (object)"", (object)2, (object)str15, (object)str16, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                    businessObject4.DoQuery(QueryStr4);
                                }
                            }
                            catch (Exception ex)
                            {
                                flag = false;
                                str2 = "Erro no Envio do Web Service!";
                            }
                        }
                        if (!flag)
                        {
                            string QueryStr5 = string.Format(Resources.UpdateIntegracao, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)str1, (object)"DE", (object)str2, (object)3, (object)str3, (object)str4, (object)DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                            businessObject4.DoQuery(QueryStr5);
                        }
                        businessObject2.MoveNext();
                    }
                    businessObject1.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject1);
                Marshal.ReleaseComObject((object)businessObject2);
                Marshal.ReleaseComObject((object)businessObject3);
                Marshal.ReleaseComObject((object)businessObject4);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private ProdutoRet IntegraProduto(string ItemCode, string Token)
        {
            Recordset businessObject = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            ProdutoRet produtoRet = new ProdutoRet();
            RoboService.JsonEvioProduto = string.Empty;
            RoboService.JsonRetornoProduto = string.Empty;
            try
            {
                string QueryStr = string.Format(Resources.ConsultaProduto, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)ItemCode);
                businessObject.DoQuery(QueryStr);
                while (!businessObject.EoF)
                {
                    string str1 = businessObject.Fields.Item((object)"codigo").Value.ToString();
                    string str2 = businessObject.Fields.Item((object)"pn").Value.ToString();
                    string str3 = businessObject.Fields.Item((object)"descricao").Value.ToString();
                    string str4 = businessObject.Fields.Item((object)"unid_medida").Value.ToString();
                    string str5 = businessObject.Fields.Item((object)"grupo").Value.ToString();
                    string str6 = businessObject.Fields.Item((object)"tipo").Value.ToString();
                    string str7 = businessObject.Fields.Item((object)"lote").Value.ToString();
                    string str8 = businessObject.Fields.Item((object)"serie").Value.ToString();
                    string str9 = businessObject.Fields.Item((object)"validade").Value.ToString();
                    string str10 = businessObject.Fields.Item((object)"inspecao_rec").Value.ToString();
                    string str11 = businessObject.Fields.Item((object)"inspecao_exp").Value.ToString();
                    string str12 = businessObject.Fields.Item((object)"laudo_rec").Value.ToString();
                    string str13 = businessObject.Fields.Item((object)"laudo_exp").Value.ToString();
                    string str14 = businessObject.Fields.Item((object)"tipo_picking").Value.ToString();
                    string str15 = businessObject.Fields.Item((object)"categoria").Value.ToString();
                    string str16 = JsonConvert.SerializeObject((object)new Produto()
                    {
                        token = Token,
                        codigo = str1,
                        pn = str2,
                        descricao = str3,
                        unid_medida = str4,
                        grupo = str5,
                        tipo = str6,
                        lote = Convert.ToBoolean(str7),
                        serie = Convert.ToBoolean(str8),
                        validade = Convert.ToBoolean(str9),
                        inspecao_rec = Convert.ToBoolean(str10),
                        inspecao_exp = Convert.ToBoolean(str11),
                        laudo_rec = Convert.ToBoolean(str12),
                        laudo_exp = Convert.ToBoolean(str13),
                        tipo_picking = str14,
                        categoria = str15
                    }, Formatting.Indented, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "produtos");
                    RestRequest request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddParameter("application/json", (object)str16, ParameterType.RequestBody);
                    string str17 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                    produtoRet = JsonConvert.DeserializeObject<ProdutoRet>(str17, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    if (!produtoRet.success)
                    {
                        RoboService.JsonEvioEmresa = str16;
                        RoboService.JsonRetornoEmresa = str17;
                    }
                    if (produtoRet == null)
                    {
                        produtoRet = new ProdutoRet();
                        produtoRet.success = false;
                        produtoRet.message = "Erro ao Cadastrar Produto";
                        RoboService.JsonEvioProduto = str16;
                        RoboService.JsonRetornoProduto = str17;
                    }
                    businessObject.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return produtoRet;
        }

        private GrupoProdutoRet IntegraGrupoProduto(string ItemCode, string Token)
        {
            Recordset businessObject = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            GrupoProdutoRet grupoProdutoRet = new GrupoProdutoRet();
            RoboService.JsonEvioGrupoProduto = string.Empty;
            RoboService.JsonRetornoGrupoProduto = string.Empty;
            try
            {
                string QueryStr = string.Format(Resources.ConsultaGrupoItem, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)ItemCode);
                businessObject.DoQuery(QueryStr);
                while (!businessObject.EoF)
                {
                    string str1 = businessObject.Fields.Item((object)"codigo").Value.ToString();
                    string str2 = businessObject.Fields.Item((object)"nome").Value.ToString();
                    string str3 = JsonConvert.SerializeObject((object)new GrupoProduto()
                    {
                        token = Token,
                        codigo = str1,
                        nome = str2
                    }, Formatting.Indented, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "produto_grupos");
                    RestRequest request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddParameter("application/json", (object)str3, ParameterType.RequestBody);
                    string str4 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                    grupoProdutoRet = JsonConvert.DeserializeObject<GrupoProdutoRet>(str4, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    if (!grupoProdutoRet.success)
                    {
                        RoboService.JsonEvioEmresa = str3;
                        RoboService.JsonRetornoEmresa = str4;
                    }
                    if (grupoProdutoRet == null)
                    {
                        grupoProdutoRet = new GrupoProdutoRet();
                        grupoProdutoRet.success = false;
                        grupoProdutoRet.message = "Erro ao Cadastrar Produto";
                        RoboService.JsonEvioGrupoProduto = str3;
                        RoboService.JsonRetornoGrupoProduto = str4;
                    }
                    businessObject.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return grupoProdutoRet;
        }

        private EmpresaRet IntegraEmresa(string Token, string CardCode, int propriedade)
        {
            Recordset businessObject = (Recordset)SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            EmpresaRet empresaRet = new EmpresaRet();
            RoboService.JsonEvioEmresa = string.Empty;
            RoboService.JsonRetornoEmresa = string.Empty;
            try
            {
                string QueryStr = string.Format(Resources.ConsultaEmpresa, (object)ConfigurationManager.AppSettings["CompanyDB"].ToString(), (object)CardCode);
                businessObject.DoQuery(QueryStr);
                while (!businessObject.EoF)
                {
                    string str1 = businessObject.Fields.Item((object)"xnome").Value.ToString();
                    string str2 = businessObject.Fields.Item((object)"xfant").Value.ToString();
                    string str3 = businessObject.Fields.Item((object)"cnpj_cpf").Value.ToString();
                    string str4 = businessObject.Fields.Item((object)"ie").Value.ToString();
                    string str5 = businessObject.Fields.Item((object)"email").Value.ToString();
                    string str6 = businessObject.Fields.Item((object)"fone").Value.ToString();
                    string str7 = JsonConvert.SerializeObject((object)new Empresa()
                    {
                        token = Token,
                        xnome = str1,
                        xfant = str2,
                        cnpj_cpf = str3.Replace("/", "").Replace("-", "").Replace(".", ""),
                        ie = str4.Replace("/", "").Replace("-", "").Replace(".", ""),
                        email = str5,
                        fone = str6,
                        propriedade = propriedade
                    }, Formatting.Indented, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "empresa");
                    RestRequest request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddParameter("application/json", (object)str7, ParameterType.RequestBody);
                    string str8 = restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", "");
                    empresaRet = JsonConvert.DeserializeObject<EmpresaRet>(str8, new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                    if (!empresaRet.success)
                    {
                        RoboService.JsonEvioEmresa = str7;
                        RoboService.JsonRetornoEmresa = str8;
                    }
                    if (empresaRet == null)
                    {
                        empresaRet = new EmpresaRet();
                        empresaRet.success = false;
                        empresaRet.message = "Erro ao Cadastrar Empresa";
                        RoboService.JsonEvioEmresa = str7;
                        RoboService.JsonRetornoEmresa = str8;
                    }
                    businessObject.MoveNext();
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject((object)businessObject);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return empresaRet;
        }

        private LoginRet Login()
        {
            LoginRet loginRet = new LoginRet();
            try
            {
                string str = JsonConvert.SerializeObject((object)new Integração.WMS.Modelos.Login()
                {
                    client_id = ConfigurationManager.AppSettings["Token"].ToString(),
                    email = ConfigurationManager.AppSettings["Email"].ToString(),
                    senha = ConfigurationManager.AppSettings["Senha"].ToString()
                }, Formatting.Indented, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                RestClient restClient = new RestClient(ConfigurationManager.AppSettings["WebService"].ToString() + "auth");
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", (object)str, ParameterType.RequestBody);
                loginRet = JsonConvert.DeserializeObject<LoginRet>(restClient.Execute((IRestRequest)request).Content.Replace(",\"data\":[]", ""), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                if (loginRet == null)
                {
                    loginRet = new LoginRet();
                    loginRet.success = false;
                    loginRet.message = "Erro ao Fazer o Login";
                }
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
            return loginRet;
        }

        private void CriarTabelaseCampos()
        {
            try
            {
                string[,] valoresValidos = new string[3, 2]
                {
          {
            "1",
            "Em Processamento"
          },
          {
            "2",
            "Processado"
          },
          {
            "3",
            "Erro"
          }
                };
                if (!SAP.ExisteTB("WMS_INTEGRACAO"))
                    SAP.AddTabela("@WMS_INTEGRACAO", "WMS- Dados de Integração", BoUTBTableType.bott_NoObject);
                SAP.AddCampos("@WMS_INTEGRACAO", "Tipo", "Tipo", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, (short)10, (string[,])null, (string)null, (string)null);
                SAP.AddCampos("@WMS_INTEGRACAO", "Codigo", "Codigo", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, (short)50, (string[,])null, (string)null, (string)null);
                SAP.AddCampos("@WMS_INTEGRACAO", "Status", "Status", BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, (short)10, valoresValidos, (string)null, (string)null);
                SAP.AddCampos("@WMS_INTEGRACAO", "Mensagem", "Mensagem", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, (short)10, (string[,])null, (string)null, (string)null);
                SAP.AddCampos("@WMS_INTEGRACAO", "JsonEnvio", "Json Envio", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, (short)10, (string[,])null, (string)null, (string)null);
                SAP.AddCampos("@WMS_INTEGRACAO", "JsonRetorno", "Json Retorno", BoFieldTypes.db_Memo, BoFldSubTypes.st_None, (short)10, (string[,])null, (string)null, (string)null);
            }
            catch (Exception ex)
            {
                RoboService.log.Error((object)ex.Message);
            }
        }
    }
}
