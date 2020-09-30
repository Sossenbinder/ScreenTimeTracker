using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScreenTimeTracker.Management.Extensions
{
	public static class HttpResponseMessageExtensions
	{
		public static async Task<T> GetContentDeserialized<T>(this HttpResponseMessage httpResponseMessage)
		{
			var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

			var clientJwtTokenResponse = JsonConvert.DeserializeObject<T>(responseContent);

			return clientJwtTokenResponse;
		}
	}
}