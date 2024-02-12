namespace IntegracoesVETX.Entity
{
	public class Storepreferencesdata
	{
		public string countryCode { get; set; }

		public string currencyCode { get; set; }

		public Currencyformatinfo currencyFormatInfo { get; set; }

		public int currencyLocale { get; set; }

		public string currencySymbol { get; set; }

		public string timeZone { get; set; }
	}
}
