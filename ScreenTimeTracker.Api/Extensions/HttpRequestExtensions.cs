using System;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.Extensions;
using ScreenTimeTracker.Worker.Service.Interface;

namespace ScreenTimeTracker.Api.Extensions
{
	internal static class HttpRequestExtensions
	{
		public static async Task<T> BodyAsJson<T>(this HttpRequest request)
		{
			var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
			return JsonConvert.DeserializeObject<T>(requestBody);
		}

		public static Task<dynamic> BodyAsJson(this HttpRequest request)
			=> request.BodyAsJson<dynamic>();

		public static AuthInfo CheckAuthorizationOrFail(this HttpRequest request, IJwtService jwtService)
		{
			var authInfo = CheckAuthorization(request, jwtService);

			if (!authInfo.Authenticated)
			{
				throw new AuthenticationException("Request not authenticated");
			}

			return authInfo;
		}

		public static AuthInfo CheckAuthorization(this HttpRequest request, IJwtService jwtService)
		{
			if (!request.Headers.ContainsKey("Authorization"))
			{
				return new AuthInfo(false, AuthFailure.NoToken);
			}

			string authHeader = request.Headers["Authorization"];

			if (authHeader.IsNullOrEmpty())
			{
				return new AuthInfo(false, AuthFailure.NoToken);
			}

			try
			{
				if (authHeader.StartsWith("Bearer"))
				{
					authHeader = authHeader.Substring("Bearer".Length + 1);
				}
			}
			catch (Exception)
			{
				return new AuthInfo(false, AuthFailure.NoToken);
			}

			return jwtService.DecodeAndGetUserFromToken(authHeader);
		}
	}
}