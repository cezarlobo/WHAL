namespace IntegracoesVETX.Entity
{
	public class Logisticsinfo
	{
		public int itemIndex { get; set; }

		public string selectedSla { get; set; }

		public string lockTTL { get; set; }

		public double price { get; set; }

		public double listPrice { get; set; }

		public double sellingPrice { get; set; }

		public string deliveryCompany { get; set; }

		public string shippingEstimate { get; set; }

		public string shippingEstimateDate { get; set; }

		public Sla[] slas { get; set; }

		public string[] shipsTo { get; set; }

		public Deliveryid[] deliveryIds { get; set; }

		public string deliveryChannel { get; set; }

		public string addressId { get; set; }
	}
}
