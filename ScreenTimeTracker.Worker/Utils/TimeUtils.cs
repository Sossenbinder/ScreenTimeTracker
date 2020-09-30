using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenTimeTracker.Worker.Utils
{
	public static class TimeUtils
	{
		private static readonly DateTime MinDate = new DateTime(2020, 10, 5);

		public static (DateTime Start, DateTime End) GetRequestedDates(DateTime? startDate, DateTime? endDate)
		{
			var start = startDate ?? MinDate;
			var end = endDate ?? DateTime.Today;
			return (start, end);
		}
	}
}