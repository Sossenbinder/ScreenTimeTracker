using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace ScreenTimeTracker.Common.Entities
{
	public class SerializeableBaseEntity : TableEntity
	{
		protected void DeserializeList<T>(ref List<T> list, string value)
		{
			list = JsonConvert.DeserializeObject<List<T>>(value);
		}

		protected string SerializeList<T>(List<T> list)
		{
			return JsonConvert.SerializeObject(list);
		}
	}
}