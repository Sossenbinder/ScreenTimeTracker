using System;

namespace ScreenTimeTracker.Events.Interface
{
	public class ActivityEventArgs
	{
		public bool IsActive { get; }

		public DateTime TimeStamp { get; }

		public ActivityEventArgs(
			bool isActive,
			DateTime timeStamp)
		{
			IsActive = isActive;
			TimeStamp = timeStamp;
		}
	}

	internal interface IActivityEvents
	{
		AsyncEvent<ActivityEventArgs> OnActivityStateChanged { get; }
	}
}