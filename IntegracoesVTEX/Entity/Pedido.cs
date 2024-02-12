using System;

namespace IntegracoesVETX.Entity
{
	public class Pedido
	{
		public string orderId { get; set; }

		public string sequence { get; set; }

		public string marketplaceOrderId { get; set; }

		public string marketplaceServicesEndpoint { get; set; }

		public string sellerOrderId { get; set; }

		public string origin { get; set; }

		public string affiliateId { get; set; }

		public string salesChannel { get; set; }

		public object merchantName { get; set; }

		public string status { get; set; }

		public string statusDescription { get; set; }

		public double value { get; set; }

		public DateTime creationDate { get; set; }

		public DateTime lastChange { get; set; }

		public string orderGroup { get; set; }

		public Total[] totals { get; set; }

		public ItemVtex[] items { get; set; }

		public Clientprofiledata clientProfileData { get; set; }

		public Shippingdata shippingData { get; set; }

		public Paymentdata paymentData { get; set; }

		public Seller[] sellers { get; set; }

		public string followUpEmail { get; set; }

		public string hostname { get; set; }

		public int roundingError { get; set; }

		public string orderFormId { get; set; }

		public bool isCompleted { get; set; }

		public Storepreferencesdata storePreferencesData { get; set; }

		public bool allowCancellation { get; set; }

		public bool allowEdition { get; set; }

		public bool isCheckedIn { get; set; }

		public Marketplace marketplace { get; set; }

		public string authorizedDate { get; set; }

		public Itemmetadata itemMetadata { get; set; }
	}
}
