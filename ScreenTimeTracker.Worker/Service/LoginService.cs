using System;
using System.Threading.Tasks;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Worker.Service.Interface;

namespace ScreenTimeTracker.Worker.Service
{
	internal class LoginService : ILoginService
	{
		private readonly IAccountService _accountService;

		public LoginService(IAccountService accountService)
		{
			_accountService = accountService;
		}

		public async Task<LoginResponse> LoginAccount(LoginModel loginModel)
		{
			try
			{
				loginModel.Validate();
			}
			catch (ArgumentException)
			{
				return new LoginResponse(IdentityResponseCode.IncompleteData);
			}

			var account = await _accountService.GetAccountForUserNameAndPassword(loginModel.Name!, loginModel.Password!);

			if (account == null)
			{
				return new LoginResponse(LoginResponseCode.AccountNotFound);
			}

			return new LoginResponse(IdentityResponseCode.Success)
			{
				Account = account
			};
		}
	}
}