using System;

namespace ScreenTimeTracker.Common.DataTypes.Models
{
	public class RegistrationModel
	{
		public string? Name { get; set; }

		public string? Password { get; set; }

		public string? ConfirmPassword { get; set; }
	}

	public static class RegistrationModelExtensions
	{
		public static void Validate(this RegistrationModel registrationModel)
		{
			if (registrationModel.Name == null
				|| registrationModel.Password == null
				|| registrationModel.ConfirmPassword == null)
			{
				throw new ArgumentException();
			}
		}
	}
}