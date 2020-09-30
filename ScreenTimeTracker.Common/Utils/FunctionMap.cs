using System;
using System.Collections.Generic;
using System.Linq;
using ScreenTimeTracker.Common.DataTypes;

namespace ScreenTimeTracker.Common.Utils
{
	public static class FunctionMap
	{
		private static readonly Dictionary<Function, string> _functionMap;

		static FunctionMap()
		{
			_functionMap = Enum
				.GetValues(typeof(Function))
				.Cast<Function>()
				.ToDictionary(x => x, x => $"api/{x}");
		}

		public static string Get(Function func) => _functionMap[func];
	}
}