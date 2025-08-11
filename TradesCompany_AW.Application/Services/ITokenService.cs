using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress);
        Task RevokeTokenAsync(string refreshToken, string ipAddress);
    }
}
