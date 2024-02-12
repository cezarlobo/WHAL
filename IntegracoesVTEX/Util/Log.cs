using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using SAPbobsCOM;

namespace IntegracoesVETX.Util
{
	public class Log
	{
		private static string _filePathCliente = ConfigurationManager.AppSettings["pathLogCliente"];

		private static string _filePathEstoque = ConfigurationManager.AppSettings["pathLogEstoque"];

		private static string _filePathPedido = ConfigurationManager.AppSettings["pathLogPedido"];

		public static void WriteLogOld(string message)
		{
			try
			{
				using (StreamWriter sw = File.AppendText(_filePathCliente))
				{
					sw.WriteLine(DateTime.Now.ToString() + ": " + message);
					sw.Flush();
					sw.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void WriteLogCliente(string message)
		{
			try
			{
				using (StreamWriter sw = File.AppendText(_filePathCliente))
				{
					sw.WriteLine(DateTime.Now.ToString() + ": " + message);
					sw.Flush();
					sw.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void WriteLogEstoque(string message)
		{
			try
			{
				using (StreamWriter sw = File.AppendText(_filePathEstoque))
				{
					sw.WriteLine(DateTime.Now.ToString() + ": " + message);
					sw.Flush();
					sw.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void WriteLogPedido(string message)
		{
			try
			{
				using (StreamWriter sw = File.AppendText(_filePathPedido))
				{
					sw.WriteLine(DateTime.Now.ToString() + ": " + message);
					sw.Flush();
					sw.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void WriteLogTable(Company company, string tipoDocumento, string numVtex, string numSAP, string status, string mensagem)
		{
			try
			{
				if (company.Connected)
				{
					UserTable userTable = company.UserTables.Item("LOG_INTEGRACAO_VTEX");
					userTable.UserFields.Fields.Item("U_data").Value = DateTime.Now.ToString();
					userTable.UserFields.Fields.Item("U_tipoDocumento").Value = tipoDocumento;
					userTable.UserFields.Fields.Item("U_numVtex").Value = numVtex;
					userTable.UserFields.Fields.Item("U_numSAP").Value = numSAP;
					userTable.UserFields.Fields.Item("U_status").Value = status;
					userTable.UserFields.Fields.Item("U_mensagem").Value = mensagem;
					userTable.Add();
					Marshal.ReleaseComObject(userTable);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
