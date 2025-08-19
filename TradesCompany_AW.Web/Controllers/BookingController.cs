using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Application.Services;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Web.ViewModel;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class BookingController : ControllerBase
    {
        private readonly IGenericRepository<CustomerBooking> _bookingGenericRepository;
        private readonly IGenericRepository<ServiceType> _serviceTypeGenericRepository;
        private readonly INotificationService _notificationService;

        public BookingController(IGenericRepository<CustomerBooking> bookingGenericRepository,
                                 IGenericRepository<ServiceType> serviceTypeGenericRepository,
                                 INotificationService notificationService)
        {
            _bookingGenericRepository = bookingGenericRepository;
            _serviceTypeGenericRepository = serviceTypeGenericRepository;
            _notificationService = notificationService;
        }

        // create booking
        [HttpPost]
        public async Task<IActionResult> BookService([FromBody] CustomerBookingViewModel data)
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
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToArray();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors
                    });
                }

                var serviceType = await _serviceTypeGenericRepository.GetByIdAsync(data.ServiceTypeId);
                if (serviceType == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"ServiceType with ID {data.ServiceTypeId} not found"
                    });
                }

                CustomerBooking customerBooking = new CustomerBooking
                {
                    ServiceTypeId = data.ServiceTypeId,
                    UserId = userId,
                    WorkDetails = data.WorkDetails,
                    Price = data.Price,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                await _bookingGenericRepository.InsertAsync(customerBooking);
                await _bookingGenericRepository.SaveAsync();
                await _notificationService.SendBookingNotificationToEmployee(data.ServiceTypeId, $"New Service Booking By {"customer"}");
                return Ok(new
                {
                    success = true,
                    data = customerBooking,
                    message = "Booking created successfully"
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
