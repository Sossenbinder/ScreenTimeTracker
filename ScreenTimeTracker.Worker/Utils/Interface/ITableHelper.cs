using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Worker.DataTypes;

namespace ScreenTimeTracker.Worker.Utils.Interface
{
	internal interface ITableHelper
	{
		Table TableType { get; }

		Task<CloudTable> GetTable();
	}
}