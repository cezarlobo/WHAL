using System;
using System.Runtime.InteropServices;
using IntegracoesVETX.Util;
using SAPbobsCOM;

namespace IntegracoesVETX.DAL
{
	public class CountyDAL
	{
		private Company oCompany;

		public string RecuperarCodigoMunicipio(string municipio, Company company)
		{
			string _query = string.Empty;
			try
			{
				oCompany = company;
				if (oCompany.Connected)
				{
					Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
					if (municipio.Contains("'"))
					{
						municipio = municipio.Replace("'", "''");
					}
					_query = string.Format("SELECT OC.AbsId FROM OCNT OC WHERE OC.Name = '{0}'", municipio);
					recordSet.DoQuery(_query);
					if (recordSet.RecordCount > 0)
					{
						string result = recordSet.Fields.Item("AbsId").Value.ToString();
						Marshal.ReleaseComObject(recordSet);
						return result;
					}
					Marshal.ReleaseComObject(recordSet);
				}
			}
			catch (Exception e)
			{
				Log.WriteLogCliente("Exception RecuperarCodigoMunicipio " + e.Message);
				throw;
			}
			return null;
		}
	}
}
