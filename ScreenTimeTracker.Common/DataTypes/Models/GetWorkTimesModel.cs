using System;

namespace ScreenTimeTracker.Common.DataTypes.Models
{
	public class GetWorkTimesModel
	{
		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }
	}

	public static class GetWorkTimesModelExtensions
	{
		public static void Validate(this GetWorkTimesModel model)
		{
			if (model.StartDate == null && model.EndDate == null)
			{
				throw new ArgumentException("At least one of start or end date has to be set");
			}
		}
	}
}