namespace IntegracoesVETX.Entity
{
	public class Selectedaddress
	{
		public string addressId { get; set; }

		public string addressType { get; set; }

		public string receiverName { get; set; }

		public string street { get; set; }

		public string number { get; set; }

		public string complement { get; set; }

		public string neighborhood { get; set; }

		public string postalCode { get; set; }

		public string city { get; set; }

		public string state { get; set; }

		public string country { get; set; }

		public float[] geoCoordinates { get; set; }
	}
}
