using Auth01.Application.Interfaces;
using Auth01.Application.DTOs;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Auth01.Infrastructure.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly IConfiguration _configuration;

        public GoogleService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleUserDto> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["GoogleAuth:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                
                return new GoogleUserDto
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    PictureUrl = payload.Picture,
                    Subject = payload.Subject
                };
            }
            catch (Exception ex)
            {
                // Log exception
                return null;
            }
        }
    }
}
