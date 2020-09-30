namespace ScreenTimeTracker.Common.DataTypes.Account
{
	public class AuthInfo
	{
		public bool Authenticated { get; set; }

		public string? UserName { get; set; }

		public AuthFailure? AuthFailure { get; set; }

		public AuthInfo(bool authenticated) => Authenticated = authenticated;

		public AuthInfo(bool authenticated, AuthFailure? authFailure)
		{
			Authenticated = authenticated;
			AuthFailure = authFailure;
		}
	}
}