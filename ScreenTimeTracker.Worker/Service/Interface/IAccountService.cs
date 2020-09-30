using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Common.Entities;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface IAccountService
	{
		Task<AccountEntity?> GetAccount(string userName, Action<TableQuery<AccountEntity>>? whereEnricherFunction = null);

		Task<AccountEntity?> GetAccountForUserNameAndPassword(string userName, string passwordHash);

		Task<TableResult> CreateAccount(string userName, string password);

		Task<TableResult> UpdateAccount(AccountEntity account);
	}
}