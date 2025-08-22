using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Application.Services;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Web.ViewModel;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize(Roles = "USER")]
    public class BookingController : ControllerBase
    {
        private readonly IGenericRepository<CustomerBooking> _bookingGenericRepository;
        private readonly IGenericRepository<ServiceType> _serviceTypeGenericRepository;
        private readonly INotificationService _notificationService;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IGenericRepository<CustomerBooking> bookingGenericRepository,
                                 IGenericRepository<ServiceType> serviceTypeGenericRepository,
                                 INotificationService notificationService,
                                 IServiceRepository serviceRepository,
                                 IBookingRepository bookingRepository)
        {
            _bookingGenericRepository = bookingGenericRepository;
            _serviceTypeGenericRepository = serviceTypeGenericRepository;
            _notificationService = notificationService;
            _serviceRepository = serviceRepository;
            _bookingRepository = bookingRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetBookingByServiceType()
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

                int seviceTypeId = await _serviceRepository.GetServiceTypeByUserId(userId);
                if (seviceTypeId == 0 || seviceTypeId == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service Not Found"
                    });
                }

                var data = await _bookingRepository.GetAllBookingByServiceType(seviceTypeId);
                if(data == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Booking Not Found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = data,
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

        [HttpGet]
        public async Task<IActionResult> GetBookingByUserId()
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

                var data = await _bookingRepository.GetAllBookingByUserId(userId);
                if (data == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Booking Not Found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = data,
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

        [HttpPost]
        public async Task<IActionResult> UpdateBookingByCustomer([FromBody] UpdateBookingDto data)
        {
            try
            {
                var booking = await _bookingGenericRepository.GetByIdAsync(data.id);
                if (booking == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Booking not found"
                    });
                }

                booking.Price = data.price;
                booking.WorkDetails = data.workDetails;
                await _bookingGenericRepository.SaveAsync();
                return Ok(new
                {
                    success = true,
                    data = booking,
                    message = "Booking Update successfully"
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
