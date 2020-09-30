using System.Threading;
using System.Threading.Tasks;

namespace ScreenTimeTracker.Management.Services.Interface
{
	public interface IAuthRefreshService
	{
		Task TryRefreshToken(CancellationToken? cancellationToken = null);
	}
}