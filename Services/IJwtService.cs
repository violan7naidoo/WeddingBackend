using OurBigDay.Api.Entities;

namespace OurBigDay.Api.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
