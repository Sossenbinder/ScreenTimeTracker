using System;

namespace ScreenTimeTracker.Common.DataTypes.Models
{
	public class LogTimeModel
	{
		public bool IsActive { get; set; }

		public DateTime? Timestamp { get; set; }
	}

	public static class LogTimeModelExtensions
	{
		public static void Validate(this LogTimeModel model)
		{
			if (model.Timestamp == null)
			{
				throw new ArgumentException();
			}
		}
	}
}