namespace IntegracoesVETX.Entity
{
	public class Shippingdata
	{
		public string id { get; set; }

		public Address address { get; set; }

		public Logisticsinfo[] logisticsInfo { get; set; }

		public Selectedaddress[] selectedAddresses { get; set; }
	}
}
