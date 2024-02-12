using System;

namespace IntegracoesVETX.Entity
{
	public class Endereco
	{
		public string addressName { get; set; }

		public string addressType { get; set; }

		public string city { get; set; }

		public string complement { get; set; }

		public string country { get; set; }

		public object countryfake { get; set; }

		public float[] geoCoordinate { get; set; }

		public string neighborhood { get; set; }

		public string number { get; set; }

		public string postalCode { get; set; }

		public string receiverName { get; set; }

		public object reference { get; set; }

		public string state { get; set; }

		public string street { get; set; }

		public string userId { get; set; }

		public string id { get; set; }

		public string accountId { get; set; }

		public string accountName { get; set; }

		public string dataEntityId { get; set; }

		public string createdBy { get; set; }

		public DateTime createdIn { get; set; }

		public object updatedBy { get; set; }

		public object updatedIn { get; set; }

		public string lastInteractionBy { get; set; }

		public DateTime lastInteractionIn { get; set; }

		public object[] followers { get; set; }

		public object[] tags { get; set; }

		public object auto_filter { get; set; }
	}
}
