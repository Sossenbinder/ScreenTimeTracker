using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using ScreenTimeTracker.Events.Interface;

namespace ScreenTimeTracker.Services
{
	internal enum DisplayState
	{
		Default,
		AnyOn,
		AllOff,
	}

	// Experimental
	internal class WindowsDisplayPortDataProvider : AbstractDataProvider
	{
		private readonly EventHandler _displaySettingsChangedEvent;

		private DisplayState _currentState = DisplayState.Default;

		public WindowsDisplayPortDataProvider(IActivityEvents activityEvents)
			: base(activityEvents)
		{
			_displaySettingsChangedEvent = async (_, __) => await OnDisplaySettingsChanged();
		}

		public override void StartListening()
		{
			SystemEvents.DisplaySettingsChanged += _displaySettingsChangedEvent;
		}

		public override void StopListening()
		{
			SystemEvents.DisplaySettingsChanged -= _displaySettingsChangedEvent;
		}

		private async Task OnDisplaySettingsChanged()
		{
			var displays = WindowsDisplayAPI
				.Display
				.GetDisplays()
				.ToList();

			// Determine if we have either all displays off, or only some on
			var allDisplaysOff = !displays.Any() || displays.All(x => !x.IsAvailable);

			var displayState = allDisplaysOff ? DisplayState.AllOff : DisplayState.AnyOn;

			if (displayState != _currentState)
			{
				_currentState = displayState;
				Console.WriteLine($"Reporting new state - {displayState.ToString()}");
				await ActivityEvents.OnActivityStateChanged.Raise(new ActivityEventArgs(displayState == DisplayState.AnyOn, DateTime.Now));
			}
		}
	}
}