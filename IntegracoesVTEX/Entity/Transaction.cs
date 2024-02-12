namespace IntegracoesVETX.Entity
{
	public class Transaction
	{
		public bool isActive { get; set; }

		public string transactionId { get; set; }

		public string merchantName { get; set; }

		public Payment[] payments { get; set; }
	}
}
