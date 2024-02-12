namespace IntegracoesVETX.Entity
{
	public class Payment
	{
		public string id { get; set; }

		public string paymentSystem { get; set; }

		public string paymentSystemName { get; set; }

		public int value { get; set; }

		public int installments { get; set; }

		public int referenceValue { get; set; }

		public string firstDigits { get; set; }

		public string lastDigits { get; set; }

		public string group { get; set; }

		public string tid { get; set; }

		public Connectorresponses connectorResponses { get; set; }
	}
}
