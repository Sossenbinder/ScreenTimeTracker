using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using ScreenTimeTracker.Common.Extensions;
using ScreenTimeTracker.Events.Interface;
using ScreenTimeTracker.Services.Interface;

namespace ScreenTimeTracker.Services
{
	internal class TrayIconService : ITrayIconService
	{
		private readonly IActivityEvents _activityEvents;

		private NotifyIcon _icon;

		private bool _isContextMenuVisible;
		private ContextMenuStrip _contextMenuStrip;

		private ToolStripMenuItem _restartToggleItem;

		private RegistryKey _autoStartRegistryKey;
		private bool _autoStartActive;

		public TrayIconService(IActivityEvents activityEvents)
		{
			_activityEvents = activityEvents;
			_autoStartRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

			_autoStartActive = GetAutoStartState();
		}

		public void InitTrayIcon()
		{
			Task.Factory.StartNew(() =>
			{
				_contextMenuStrip = new ContextMenuStrip();
				var toggleItem = _contextMenuStrip.Items.Add("Autostart", null, (_, __) => OnRestartToggle());
				_restartToggleItem = (ToolStripMenuItem)toggleItem;
				_restartToggleItem.Checked = _autoStartActive;
				_contextMenuStrip.Items.Add("Exit", null, async (_, __) => await OnExitClick());

				_icon = new NotifyIcon
				{
					Icon = LocateTrayIcon(),
					Visible = true,
					ContextMenuStrip = _contextMenuStrip,
					Text = "ScreentimeTracker",
				};

				_icon.Click += OnNotifyIconClick;

				_icon.ShowBalloonTip(5000, "Info", "ScreentimeTracker running", ToolTipIcon.Info);

				Application.Run();
			}, TaskCreationOptions.LongRunning);
		}

		private void OnRestartToggle()
		{
			if (!_autoStartActive)
			{
				_autoStartRegistryKey.SetValue("ScreenTimeTracker", System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
			}
			else
			{
				_autoStartRegistryKey.DeleteValue("ScreenTimeTracker");
			}

			_autoStartActive = !_autoStartActive;
			_restartToggleItem.Checked = _autoStartActive;
		}

		private async Task OnExitClick()
		{
			await _activityEvents.OnActivityStateChanged.Raise(new ActivityEventArgs(false, DateTime.Now));
			Environment.Exit(0);
		}

		private bool GetAutoStartState()
		{
			var val = _autoStartRegistryKey.GetValue("ScreenTimeTracker");
			return !(val as string)?.IsNullOrEmpty() ?? false;
		}

		private void OnNotifyIconClick(object? sender, EventArgs e)
		{
			if (_isContextMenuVisible)
			{
				_contextMenuStrip.Hide();
			}
			else
			{
				_contextMenuStrip.Show();
			}

			_isContextMenuVisible = !_isContextMenuVisible;
		}

		private static Icon? LocateTrayIcon()
		{
			var assembly = typeof(Program).Assembly;

			var resourceNames = assembly.GetManifestResourceNames();
			var icoName = resourceNames.SingleOrDefault(x => x.Contains("sicon.ico"));

			if (icoName == null)
			{
				return null;
			}

			var resourceStream = assembly.GetManifestResourceStream(icoName);

			return resourceStream == null ? null : new Icon(resourceStream);
		}
	}
}