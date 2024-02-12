namespace IntegracoesVETX.Business
{
	internal class OrderFiltered
	{
		public List[] list { get; set; }

		public object[] facets { get; set; }

		public Paging paging { get; set; }

		internal OrderFiltered()
		{
		}
	}
}
