using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Common.Entities;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Extensions;
using ScreenTimeTracker.Worker.Service.Interface;
using ScreenTimeTracker.Worker.Utils;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker.Service
{
	internal class JwtRefreshTokenService : IJwtRefreshTokenService
	{
		private readonly IJwtService _jwtService;

		private readonly IAccountService _accountService;

		private readonly ITableHelper _tableHelper;

		public JwtRefreshTokenService(
			TableHelperRegistry tableHelperRegistry,
			IAccountService accountService,
			IJwtService jwtService)
		{
			_jwtService = jwtService;
			_accountService = accountService;
			_tableHelper = tableHelperRegistry.TableHelpers[Table.RefreshTokens];
		}

		public async Task<Guid> GetAndInsertFreshToken(AccountEntity account)
		{
			var refreshToken = Guid.NewGuid();
			var partitionKey = AccountUtils.GetPartitionIndexForName(account.Name!).ToString();

			var entity = new RefreshTokenEntity()
			{
				RowKey = account.Name,
				PartitionKey = partitionKey,
				RefreshToken = refreshToken,
				UserName = account.Name!,
			};

			var insertOperation = TableOperation.InsertOrReplace(entity);

			var table = await _tableHelper.GetTable();
			await table.ExecuteAsync(insertOperation);

			return refreshToken;
		}

		public async Task<(Guid NewRefreshToken, string NewJwtToken)> GetNewTokenSet(Guid refreshToken, DateTime expirationDate)
		{
			var tokenInformation = await GetRefreshTokenEntity(refreshToken);

			var account = (await _accountService.GetAccount(tokenInformation.UserName))!;

			var newJwtToken = _jwtService.GenerateTokenForAccount(account, expirationDate);

			var newRefreshToken = await GetAndInsertFreshToken(account);

			return (newRefreshToken, newJwtToken);
		}

		private async Task<RefreshTokenEntity> GetRefreshTokenEntity(Guid refreshToken)
		{
			var workTimeTable = await _tableHelper.GetTable();

			var tableQuery = new TableQuery<RefreshTokenEntity>()
				.Where(TableQuery.GenerateFilterConditionForGuid(nameof(RefreshTokenEntity.RefreshToken), QueryComparisons.Equal, refreshToken));

			var entities = await workTimeTable.ExecuteQueryFull(tableQuery);

			return entities.Single();
		}
	}
}