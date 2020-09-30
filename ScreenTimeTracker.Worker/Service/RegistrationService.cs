using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Common.Entities;
using ScreenTimeTracker.Worker.DataTypes;
using ScreenTimeTracker.Worker.Service.Interface;
using ScreenTimeTracker.Worker.Utils;
using ScreenTimeTracker.Worker.Utils.Interface;

namespace ScreenTimeTracker.Worker.Service
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly ITableHelper _accountTableHelper;

		private readonly IAccountService _accountService;

		public RegistrationService(
			TableHelperRegistry tableHelperRegistry,
			IAccountService accountService)
		{
			_accountService = accountService;
			_accountTableHelper = tableHelperRegistry.TableHelpers[Table.Accounts];
		}

		public async Task<IdentityResponseCode> Register(RegistrationModel registrationModel)
		{
			try
			{
				registrationModel.Validate();
			}
			catch (ArgumentException)
			{
				return IdentityResponseCode.IncompleteData;
			}

			if (registrationModel.Password != registrationModel.ConfirmPassword)
			{
				return IdentityResponseCode.PasswordsDontMatch;
			}

			var accountName = registrationModel.Name!;

			var account = await _accountService.GetAccount(accountName);

			if (account != null)
			{
				return RegistrationResponseCode.AlreadyRegistered;
			}

			var insertResult = await _accountService.CreateAccount(accountName, registrationModel.Password!);

			return insertResult.HttpStatusCode == (int)HttpStatusCode.NoContent
				? IdentityResponseCode.Success
				: IdentityResponseCode.UnknownError;
		}
	}
}