using System.Collections.Generic;

namespace IntegracoesVETX.Entity
{
	public class ItemVtex
	{
		public string uniqueId { get; set; }

		public string id { get; set; }

		public string productId { get; set; }

		public string ean { get; set; }

		public string lockId { get; set; }

		public Itemattachment itemAttachment { get; set; }

		public int quantity { get; set; }

		public string seller { get; set; }

		public string name { get; set; }

		public string refId { get; set; }

		public double price { get; set; }

		public double listPrice { get; set; }

		public string imageUrl { get; set; }

		public string detailUrl { get; set; }

		public string sellerSku { get; set; }

		public int commission { get; set; }

		public int tax { get; set; }

		public Additionalinfo additionalInfo { get; set; }

		public string measurementUnit { get; set; }

		public double unitMultiplier { get; set; }

		public int sellingPrice { get; set; }

		public bool isGift { get; set; }

		public int rewardValue { get; set; }

		public int freightCommission { get; set; }

		public string taxCode { get; set; }

		public List<PriceTag> PriceTags { get; set; }
	}
}
