using System;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.Entities;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface IJwtService
	{
		string GenerateTokenForAccount(AccountEntity account, DateTime? expirationDate = null);

		AuthInfo DecodeAndGetUserFromToken(string token);
	}
}