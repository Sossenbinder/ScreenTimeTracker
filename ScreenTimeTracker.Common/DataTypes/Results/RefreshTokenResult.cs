using Newtonsoft.Json;

namespace ScreenTimeTracker.Common.DataTypes.Results
{
	public class RefreshTokenResult
	{
		[JsonProperty("Token")]
		public string Token { get; set; }
	}
}