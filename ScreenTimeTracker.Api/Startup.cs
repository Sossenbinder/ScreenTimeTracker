using System.Collections.Generic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScreenTimeTracker.Api;
using ScreenTimeTracker.Worker;
using Serilog;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ScreenTimeTracker.Api
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			var azureServiceTokenProvider = new AzureServiceTokenProvider();
			var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.AddUserSecrets(typeof(Startup).Assembly)
				.AddAzureKeyVault("https://screentimetracker.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager())
				.AddInMemoryCollection(new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>("AccountsTableName", "Accounts"),
					new KeyValuePair<string, string>("WorkTimesTableName", "WorkTimes"),
					new KeyValuePair<string, string>("RefreshTokensTableName", "RefreshTokens")
				})
				.Build();

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();

			builder.Services.AddSingleton<IConfiguration>(configuration);
			builder.Services.AddLogging(x => x.AddSerilog());

			WorkerModule.RegisterServices(builder.Services);
		}
	}
}