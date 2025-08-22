using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Web.ViewModel;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IGenericRepository<Address> _addressGenericRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController
            (
            IProfileRepository profileRepository,
            IGenericRepository<Address> addressGenericRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _profileRepository = profileRepository;
            _addressGenericRepository = addressGenericRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileDetails()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                var data = await _profileRepository.GetProfileDetails(userId);
                if (data == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Profile Details Not Found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = data,
                    message = "Profile Data Fatched successfully"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred",
                    errors = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressViewModel address)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                // check user is available or not 
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                // add to address
                Address data = new Address
                {
                    City = address.City,
                    State = address.State,
                    Street = address.Street,
                    PinCode = address.PinCode,
                    UserId = userId,
                };

                await _addressGenericRepository.InsertAsync(data);
                await _addressGenericRepository.SaveAsync();
                return Ok(new
                {
                    success = true,
                    data = data,
                    message = "Address Data Add successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred",
                    errors = new[] { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressViewModel address)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var addressExist = await _addressGenericRepository.GetByIdAsync(address.AddressId);
                if (addressExist == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Address not found"
                    });
                }

                if (addressExist.UserId != userId)
                {
                    return Forbid(); // or return Unauthorized()
                }

                addressExist.City = address.City;
                addressExist.State = address.State;
                addressExist.Street = address.Street;
                addressExist.PinCode = address.PinCode;

                await _addressGenericRepository.SaveAsync();

                return Ok(new
                {
                    success = true,
                    message = "Address updated successfully",
                    data = addressExist
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred",
                    errors = new[] { ex.Message }
                });
            }
        }

    }
}
