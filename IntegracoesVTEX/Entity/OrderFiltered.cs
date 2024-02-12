namespace IntegracoesVETX.Entity
{
	public class OrderFiltered
	{
		public List[] list { get; set; }

		public object[] facets { get; set; }

		public Paging paging { get; set; }

		public Stats stats { get; set; }

		internal OrderFiltered()
		{
		}
	}
}
