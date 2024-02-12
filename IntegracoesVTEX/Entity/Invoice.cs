using System.Collections.Generic;

namespace IntegracoesVETX.Entity
{
	internal class Invoice
	{
		public string type { get; set; }

		public string issuanceDate { get; set; }

		public string invoiceNumber { get; set; }

		public string invoiceValue { get; set; }

		public string invoiceKey { get; set; }

		public string invoiceUrl { get; set; }

		public string courier { get; set; }

		public string trackingNumber { get; set; }

		public string trackingUrl { get; set; }

		public List<ItemNF> items { get; set; }

		internal Invoice()
		{
			type = "Output";
		}
	}
}
