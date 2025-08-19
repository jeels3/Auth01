using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Application.Services;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Infrastructure.Data;

namespace TradesCompany_AW.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager <ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public string GenerateAccessToken(ApplicationUser user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);

            if (principal == null)
                throw new SecurityTokenException("Invalid access token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new SecurityTokenException("User not found");
            }

            var storedRefreshToken =  user.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefault();
            
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
                throw new SecurityTokenException("Invalid refresh token");

            // Revoke the used refresh token
            storedRefreshToken.RevokedAt = DateTime.Now;
            storedRefreshToken.RevokedByIp = ipAddress;

            // Generate new tokens
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles[0];
            var newAccessToken = GenerateAccessToken(user, role);
            var newRefreshToken = GenerateRefreshToken();

            // Save new refresh token
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now,
                CreatedByIp = ipAddress,
                UserId = user.Id
            });

            // Clean up old refresh tokens
            user.RefreshTokens.RemoveAll(rt => !rt.IsActive);

            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]))
            };
        }

        public async Task RevokeTokenAsync(string refreshToken, string ipAddress)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user == null)
                throw new SecurityTokenException("Invalid refresh token");

            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (token == null || !token.IsActive)
                throw new SecurityTokenException("Invalid refresh token");

            token.RevokedAt = DateTime.Now;
            token.RevokedByIp = ipAddress;

            await _context.SaveChangesAsync();
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // We don't validate lifetime here since we want to refresh expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
