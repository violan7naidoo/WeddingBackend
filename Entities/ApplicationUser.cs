using Microsoft.AspNetCore.Identity;

namespace OurBigDay.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
