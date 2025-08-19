using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.Repository;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var count = await _notificationRepository.GetCountByUserId(userId);
            return Ok(count);
        }
    }
}
