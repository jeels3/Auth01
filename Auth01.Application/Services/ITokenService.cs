using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth01.Application.DTOs;
using Auth01.Domain.Entities;

namespace Auth01.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, string role);
        string GenerateRefreshToken();
        Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress);
        Task RevokeTokenAsync(string refreshToken, string ipAddress);
    }
}
