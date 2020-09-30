using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ScreenTimeTracker.Management.Extensions
{
	public static class IJSRuntimeExtensions
	{
		public static ValueTask<string?> GetCookie(this IJSRuntime jsRuntime, string cookieName, CancellationToken? cancellationToken = null)
		{
			return jsRuntime.InvokeAsync<string?>("blazorExtensions.GetCookie", cancellationToken ?? CancellationToken.None, new object[] { cookieName });
		}

		public static ValueTask<string> SetCookie(this IJSRuntime jsRuntime, string name, string value, int expirationMinutes, CancellationToken? cancellationToken = null)
		{
			return jsRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", cancellationToken ?? CancellationToken.None, new object[] { name, value, expirationMinutes });
		}
	}
}