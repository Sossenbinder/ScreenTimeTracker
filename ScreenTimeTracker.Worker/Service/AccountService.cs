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
	internal class AccountService : IAccountService
	{
		private readonly ITableHelper _accountTableHelper;

		public AccountService(TableHelperRegistry tableHelperRegistry)
		{
			_accountTableHelper = tableHelperRegistry.TableHelpers[Table.Accounts];
		}

		public async Task<AccountEntity?> GetAccount(
			string userName,
			Action<TableQuery<AccountEntity>>? whereEnricherFunction = null)
		{
			var accountTable = await _accountTableHelper.GetTable();

			var partitionForName = AccountUtils.GetPartitionIndexForName(userName).ToString();

			var query = new TableQuery<AccountEntity>()
				.AndWhereEquals(nameof(AccountEntity.PartitionKey), partitionForName)
				.AndWhereEquals(nameof(AccountEntity.RowKey), userName);

			whereEnricherFunction?.Invoke(query);

			var accounts = await accountTable.ExecuteQueryFull(query);

			return accounts.FirstOrDefault();
		}

		public Task<AccountEntity?> GetAccountForUserNameAndPassword(string userName, string passwordHash)
			=> GetAccount(
				userName,
				x => x.AndWhereEquals(nameof(AccountEntity.HashedPassword), PasswordHasher.HashPassword(passwordHash)));

		public async Task<TableResult> CreateAccount(string userName, string password)
		{
			var partitionForName = AccountUtils.GetPartitionIndexForName(userName).ToString();

			var insertOperation = TableOperation.Insert(new AccountEntity()
			{
				PartitionKey = partitionForName,
				RowKey = userName,
				Name = userName,
				HashedPassword = PasswordHasher.HashPassword(password!),
			});

			var accountTable = await _accountTableHelper.GetTable();

			return await accountTable.ExecuteAsync(insertOperation);
		}

		public async Task<TableResult> UpdateAccount(AccountEntity account)
		{
			var accountTable = await _accountTableHelper.GetTable();

			var insertOperation = TableOperation.Insert(account);

			return await accountTable.ExecuteAsync(insertOperation);
		}
	}
}