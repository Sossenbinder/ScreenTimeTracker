using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Api.Extensions;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.DataTypes.Models;
using ScreenTimeTracker.Common.DataTypes.Results;
using ScreenTimeTracker.Worker.Service.Interface;

namespace ScreenTimeTracker.Api
{
	public class Api
	{
		private readonly IRegistrationService _registrationService;

		private readonly ILoginService _loginService;

		private readonly IJwtService _jwtService;

		private readonly IWorkTimesService _workTimesService;

		private readonly IAccountService _accountService;

		private readonly IJwtRefreshTokenService _jwtRefreshTokenService;

		public Api(
			IRegistrationService registrationService,
			ILoginService loginService,
			IJwtService jwtService,
			IWorkTimesService workTimesService,
			IAccountService accountService,
			IJwtRefreshTokenService jwtRefreshTokenService)
		{
			_registrationService = registrationService;
			_loginService = loginService;
			_jwtService = jwtService;
			_workTimesService = workTimesService;
			_accountService = accountService;
			_jwtRefreshTokenService = jwtRefreshTokenService;
		}

		[FunctionName(nameof(Ping))]
		public IActionResult Ping(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(Ping))]
			HttpRequest request)
		{
			return new OkObjectResult("Pong");
		}

		[FunctionName(nameof(GetWorkTimes))]
		public async Task<IActionResult> GetWorkTimes(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = nameof(GetWorkTimes))]
			HttpRequest request)
		{
			var authInfo = request.CheckAuthorization(_jwtService);
			if (!authInfo.Authenticated)
			{
				return ReturnUnauthorizedResult(authInfo);
			}

			var getWorkTimesModel = await request.BodyAsJson<GetWorkTimesModel>();
			getWorkTimesModel.Validate();

			var workTimesResult = new List<WorkTimes>();
			await foreach (var result in _workTimesService.GetWorkTimes(authInfo.UserName!, getWorkTimesModel))
			{
				workTimesResult.Add(result);
			}

			return new JsonResult(workTimesResult)
			{
				StatusCode = (int)HttpStatusCode.OK,
			};
		}

		[FunctionName(nameof(LogTime))]
		public async Task<IActionResult> LogTime(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = nameof(LogTime))]
			HttpRequest request,
			ILogger logger)
		{
			var authInfo = request.CheckAuthorization(_jwtService);
			if (!authInfo.Authenticated)
			{
				return ReturnUnauthorizedResult(authInfo);
			}

			var logTimeModel = await request.BodyAsJson<LogTimeModel>();
			logTimeModel.Validate();

			var userName = authInfo.UserName!;
			if (logTimeModel.IsActive)
			{
				await _workTimesService.LogStart(userName, logTimeModel);
			}
			else
			{
				await _workTimesService.LogEnd(userName, logTimeModel);
			}

			return new OkResult();
		}

		[FunctionName(nameof(Register))]
		public async Task<IActionResult> Register(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = nameof(Register))]
			HttpRequest request)
		{
			var registrationModel = await request.BodyAsJson<RegistrationModel>();

			var result = await _registrationService.Register(registrationModel);

			return new JsonResult(new RegisterResult
			{
				StatusCode = result.Code
			})
			{
				StatusCode = GetStatusCode(result)
			};
		}

		[FunctionName(nameof(GetClientJwtToken))]
		public async Task<IActionResult> GetClientJwtToken(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(GetClientJwtToken))]
			HttpRequest request)
		{
			var authInfo = request.CheckAuthorization(_jwtService);
			if (!authInfo.Authenticated)
			{
				return ReturnUnauthorizedResult(authInfo);
			}

			var account = await _accountService.GetAccount(authInfo.UserName!);

			var token = _jwtService.GenerateTokenForAccount(account!);

			return new OkObjectResult(new GetTokenResult()
			{
				ClientJwtToken = token,
			});
		}

		[FunctionName(nameof(RefreshJwtToken))]
		public async Task<IActionResult> RefreshJwtToken(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = nameof(RefreshJwtToken))]
			HttpRequest request)
		{
			var response = request.HttpContext.Response;

			var refreshToken = request.Cookies["refreshToken"];

			if (refreshToken == null)
			{
				return new OkResult();
			}

			var expirationDate = DateTime.UtcNow.AddMinutes(15);
			var (newRefreshToken, newJwtToken) = await _jwtRefreshTokenService.GetNewTokenSet(Guid.Parse(refreshToken), expirationDate);
			AppendRefreshTokenCookie(response, newRefreshToken);

			return new OkObjectResult(new RefreshTokenResult()
			{
				Token = newJwtToken,
			});
		}

		[FunctionName(nameof(Login))]
		public async Task<IActionResult> Login(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = nameof(Login))]
			HttpRequest request)
		{
			var response = request.HttpContext.Response;

			var loginModel = await request.BodyAsJson<LoginModel>();

			var result = await _loginService.LoginAccount(loginModel);

			if (result.ResponseCode != IdentityResponseCode.Success)
			{
				return new JsonResult(new LoginResult()
				{
					StatusCode = result.ResponseCode.Code
				})
				{
					StatusCode = GetStatusCode(result.ResponseCode)
				};
			}

			var expirationDate = DateTime.UtcNow.AddMinutes(15);
			var jwtToken = _jwtService.GenerateTokenForAccount(result.Account!, expirationDate);

			var refreshToken = await _jwtRefreshTokenService.GetAndInsertFreshToken(result.Account!);
			AppendRefreshTokenCookie(response, refreshToken);

			return new OkObjectResult(new LoginResult()
			{
				StatusCode = result.ResponseCode.Code,
				Token = jwtToken
			});
		}

		private static IActionResult ReturnUnauthorizedResult(AuthInfo authInfo)
		{
			var result = authInfo.AuthFailure.HasValue
				? new
				{
					authFailureCode = authInfo.AuthFailure,
					authFailureMessage = authInfo.AuthFailure.ToString()
				}
				: null;

			return new ObjectResult(result)
			{
				StatusCode = (int)HttpStatusCode.Unauthorized,
			};
		}

		private static int GetStatusCode(IdentityResponseCode responseCode)
		{
			return responseCode.Code switch
			{
				(int)IdentityResponseCodeRaw.Success => (int)HttpStatusCode.OK,
				(int)IdentityResponseCodeRaw.UnknownError => (int)HttpStatusCode.InternalServerError,
				_ => (int)HttpStatusCode.BadRequest,
			};
		}

		private static void AppendRefreshTokenCookie(HttpResponse response, Guid refreshToken)
		{
			var expiration = DateTime.UtcNow.AddYears(1);

			response.Cookies.Append("refreshToken", refreshToken.ToString(), new CookieOptions()
			{
				Expires = DateTime.UtcNow.AddYears(1),
				MaxAge = expiration - DateTime.UtcNow,
#if RELEASE
				SameSite = SameSiteMode.None,
				Secure = true,
#endif
			});
		}
	}
}