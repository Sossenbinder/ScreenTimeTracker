using ScreenTimeTracker.Events.Interface;

namespace ScreenTimeTracker.Events
{
	public class ActivityEvents : IActivityEvents
	{
		public AsyncEvent<ActivityEventArgs> OnActivityStateChanged { get; } = new AsyncEvent<ActivityEventArgs>();
	}
}