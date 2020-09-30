using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ScreenTimeTracker.Management.Services.Interface
{
	public interface IAuthService
	{
		event Action OnUserLogin;

		string? JwtToken { get; set; }

		IEnumerable<Claim> GetClaims();
	}
}