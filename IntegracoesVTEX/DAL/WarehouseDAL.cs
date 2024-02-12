using System;
using System.Configuration;
using IntegracoesVETX.Util;
using SAPbobsCOM;

namespace IntegracoesVETX.DAL
{
	public class WarehouseDAL
	{
		private Company oCompany;

		private Log log;

		internal WarehouseDAL()
		{
			log = new Log();
		}

		public Recordset RecuperarSaldoEstoqueSAP(Company company)
		{
			string _query = string.Empty;
			string whsCode = ConfigurationManager.AppSettings["WhsCode"];
			oCompany = company;
			Recordset recordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
			try
			{
				if (company.Connected)
				{
					_query = string.Format("Select a.ItemCode, b.OnHand From OITM a inner join OITW b on a.ItemCode = b.ItemCode where invntItem = 'Y' and a.SellItem = 'Y' and a.frozenFor = 'N' and WhsCode = '{0}'", whsCode);
					recordSet.DoQuery(_query);
					if (recordSet.RecordCount > 0)
					{
						log.WriteLogEstoque("Foram encontrados " + recordSet.RecordCount + " Items no SAP.");
						return recordSet;
					}
				}
			}
			catch (Exception e)
			{
				log.WriteLogEstoque("Exception recuperarSaldoEstoqueSAP " + e.Message);
				throw;
			}
			return recordSet;
		}
	}
}
