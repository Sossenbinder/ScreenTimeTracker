using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScreenTimeTracker.Events;
using ScreenTimeTracker.Events.Interface;
using ScreenTimeTracker.Services;
using ScreenTimeTracker.Services.Interface;

namespace ScreenTimeTracker
{
	internal class Program
	{
		private static async Task Main(string[] args)
		{
			var configuration = ConfigBuilder.ValidateAndBuildConfiguration();

			var serviceProvider = new ServiceCollection()
				.AddSingleton(configuration)
				.AddSingleton<TrackingService>()
				.AddSingleton<IDataProvider, UserInputTimeDataProvider>()
				.AddSingleton<IActivityEvents, ActivityEvents>()
				.AddSingleton<IEventSender, EventSender>()
				.AddSingleton<ITrayIconService, TrayIconService>()
				.AddHttpClient<IEventSender, EventSender>(client =>
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["Token"]);
#if DEBUG
					client.BaseAddress = new Uri("http://localhost:7071/");
#else
					client.BaseAddress = new Uri("https://screentimetracker.azurewebsites.net");
#endif
				})
				.Services
				.BuildServiceProvider();

			serviceProvider.GetService<ITrayIconService>().InitTrayIcon();

			serviceProvider.GetService<IEventSender>();
			using var trackingService = serviceProvider.GetService<TrackingService>();

			trackingService.StartListening();

			await Task.Delay(-1);
		}
	}
}