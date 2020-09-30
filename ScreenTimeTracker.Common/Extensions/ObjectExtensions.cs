using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ScreenTimeTracker.Common.Extensions
{
	public static class ObjectExtensions
	{
		public static string AsJson(this object obj) => JsonConvert.SerializeObject(obj);
	}
}