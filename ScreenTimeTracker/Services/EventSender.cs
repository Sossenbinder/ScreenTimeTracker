using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using ScreenTimeTracker.Common.DataTypes;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Common.Utils;
using ScreenTimeTracker.Events.Interface;
using ScreenTimeTracker.Services.Interface;

namespace ScreenTimeTracker.Services
{
	internal class EventSender : IEventSender
	{
		private readonly HttpClient _httpClient;

		private readonly AsyncRetryPolicy _retryPolicy;

		private CancellationTokenSource _retryCancellationTokenSource;

		private readonly Context _retryContext;

		public EventSender(
			HttpClient httpClient,
			IActivityEvents activityEvents)
		{
			_httpClient = httpClient;
			activityEvents.OnActivityStateChanged.Register(OnActivityStateChanged);

			_retryPolicy = Policy
				.Handle<WebException>()
				.WaitAndRetryAsync(new List<TimeSpan>()
				{
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(15),
					TimeSpan.FromSeconds(60),
				});

			_retryCancellationTokenSource = new CancellationTokenSource();

			_retryContext = new Context("RetryContext")
			{
				{nameof(_retryCancellationTokenSource), _retryCancellationTokenSource}
			};
		}

		private async Task OnActivityStateChanged(ActivityEventArgs eventArgs)
		{
			_retryCancellationTokenSource.Cancel();
			_retryCancellationTokenSource = new CancellationTokenSource();

			var logTimeModel = new LogTimeModel()
			{
				IsActive = eventArgs.IsActive,
				Timestamp = eventArgs.TimeStamp,
			};

			var httpContent = new StringContent(
				JsonSerializer.Serialize(logTimeModel),
				Encoding.UTF8,
				"application/json");

			await _retryPolicy.ExecuteAsync(
				(ctx, ct) => _httpClient.PostAsync(FunctionMap.Get(Function.LogTime), httpContent, ct),
				_retryContext,
				_retryCancellationTokenSource.Token);
		}
	}
}