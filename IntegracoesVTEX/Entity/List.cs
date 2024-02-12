using System;

namespace IntegracoesVETX.Entity
{
	public class List
	{
		public string orderId { get; set; }

		public DateTime creationDate { get; set; }

		public string clientName { get; set; }

		public object items { get; set; }

		public float totalValue { get; set; }

		public string paymentNames { get; set; }

		public string status { get; set; }

		public string statusDescription { get; set; }

		public object marketPlaceOrderId { get; set; }

		public string sequence { get; set; }

		public string salesChannel { get; set; }

		public string affiliateId { get; set; }

		public string origin { get; set; }

		public bool workflowInErrorState { get; set; }

		public bool workflowInRetry { get; set; }

		public string lastMessageUnread { get; set; }

		public object ShippingEstimatedDate { get; set; }

		public object ShippingEstimatedDateMax { get; set; }

		public object ShippingEstimatedDateMin { get; set; }

		public bool orderIsComplete { get; set; }

		public object listId { get; set; }

		public object listType { get; set; }

		public object authorizedDate { get; set; }

		public object callCenterOperatorName { get; set; }

		public int totalItems { get; set; }

		public string currencyCode { get; set; }

		public string hostname { get; set; }

		public object invoiceOutput { get; set; }

		public object invoiceInput { get; set; }
	}
}
