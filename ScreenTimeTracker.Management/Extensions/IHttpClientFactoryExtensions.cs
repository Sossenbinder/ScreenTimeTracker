using System.Net.Http;
using ScreenTimeTracker.Management.Utils;

namespace ScreenTimeTracker.Management.Extensions
{
	public static class IHttpClientFactoryExtensions
	{
		public static HttpClient GetDefaultClient(this IHttpClientFactory clientFactory)
		{
			return clientFactory.CreateClient(HttpClients.ScreenTimeTracker);
		}
	}
}