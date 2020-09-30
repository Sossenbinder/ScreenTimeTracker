using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Service;
using ScreenTimeTracker.Worker.Service.Interface;
using ScreenTimeTracker.Worker.Utils;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker
{
	public static class WorkerModule
	{
		public static void RegisterServices(IServiceCollection serviceCollection)
		{
			serviceCollection
				.AddSingleton<IRegistrationService, RegistrationService>()
				.AddSingleton<ILoginService, LoginService>()
				.AddSingleton<IJwtService, JwtService>()
				.AddSingleton<IJwtRefreshTokenService, JwtRefreshTokenService>()
				.AddSingleton<IWorkTimesService, WorkTimesService>()
				.AddSingleton<IAccountService, AccountService>()
				.AddSingleton<TableHelperRegistry>()
				.AddSingleton(ctx =>
					CloudStorageAccount.Parse(ctx.GetService<IConfiguration>()["CloudStorageAccountConnectionString"]))
				.AddSingleton(ctx =>
					ctx.GetService<CloudStorageAccount>().CreateCloudTableClient());

			foreach (var table in Enum.GetValues(typeof(Table)).Cast<Table>())
			{
				serviceCollection.AddSingleton<ITableHelper, TableHelper>(ctx => new TableHelper(
					ctx.GetService<IConfiguration>(),
					ctx.GetService<CloudTableClient>(),
					table,
					ctx.GetService<ILogger<ITableHelper>>()));
			}
		}
	}
}