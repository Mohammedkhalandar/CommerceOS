using CommerceOS.Application.DTOs.Auth;
using CommerceOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommerceOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ==========================================
    // REGISTER
    // ==========================================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }

    // ==========================================
    // LOGIN
    // ==========================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }

    // ==========================================
    // CURRENT USER
    // ==========================================

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            id,
            name,
            email,
            role
        });
    }

    // ==========================================
    // ADMIN TEST
    // ==========================================

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok(new
        {
            message = "Welcome Admin! You have access to this endpoint.",
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}