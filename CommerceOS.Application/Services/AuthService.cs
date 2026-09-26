using CommerceOS.Application.DTOs.Auth;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommerceOS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<object> RegisterAsync(RegisterRequest request)
    {
        var existingUser =
            await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Role = "User",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive,
            user.CreatedAtUtc
        };
    }

    public async Task<object> LoginAsync(LoginRequest request)
    {
        var user =
            await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var result =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var token = GenerateJwtToken(user);

        return new
        {
            token,
            user = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role
            }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured.");

        var issuer =
            _configuration["Jwt:Issuer"]
            ?? "CommerceOS";

        var audience =
            _configuration["Jwt:Audience"]
            ?? "CommerceOS";

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.FullName),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}