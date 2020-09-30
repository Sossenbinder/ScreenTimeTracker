using ScreenTimeTracker.Events.Interface;
using ScreenTimeTracker.Services.Interface;

namespace ScreenTimeTracker.Services
{
	internal abstract class AbstractDataProvider : IDataProvider
	{
		protected readonly IActivityEvents ActivityEvents;

		protected AbstractDataProvider(IActivityEvents activityEvents)
		{
			ActivityEvents = activityEvents;
		}

		public abstract void StartListening();

		public abstract void StopListening();
	}
}