using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IntegracoesVETX.Service
{
	internal class BaseService
	{
		private static string accountName = ConfigurationManager.AppSettings["accountName"];

		private static string environment = ConfigurationManager.AppSettings["environment"];

		private static string appKey = ConfigurationManager.AppSettings["appKey"];

		private static string appToken = ConfigurationManager.AppSettings["appToken"];

		public static HttpClient BuildClient()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			string baseUri = "https://" + accountName + "." + environment + ".com.br/";
			HttpClient val = new HttpClient
			{
				BaseAddress = new Uri(baseUri)
			};
			val.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appKey", appKey);
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appToken", appToken);
			((HttpHeaders)val.DefaultRequestHeaders).Add("Accept", "application/vnd.vtex.ds.v10+json");
			((HttpHeaders)val.DefaultRequestHeaders).Add("REST-Range", "resources=0-700");
			((HttpHeaders)val.DefaultRequestHeaders).Add("REST-Accept-Ranges", "resources");
			return val;
		}

		public static HttpClient BuildClientPedido()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Expected O, but got Unknown
			string baseUriPedido = "https://" + accountName + "." + environment + ".com.br/";
			HttpClient val = new HttpClient
			{
				BaseAddress = new Uri(baseUriPedido)
			};
			val.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appKey", appKey);
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appToken", appToken);
			((HttpHeaders)val.DefaultRequestHeaders).Add("Accept", "application/vnd.vtex.ds.v10+json");
			((HttpHeaders)val.DefaultRequestHeaders).Add("REST-Range", "resources=0-500");
			((HttpHeaders)val.DefaultRequestHeaders).Add("REST-Accept-Ranges", "resources");
			return val;
		}

		public static HttpClient BuildClientLogistics()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			string baseUri = "https://logistics." + environment + ".com.br/";
			HttpClient val = new HttpClient
			{
				BaseAddress = new Uri(baseUri)
			};
			val.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appKey", appKey);
			((HttpHeaders)val.DefaultRequestHeaders).Add("x-vtex-api-appToken", appToken);
			((HttpHeaders)val.DefaultRequestHeaders).Add("Accept", "application/vnd.vtex.ds.v10+json");
			((HttpHeaders)val.DefaultRequestHeaders).Add("REST-Range", "resources=0-10");
			return val;
		}
	}
}
