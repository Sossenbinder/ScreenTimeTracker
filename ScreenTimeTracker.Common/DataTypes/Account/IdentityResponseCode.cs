namespace ScreenTimeTracker.Common.DataTypes.Account
{
	public enum IdentityResponseCodeRaw
	{
		Success = 0,
		UnknownError = 1,
		PasswordsDontMatch = 2,
		IncompleteData = 3,
	}

	public class IdentityResponseCode : EnumClass
	{
		public static IdentityResponseCode Success = new IdentityResponseCode((int)IdentityResponseCodeRaw.Success, nameof(Success));

		public static IdentityResponseCode UnknownError = new IdentityResponseCode((int)IdentityResponseCodeRaw.UnknownError, nameof(UnknownError));

		public static IdentityResponseCode PasswordsDontMatch = new IdentityResponseCode((int)IdentityResponseCodeRaw.PasswordsDontMatch, nameof(PasswordsDontMatch));

		public static IdentityResponseCode IncompleteData = new IdentityResponseCode((int)IdentityResponseCodeRaw.IncompleteData, nameof(IncompleteData));

		public IdentityResponseCode(int code, string? name = null) : base(code, name)
		{
		}
	}
}