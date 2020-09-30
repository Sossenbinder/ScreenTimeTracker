using System;
using System.Security.Cryptography;
using System.Text;

namespace ScreenTimeTracker.Worker.Utils
{
	public static class PasswordHasher
	{
		public static string HashPassword(string clearTextPassword)
		{
			using var sha256 = SHA256.Create();

			var password = Encoding.UTF8.GetBytes(clearTextPassword);
			return Convert.ToBase64String(sha256.ComputeHash(password));
		}
	}
}