using DashboardApi.Models;

namespace DashboardApi.Services;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
