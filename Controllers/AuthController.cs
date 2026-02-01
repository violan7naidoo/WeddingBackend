using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OurBigDay.Api.DTOs;
using OurBigDay.Api.Entities;
using OurBigDay.Api.Services;

namespace OurBigDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _config = config;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        return Ok(new AuthResponse(token, user.Email ?? "", roles.FirstOrDefault() ?? "Family", user.DisplayName ?? user.UserName ?? ""));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var role = request.Role ?? "Family";
        if (new[] { "Admin", "Family", "Guest" }.Contains(role))
            await _userManager.AddToRoleAsync(user, role);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        return Ok(new AuthResponse(token, user.Email ?? "", roles.FirstOrDefault() ?? "Family", user.DisplayName ?? ""));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Me(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        return Ok(new AuthResponse(token, user.Email ?? "", roles.FirstOrDefault() ?? "Family", user.DisplayName ?? ""));
    }
}
