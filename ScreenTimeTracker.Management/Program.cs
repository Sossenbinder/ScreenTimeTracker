using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ScreenTimeTracker.Management.Services;
using ScreenTimeTracker.Management.Services.Interface;
using ScreenTimeTracker.Management.Utils;

namespace ScreenTimeTracker.Management
{
	public class Program
	{
		private const string ClientUrl =
#if DEBUG
			"http://localhost:7071/"
#else
			"https://screentimetracker.azurewebsites.net"
#endif
			;

		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			var services = builder.Services;
			services
				.AddSingleton<IAuthService, AuthService>()
				.AddSingleton<IAuthRefreshService, AuthRefreshService>()
				.AddSingleton<AuthenticationStateProvider, AuthProvider>()
				.AddAuthorizationCore();

			services.AddHttpClient(HttpClients.ScreenTimeTracker, client =>
			{
				client.BaseAddress = new Uri(ClientUrl);
			});

			services.AddScoped<HttpClient>();

			// This is so dumb, but without this the Release config doesn't work
			_ = new JwtHeader();
			_ = new JwtPayload();

			await builder.Build().RunAsync();
		}
	}
}