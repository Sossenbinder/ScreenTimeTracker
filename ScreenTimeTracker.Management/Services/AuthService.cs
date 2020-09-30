using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ScreenTimeTracker.Management.Services.Interface;

namespace ScreenTimeTracker.Management.Services
{
	public class AuthService : IAuthService
	{
		public event Action OnUserLogin;

		private string? _jwtToken;

		public string? JwtToken
		{
			get => _jwtToken;
			set
			{
				_jwtToken = value;
				OnUserLogin();
			}
		}

		public IEnumerable<Claim> GetClaims()
		{
			if (_jwtToken == null)
			{
				return new List<Claim>();
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.ReadJwtToken(JwtToken);

			return token.Claims;
		}
	}
}