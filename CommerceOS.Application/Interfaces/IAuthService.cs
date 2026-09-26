using CommerceOS.Application.DTOs.Auth;

namespace CommerceOS.Application.Interfaces;

public interface IAuthService
{
    Task<object> RegisterAsync(RegisterRequest request);

    Task<object> LoginAsync(LoginRequest request);
}