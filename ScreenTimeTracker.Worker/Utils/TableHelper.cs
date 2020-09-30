using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker.Utils
{
	internal class TableHelper : ITableHelper
	{
		public Table TableType { get; }

		private readonly string _tableName;

		private readonly CloudTableClient _cloudTableClient;
		private readonly ILogger<ITableHelper> _logger;

		public TableHelper(
			IConfiguration configuration,
			CloudTableClient cloudTableClient,
			Table tableType,
			ILogger<ITableHelper> logger)
		{
			TableType = tableType;
			_tableName = configuration[$"{tableType}TableName"];
			_cloudTableClient = cloudTableClient;
			_logger = logger;
		}

		public async Task<CloudTable> GetTable()
		{
			_logger.LogInformation($"Table name: {_tableName}");
			var table = _cloudTableClient.GetTableReference(_tableName);

			await table.CreateIfNotExistsAsync();

			return table;
		}
	}
}