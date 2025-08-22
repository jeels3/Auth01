using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> getunreadnotificationcount()
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
                var count = await _notificationRepository.GetCountByUserId(userId);
                return Ok(new
                {
                    success = true,
                    data = count,
                    message = "Notification count get successfully"
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
        public async Task<IActionResult> getunreadnotification()
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
                var notifications = await _notificationRepository.GetNotificationByUserId(userId);
                return Ok(new
                {
                    success = true,
                    data = notifications,
                    message = "Notifications get successfully"
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
        public async Task<IActionResult> markAsSeenNotification([FromBody] int id)
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

                if (id == 0 && id == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Notification Not Found"
                    });

                }

                var result = await _notificationRepository.NotificatinSeen(userId , id);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        data = result,
                        message = "Notifications get successfully"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "An unexpected error occurred",
                        errors = new[] { "An unexpected error occurred" }
                    });
                }

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
