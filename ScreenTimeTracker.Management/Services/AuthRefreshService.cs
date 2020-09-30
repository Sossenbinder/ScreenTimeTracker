using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using ScreenTimeTracker.Common.DataTypes;
using ScreenTimeTracker.Common.DataTypes.Results;
using ScreenTimeTracker.Common.Extensions;
using ScreenTimeTracker.Common.Utils;
using ScreenTimeTracker.Management.Extensions;
using ScreenTimeTracker.Management.Services.Interface;
using ScreenTimeTracker.Management.Utils;

namespace ScreenTimeTracker.Management.Services
{
	public class AuthRefreshService : IAuthRefreshService
	{
		private readonly IAuthService _authService;

		private readonly HttpClient _httpClient;

		private readonly IJSRuntime _jsRuntime;

		private CancellationTokenSource _cancellationTokenSource;

		public AuthRefreshService(
			IAuthService authService,
			IHttpClientFactory httpClientFactory,
			IJSRuntime jsRuntime)
		{
			_authService = authService;
			_httpClient = httpClientFactory.CreateClient(HttpClients.ScreenTimeTracker);
			_jsRuntime = jsRuntime;

			_cancellationTokenSource = new CancellationTokenSource();

			authService.OnUserLogin += StartRefreshCycle;
		}

		public void StartRefreshCycle()
		{
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = _cancellationTokenSource.Token;

			var _ = Task.Factory.StartNew(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
					await TryRefreshToken(cancellationToken);
				}
			}, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public async Task TryRefreshToken(CancellationToken? cancellationToken = null)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, FunctionMap.Get(Function.RefreshJwtToken));
			request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
			var response = await _httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);

			string jwtToken = null!;

			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();

				if (!responseString.IsNullOrEmpty())
				{
					var refreshTokenResult = JsonSerializer.Deserialize<RefreshTokenResult>(responseString);

					jwtToken = refreshTokenResult.Token;
				}
			}

			_authService.JwtToken = jwtToken;
		}
	}
}