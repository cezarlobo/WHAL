using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;
using IntegracoesVETX.Business;
using IntegracoesVETX.DAL;
using IntegracoesVETX.Util;
using SAPbobsCOM;

namespace IntegracoesVTEX
{
	public class Scheduler : ServiceBase
	{
		private Timer timerEstoque;

		private Timer timerPedidos;

		private Timer timerRetNF;

		private Timer timerCancelPedido;

		private Timer timerRetRastreamento;

		private string tempoDefinidoExecucaoRastreamento = ConfigurationManager.AppSettings["horaExecucaoEnvioCodRastreamento"];

		private string _path = ConfigurationManager.AppSettings["Path"];

		private bool jobIntegracaoEstoque = Convert.ToBoolean(ConfigurationManager.AppSettings["jobIntegracaoEstoque"]);

		private bool jobIntegracaoPedido = Convert.ToBoolean(ConfigurationManager.AppSettings["jobIntegracaoPedido"]);

		private bool jobIntegracaoRetNF = Convert.ToBoolean(ConfigurationManager.AppSettings["jobIntegracaoRetornoNF"]);

		private bool jobIntegracaoCancelPedido = Convert.ToBoolean(ConfigurationManager.AppSettings["jobIntegracaoCancelPedido"]);

		private bool jobRetornoRastreamento = Convert.ToBoolean(ConfigurationManager.AppSettings["jobRetornoCodigoRastreio"]);

		private Log log;

		private Company oCompany;

		private IContainer components;

		public Scheduler()
		{
			InitializeComponent();
			log = new Log();
			oCompany = CommonConn.InitializeCompany();
		}

		public void TesteWriter()
		{
			IntegracaoService integracaoService = new IntegracaoService();
			string horaAtual = string.Format("{0:t}", DateTime.Now);
			log.WriteLogPedido("Hora atual executada" + horaAtual);
			if (horaAtual == tempoDefinidoExecucaoRastreamento)
			{
				integracaoService.IniciarIntegracaoRetornoRastreamento(oCompany);
			}
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				log.WriteLogEstoque("#### Integração Inicializada.");
				log.WriteLogPedido("#### Integração Inicializada.");
				if (jobIntegracaoEstoque)
				{
					timerEstoque = new Timer();
					string intervaloExecucaoEstoque = ConfigurationManager.AppSettings["intervaloExecucaoEstoque"] + ",01";
					timerEstoque.Interval = TimeSpan.FromHours(Convert.ToDouble(intervaloExecucaoEstoque)).TotalMilliseconds;
					timerEstoque.Enabled = true;
					timerEstoque.Elapsed += IntegracaoEstoque;
				}
				if (jobIntegracaoPedido)
				{
					timerPedidos = new Timer();
					string intervaloExecucaoPedido = ConfigurationManager.AppSettings["intervaloExecucaoPedido"];
					timerPedidos.Interval = Convert.ToInt32(intervaloExecucaoPedido);
					timerPedidos.Enabled = true;
					timerPedidos.Elapsed += IntegracaoPedido;
				}
				if (jobIntegracaoRetNF)
				{
					timerRetNF = new Timer();
					string intervaloExecucaoRetNF = ConfigurationManager.AppSettings["intervaloExecucaoRetNF"];
					timerRetNF.Interval = Convert.ToInt32(intervaloExecucaoRetNF);
					timerRetNF.Enabled = true;
					timerRetNF.Elapsed += IntegracaoRetornoNF;
				}
				if (jobIntegracaoCancelPedido)
				{
					timerCancelPedido = new Timer();
					string intervaloExecucaoCancelPedido = ConfigurationManager.AppSettings["intervaloExecucaoCancelPedido"];
					timerCancelPedido.Interval = Convert.ToInt32(intervaloExecucaoCancelPedido);
					timerCancelPedido.Enabled = true;
					timerCancelPedido.Elapsed += IntegracaoCancelPedido;
				}
				if (jobRetornoRastreamento)
				{
					timerRetRastreamento = new Timer();
					timerRetRastreamento.Interval = TimeSpan.FromMinutes(1.0).TotalMilliseconds;
					timerRetRastreamento.Enabled = true;
					timerRetRastreamento.Elapsed += IntegracaoRetornoRastreamento;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void IntegracaoPedido(object sender, ElapsedEventArgs e)
		{
			try
			{
				timerPedidos.Enabled = false;
				timerPedidos.AutoReset = false;
				log.WriteLogPedido("#### INTEGRAÇÃO DE PEDIDOS INICIALIZADA");
				new IntegracaoService().IniciarIntegracaoPedido(oCompany);
				timerPedidos.Enabled = true;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			catch (Exception ex)
			{
				log.WriteLogPedido("Exception IntegracaoPedido " + ex.Message);
				throw;
			}
		}

		private void IntegracaoEstoque(object sender, ElapsedEventArgs e)
		{
			try
			{
				timerEstoque.Enabled = false;
				timerEstoque.AutoReset = false;
				log.WriteLogEstoque("#### INTEGRAÇÃO DE ESTOQUE INICIALIZADA");
				new IntegracaoService().IniciarIntegracaoEstoque(oCompany);
				timerEstoque.Enabled = true;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			catch (Exception ex)
			{
				log.WriteLogEstoque("Exception IntegracaoEstoque " + ex.Message);
				throw;
			}
		}

		private void IntegracaoRetornoNF(object sender, ElapsedEventArgs e)
		{
			try
			{
				timerRetNF.Enabled = false;
				timerRetNF.AutoReset = false;
				log.WriteLogPedido("#### INTEGRAÇÃO RETORNO NF INICIALIZADA");
				new IntegracaoService().RetornoNotaFiscal(oCompany);
				timerRetNF.Enabled = true;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			catch (Exception ex)
			{
				log.WriteLogPedido("Exception IntegracaoRetornoNF " + ex.Message);
				throw;
			}
		}

		private void IntegracaoCancelPedido(object sender, ElapsedEventArgs e)
		{
			try
			{
				timerCancelPedido.Enabled = false;
				timerCancelPedido.AutoReset = false;
				log.WriteLogPedido("#### INTEGRAÇÃO CANCELAMENTO DE PEDIDO INICIADA");
				new IntegracaoService().IniciarIntegracaoCancelamentoPedido(oCompany);
				timerCancelPedido.Enabled = true;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			catch (Exception ex)
			{
				log.WriteLogPedido("Exception IntegracaoCancelamento " + ex.Message);
				throw;
			}
		}

		private void IntegracaoRetornoRastreamento(object sender, ElapsedEventArgs e)
		{
			string horaAtual = string.Format("{0:t}", DateTime.Now);
			log.WriteLogPedido("Hora atual: " + horaAtual + " / TempoDefinido: " + tempoDefinidoExecucaoRastreamento);
			if (horaAtual == tempoDefinidoExecucaoRastreamento)
			{
				timerRetRastreamento.Enabled = false;
				timerRetRastreamento.AutoReset = false;
				log.WriteLogPedido("#### INTEGRAÇÃO RETORNO CÓD. RASTREIO INICIADA");
				new IntegracaoService().IniciarIntegracaoRetornoRastreamento(oCompany);
				timerRetRastreamento.Enabled = true;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
		}

		public void StartDebug()
		{
			OnStart(null);
		}
		protected override void OnStop()
		{
			timerEstoque.Stop();
			timerPedidos.Stop();
			timerRetNF.Stop();
			timerCancelPedido.Stop();
			timerRetRastreamento.Stop();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
			base.ServiceName = "Service1";
		}
	}
}
