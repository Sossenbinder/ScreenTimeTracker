namespace ScreenTimeTracker.Common.DataTypes.Account
{
	public class LoginResponseCode : IdentityResponseCode
	{
		public static LoginResponseCode AccountNotFound = new LoginResponseCode(4, nameof(AccountNotFound));

		public LoginResponseCode(int code, string? name = null)
			: base(code, name)
		{
		}
	}
}