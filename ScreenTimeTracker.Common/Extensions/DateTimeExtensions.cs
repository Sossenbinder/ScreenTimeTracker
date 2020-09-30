using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenTimeTracker.Common.Extensions
{
	public static class DateTimeExtensions
	{
		public static string ToStorageSafeShortDateString(this DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd");
		}
	}
}