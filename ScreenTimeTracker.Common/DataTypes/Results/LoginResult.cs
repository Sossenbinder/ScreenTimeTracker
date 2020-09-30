using Newtonsoft.Json;

namespace ScreenTimeTracker.Common.DataTypes.Results
{
	public class LoginResult
	{
		[JsonProperty("StatusCode")]
		public int StatusCode { get; set; }

		[JsonProperty("Token")]
		public string? Token { get; set; }
	}
}