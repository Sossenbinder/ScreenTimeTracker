using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Common.Extensions;

namespace ScreenTimeTracker.Worker.Extensions
{
	public static class TableQueryExtensions
	{
		public static TableQuery<T> AndWhereEquals<T>(this TableQuery<T> tableQuery, string toCompare, string compareObject)
			where T : ITableEntity, new()
			=> tableQuery.AndCondition(QueryComparisons.Equal, toCompare, compareObject);

		public static TableQuery<T> AndWhereGreaterEqual<T>(this TableQuery<T> tableQuery, string toCompare, string compareObject)
			where T : ITableEntity, new()
			=> tableQuery.AndCondition(QueryComparisons.GreaterThanOrEqual, toCompare, compareObject);

		public static TableQuery<T> AndWhereLessEqual<T>(this TableQuery<T> tableQuery, string toCompare,
			string compareObject)
			where T : ITableEntity, new()
			=> tableQuery.AndCondition(QueryComparisons.LessThanOrEqual, toCompare, compareObject);

		private static TableQuery<T> AndCondition<T>(this TableQuery<T> tableQuery, string comparison, string toCompare, string compareObject)
			where T : ITableEntity, new()
		{
			var filterCondition = TableQuery.GenerateFilterCondition(toCompare, comparison, compareObject);

			tableQuery.FilterString = tableQuery.FilterString.IsNullOrEmpty()
				? filterCondition
				: TableQuery.CombineFilters(tableQuery.FilterString, TableOperators.And, filterCondition);

			return tableQuery;
		}
	}
}