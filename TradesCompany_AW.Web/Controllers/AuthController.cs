using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Security.Claims;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Application.Services;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Infrastructure.Data;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest("User with this email already exists");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true // Set to false if you want email confirmation
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

             await _userManager.AddToRoleAsync(user, model.Role.ToUpper());

            if(model.Role.ToUpper() == "EMPLOYEE")
            {
                // Add to serviceMan
                var ServiceTypeId = model.ServiceTypeId;
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid email or password");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now,
                CreatedByIp = GetIpAddress(),
                UserId = user.Id
            };
            user.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var tokenResponse = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15) // Adjust based on your JWT settings
            };

            return Ok(tokenResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            try
            {
                var tokenResponse = await _tokenService.RefreshTokenAsync(model.AccessToken, model.RefreshToken, GetIpAddress());
                return Ok(tokenResponse);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest model)
        {
            try
            {
                await _tokenService.RevokeTokenAsync(model.RefreshToken, GetIpAddress());
                return Ok(new { message = "Token revoked successfully" });
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    // Revoke all active refresh tokens for this user
                    foreach (var token in user.RefreshTokens.Where(rt => rt.IsActive))
                    {
                        token.RevokedAt = DateTime.Now;
                        token.RevokedByIp = GetIpAddress();
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { message = "Logged out successfully" });
        }

        private string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
