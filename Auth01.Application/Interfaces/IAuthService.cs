using Auth01.Application.DTOs;

namespace Auth01.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> RegisterAsync(RegisterDto model);
        Task<TokenResponse> LoginAsync(LoginDto model);
        Task<TokenResponse> GoogleLoginAsync(GoogleLoginDto model);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model, string ipAddress);
        Task RevokeTokenAsync(RefreshTokenRequest model, string ipAddress);
        Task LogoutAsync(string userId, string ipAddress);
    }
}
