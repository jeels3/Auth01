using Auth01.Application.DTOs;

namespace Auth01.Application.Interfaces
{
    public interface IGoogleService
    {
        Task<GoogleUserDto> VerifyGoogleTokenAsync(string idToken);
    }
}
