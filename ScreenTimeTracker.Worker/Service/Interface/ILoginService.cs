using System.Threading.Tasks;
using ScreenTimeTracker.Common.DataTypes.Models;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface ILoginService
	{
		Task<LoginResponse> LoginAccount(LoginModel loginModel);
	}
}