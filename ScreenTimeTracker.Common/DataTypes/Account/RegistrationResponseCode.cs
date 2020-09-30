using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenTimeTracker.Common.DataTypes.Account
{
	public class RegistrationResponseCode : IdentityResponseCode
	{
		public static RegistrationResponseCode AlreadyRegistered = new RegistrationResponseCode(3, nameof(AlreadyRegistered));

		public RegistrationResponseCode(int code, string? name = null)
			: base(code, name)
		{
		}
	}
}