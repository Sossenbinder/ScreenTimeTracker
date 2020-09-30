using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ScreenTimeTracker.Common.Entities
{
	public class RefreshTokenEntity : TableEntity
	{
		public Guid RefreshToken { get; set; }

		public string UserName { get; set; }
	}
}