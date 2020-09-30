using Microsoft.WindowsAzure.Storage.Table;

namespace ScreenTimeTracker.Common.Entities
{
	public class AccountEntity : TableEntity
	{
		public string? Name { get; set; }

		public string? HashedPassword { get; set; }
	}
}