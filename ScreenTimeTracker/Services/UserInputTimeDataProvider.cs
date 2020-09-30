using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ScreenTimeTracker.Events.Interface;
using ScreenTimeTracker.Utils;

namespace ScreenTimeTracker.Services
{
	internal class LastActivityInfo
	{
		public bool IsActive { get; set; }
	}

	internal class UserInputTimeDataProvider : AbstractDataProvider
	{
		private readonly CancellationTokenSource _cancellationTokenSource;

		private readonly TimeSpan _checkInterval;

		private readonly TimeSpan _idleTimeoutInMinutes;

		private bool _isUserCurrentlyActive;

		public UserInputTimeDataProvider(
			IActivityEvents activityEvents,
			IConfiguration configuration)
			: base(activityEvents)
		{
			var checkIntervalInSeconds = configuration["checkIntervalInSeconds"];
			_checkInterval = checkIntervalInSeconds != null ? TimeSpan.FromSeconds(int.Parse(checkIntervalInSeconds)) : TimeSpan.FromSeconds(600);

			_cancellationTokenSource = new CancellationTokenSource();

			var idleTimeoutMinutes = configuration["idleTimeoutMinutes"];
			_idleTimeoutInMinutes = idleTimeoutMinutes != null ? TimeSpan.FromMinutes(int.Parse(idleTimeoutMinutes)) : TimeSpan.FromMinutes(30);

			_isUserCurrentlyActive = false;
		}

		public override void StartListening()
		{
			var cancellationToken = _cancellationTokenSource.Token;

			Task.Run(async () =>
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					await CheckLastInputTime();
					await Task.Delay(_checkInterval, cancellationToken);
				}
			}, cancellationToken)
				.ContinueWith(
					x => x.Exception!.Handle(exc => exc is TaskCanceledException),
					cancellationToken);
		}

		public override void StopListening()
		{
			_cancellationTokenSource.Cancel();
		}

		private async ValueTask CheckLastInputTime()
		{
			var timeSinceLastUserInput = NativeMethods.GetTimeSinceLastInput();

			if (_isUserCurrentlyActive && timeSinceLastUserInput >= _idleTimeoutInMinutes)
			{
				// In this case the user went from active to idle
				_isUserCurrentlyActive = false;
				await NotifyActivity(timeSinceLastUserInput);
			}
			else if (!_isUserCurrentlyActive && timeSinceLastUserInput < _idleTimeoutInMinutes)
			{
				// In this case the idle state was resolved as activity resumed
				_isUserCurrentlyActive = true;
				await NotifyActivity(timeSinceLastUserInput);
			}
		}

		private async Task NotifyActivity(TimeSpan timeSinceLastUserInput)
		{
			var timeStamp = DateTime.Now;

			if (!_isUserCurrentlyActive)
			{
				// If the user went idle, we need to subtract the time since last input in order to
				// calculate the timestamp the user stopped being active.

				timeStamp -= timeSinceLastUserInput;
			}

			var activityEvent = new ActivityEventArgs(_isUserCurrentlyActive, timeStamp);

			await ActivityEvents.OnActivityStateChanged.Raise(activityEvent);
		}
	}
}