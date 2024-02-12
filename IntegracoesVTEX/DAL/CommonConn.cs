using System;
using System.Configuration;
using SAPbobsCOM;

namespace IntegracoesVETX.DAL
{
	public class CommonConn
	{
		public static Company oCompany;

		public static Company InitializeCompany()
		{
			try
			{
				oCompany = new CompanyClass
				{
					Server = ConfigurationManager.AppSettings["Server"],
					language = BoSuppLangs.ln_English,
					UseTrusted = false,
					DbServerType = BoDataServerTypes.dst_MSSQL2016,
					CompanyDB = ConfigurationManager.AppSettings["DataBase"],
					UserName = ConfigurationManager.AppSettings["SapUser"],
					Password = ConfigurationManager.AppSettings["SapPassword"],
					DbUserName = ConfigurationManager.AppSettings["DbUser"],
					DbPassword = ConfigurationManager.AppSettings["DbPassword"]
				};
				if (oCompany.Connected)
				{
					oCompany.Disconnect();
				}
				if (oCompany.Connect() != 0L)
				{
					int temp_int = 0;
					string temp_string = "";
					oCompany.GetLastError(out temp_int, out temp_string);
					return oCompany;
				}
				return oCompany;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void FinalizeCompany()
		{
			if (oCompany.Connected)
			{
				oCompany.Disconnect();
			}
		}
	}
}
