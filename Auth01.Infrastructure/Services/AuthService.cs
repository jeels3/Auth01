using Auth01.Application.DTOs;
using Auth01.Application.Interfaces;
using Auth01.Application.Services;
using Auth01.Domain.Entities;
using Auth01.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth01.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IGoogleService _googleService;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, ApplicationDbContext context, IGoogleService googleService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _googleService = googleService;
        }

        public async Task<TokenResponse> RegisterAsync(RegisterDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, model.Role.ToUpper());

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<TokenResponse> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new Exception("Invalid email or password");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<TokenResponse> GoogleLoginAsync(GoogleLoginDto model)
        {
            var googleUser = await _googleService.VerifyGoogleTokenAsync(model.IdToken);
            if (googleUser == null)
                throw new Exception("Invalid Google Token");

            var user = await _userManager.FindByEmailAsync(googleUser.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = googleUser.Email,
                    UserName = googleUser.Email,
                    FirstName = googleUser.FirstName,
                    LastName = googleUser.LastName,
                    ProfilePictureUrl = googleUser.PictureUrl,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception("Could not create user from Google data");
                
                // Assign default role, e.g., "User"
                await _userManager.AddToRoleAsync(user, "USER");
            }
            
            // Link Google Login provider if not already linked (Optional but implementing login logic here is sufficient for now)
            
            return await GenerateAuthResponseAsync(user);
        }

        private async Task<TokenResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";
            var accessToken = _tokenService.GenerateAccessToken(user, role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now,
                UserId = user.Id
            };

            user.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15)
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest model, string ipAddress)
        {
            return await _tokenService.RefreshTokenAsync(model.AccessToken, model.RefreshToken, ipAddress);
        }

        public async Task RevokeTokenAsync(RefreshTokenRequest model, string ipAddress)
        {
            await _tokenService.RevokeTokenAsync(model.RefreshToken, ipAddress);
        }

        public async Task LogoutAsync(string userId, string ipAddress)
        {
             var user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                foreach (var token in user.RefreshTokens.Where(rt => rt.IsActive))
                {
                    token.RevokedAt = DateTime.Now;
                    token.RevokedByIp = ipAddress;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
