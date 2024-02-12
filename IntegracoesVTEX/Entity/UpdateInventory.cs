namespace IntegracoesVETX.Entity
{
	internal class UpdateInventory
	{
		public bool unlimitedQuantity { get; set; }

		public string dateUtcOnBalanceSystem { get; set; }

		public int quantity { get; set; }

		internal UpdateInventory()
		{
			unlimitedQuantity = false;
			dateUtcOnBalanceSystem = null;
		}
	}
}
