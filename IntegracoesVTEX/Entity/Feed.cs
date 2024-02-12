using System;

namespace IntegracoesVETX.Entity
{
	public class Feed
	{
		public string eventId { get; set; }

		public string handle { get; set; }

		public string domain { get; set; }

		public string state { get; set; }

		public string orderId { get; set; }

		public DateTime lastChange { get; set; }
	}
}
