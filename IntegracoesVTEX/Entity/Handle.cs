namespace IntegracoesVETX.Entity
{
	internal class Handle
	{
		private string[] handle;

		public string[] insertHandle(string _handle)
		{
			handle = new string[1] { _handle };
			return handle;
		}
	}
}
