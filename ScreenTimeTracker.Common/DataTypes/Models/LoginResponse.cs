using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.Entities;

namespace ScreenTimeTracker.Common.DataTypes.Models
{
	public class LoginResponse
	{
		public IdentityResponseCode ResponseCode { get; }

		public AccountEntity? Account { get; set; }

		public LoginResponse(IdentityResponseCode responseCode)
		{
			ResponseCode = responseCode;
		}
	}
}