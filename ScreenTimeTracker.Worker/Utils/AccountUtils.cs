using System;
using ScreenTimeTracker.Common.Extensions;

namespace ScreenTimeTracker.Worker.Utils
{
	public static class AccountUtils
	{
		public static int GetPartitionIndexForName(string accountName)
		{
			if (accountName.IsNullOrEmpty())
			{
				throw new ArgumentException("Invalid name");
			}

			return GetPartitionIndexForString(accountName);
		}

		public static int GetPartitionIndexForRefreshToken(Guid refreshToken)
		{
			var tokenAsString = refreshToken.ToString();

			return GetPartitionIndexForString(tokenAsString);
		}

		private static int GetPartitionIndexForString(string valToExamine)
		{
			var firstChar = valToExamine[0];

			var partitionIndex = firstChar % 4;

			return partitionIndex;
		}
	}
}