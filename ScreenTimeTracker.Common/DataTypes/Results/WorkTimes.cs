using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenTimeTracker.Common.DataTypes.Results
{
	public class WorkTimeSpan
	{
		public DateTime Start { get; set; }

		public DateTime End { get; set; }
	}

	public class WorkTimes
	{
		public DateTime Date { get; set; }

		public double TotalWorkMinutes { get; set; }

		public List<WorkTimeSpan> WorkTimeSets { get; set; }
	}
}