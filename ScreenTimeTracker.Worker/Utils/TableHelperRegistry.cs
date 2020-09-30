using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker.Utils
{
	internal class TableHelperRegistry
	{
		public Dictionary<Table, ITableHelper> TableHelpers;

		public TableHelperRegistry(IEnumerable<ITableHelper> tableHelpers)
		{
			TableHelpers = tableHelpers
				.ToDictionary(x => x.TableType, x => x);
		}
	}
}