using System;

namespace ScreenTimeTracker.Common.DataTypes.Models
{
	public class LoginModel
	{
		public string? Name { get; set; }

		public string? Password { get; set; }
	}

	public static class LoginModelExtensions
	{
		public static void Validate(this LoginModel loginModel)
		{
			if (loginModel.Name == null || loginModel.Password == null)
			{
				throw new ArgumentException();
			}
		}
	}
}