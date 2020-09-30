using System.Threading.Tasks;
using ScreenTimeTracker.Common.DataTypes.Account;
using ScreenTimeTracker.Common.DataTypes.Models;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface IRegistrationService
	{
		Task<IdentityResponseCode> Register(RegistrationModel registrationModel);
	}
}