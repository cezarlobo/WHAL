using System;

namespace IntegracoesVETX.Entity
{
	public class Orders
	{
		public int Id { get; set; }

		public string IdOrder { get; set; }

		public string IdOrderMarketplace { get; set; }

		public DateTime InsertedDate { get; set; }

		public string OrderStatus { get; set; }
	}
}
