using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IntegracoesVETX.Util
{
	public static class JsonUtil
	{
		public static string ConvertToJsonString(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
		}
	}
}
