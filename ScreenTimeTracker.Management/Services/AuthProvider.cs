using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using ScreenTimeTracker.Management.Services.Interface;

namespace ScreenTimeTracker.Management.Services
{
	public class AuthProvider : AuthenticationStateProvider
	{
		private readonly IAuthService _authService;

		public AuthProvider(IAuthService authService)
		{
			_authService = authService;

			_authService.OnUserLogin += () => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var claims = _authService.GetClaims().ToList();

			var identity = claims.Any() ? new ClaimsIdentity(claims, "UserInfo") : new ClaimsIdentity();

			var user = new ClaimsPrincipal(identity);

			return Task.FromResult(new AuthenticationState(user));
		}
	}
}