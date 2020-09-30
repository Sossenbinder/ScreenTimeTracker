using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace ScreenTimeTracker.Worker.Extensions
{
	public static class CloudTableExtensions
	{
		public static async Task<List<T>> ExecuteQueryFull<T>(
			this CloudTable cloudTable,
			TableQuery<T> query)
			where T : ITableEntity, new()
		{
			List<T> queriedItems = new List<T>();

			TableContinuationToken? continuationToken = null;

			do
			{
				var result = await cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);

				continuationToken = result.ContinuationToken;

				queriedItems.AddRange(result.Results);
			} while (continuationToken != null);

			return queriedItems;
		}
	}
}