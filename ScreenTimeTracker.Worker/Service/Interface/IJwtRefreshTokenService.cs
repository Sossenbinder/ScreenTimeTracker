using System;
using System.Threading.Tasks;
using ScreenTimeTracker.Common.Entities;

namespace ScreenTimeTracker.Worker.Service.Interface
{
	public interface IJwtRefreshTokenService
	{
		Task<Guid> GetAndInsertFreshToken(AccountEntity account);

		Task<(Guid NewRefreshToken, string NewJwtToken)> GetNewTokenSet(Guid refreshToken, DateTime expirationDate);
	}
}