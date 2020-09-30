using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.Entities;
using ScreenTimeTracker.Worker.Service.Interface;

namespace ScreenTimeTracker.Worker.Service
{
	internal class JwtService : IJwtService
	{
		private const string NameKey = "UserName";

		private const string Expiration = "exp";

		private readonly string _jwtSecretToken;

		private readonly IJwtEncoder _jwtEncoder;

		private readonly IJwtDecoder _jwtDecoder;

		public JwtService(IConfiguration configuration)
		{
			_jwtSecretToken = configuration["JwtSecretToken"];

			var algorithm = new HMACSHA256Algorithm();
			var serializer = new JsonNetSerializer();
			var base64Encoder = new JwtBase64UrlEncoder();

			_jwtEncoder = new JwtEncoder(algorithm, serializer, base64Encoder);

			var dateTimeProvider = new UtcDateTimeProvider();
			var jwtValidator = new JwtValidator(serializer, dateTimeProvider);

			_jwtDecoder = new JwtDecoder(serializer, jwtValidator, base64Encoder, algorithm);
		}

		public string GenerateTokenForAccount(
			AccountEntity account,
			DateTime? expirationDate = null)
		{
			var claims = new Dictionary<string, string>
			{
				{ NameKey, account.Name! },
			};

			if (expirationDate != null)
			{
				claims.Add(Expiration, expirationDate.Value.Ticks.ToString());
			}

			return _jwtEncoder.Encode(claims, _jwtSecretToken);
		}

		public AuthInfo DecodeAndGetUserFromToken(string token)
		{
			try
			{
				var rawJsonToken = _jwtDecoder.Decode(token, _jwtSecretToken, true);
				var parsedRawJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJsonToken);

				var isValidToken = true;
				if (!parsedRawJson.ContainsKey(Expiration))
				{
					return new AuthInfo(isValidToken)
					{
						UserName = parsedRawJson["UserName"],
					};
				}

				var expirationDate = new DateTime(long.Parse(parsedRawJson[Expiration]));

				isValidToken = expirationDate - DateTime.UtcNow > TimeSpan.Zero;

				return new AuthInfo(isValidToken, isValidToken ? (AuthFailure?)null : AuthFailure.Expired)
				{
					UserName = parsedRawJson["UserName"],
				};
			}
			catch (Exception)
			{
				return new AuthInfo(false, AuthFailure.InvalidToken);
			}
		}
	}
}