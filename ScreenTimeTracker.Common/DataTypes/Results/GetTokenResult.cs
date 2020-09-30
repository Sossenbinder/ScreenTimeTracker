using Newtonsoft.Json;

namespace ScreenTimeTracker.Common.DataTypes.Results
{
	public class GetTokenResult
	{
		[JsonProperty("ClientJwtToken")]
		public string? ClientJwtToken { get; set; }
	}
}