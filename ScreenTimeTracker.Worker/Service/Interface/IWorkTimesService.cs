using System.Collections.Generic;
using System.Threading.Tasks;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Common.DataTypes.Results;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface IWorkTimesService
	{
		Task LogStart(string userName, LogTimeModel logTimeModel);

		Task LogEnd(string userName, LogTimeModel logTimeModel);

		IAsyncEnumerable<WorkTimes> GetWorkTimes(string userName, GetWorkTimesModel getWorkTimesModel);
	}
}