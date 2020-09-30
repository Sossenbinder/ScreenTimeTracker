using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ScreenTimeTracker.Utils
{
	public static class NativeMethods
	{
		private struct LastInputInfo
		{
			public uint CbSize;
			public uint DwTime;
		}

		[DllImport("user32.dll")]
		private static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);

		public static TimeSpan GetTimeSinceLastInput()
		{
			var lastInputInfo = new LastInputInfo();
			lastInputInfo.CbSize = (uint)Marshal.SizeOf(lastInputInfo);

			if (GetLastInputInfo(ref lastInputInfo))
			{
				return TimeSpan.FromMilliseconds(Environment.TickCount64 - lastInputInfo.DwTime);
			}
			else
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
	}
}