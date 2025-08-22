using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TradesCompany_AW.Application.DTOs;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IGenericRepository<Quotation> _quotationGenericrepository;
        private readonly IServiceRepository _serviceRepository;

        public QuotationController
            (
            IGenericRepository<Quotation> quotationGenericrepository,
            IServiceRepository serviceRepository
            )
        {
            _quotationGenericrepository = quotationGenericrepository;
            _serviceRepository = serviceRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuotation([FromBody] QuotationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid data" });
                }
                // find current user servicemanId 
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User not authenticated"
                    });
                }
                var serviceManId = await _serviceRepository.GetServiceManIdByUserId(userId);
                if (serviceManId == null || serviceManId == 0)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "User Is Not Employee"
                    });
                }
                // Check Schedule Time Is Available Or Not

                Quotation quotation = new Quotation
                {
                    BookingId = dto.BookingId,
                    ServiceManId = serviceManId,
                    ServiceDetails = dto.ServiceDetails,
                    Price = dto.Price,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                };

                await _quotationGenericrepository.InsertAsync(quotation);
                await _quotationGenericrepository.SaveAsync();

                return Ok(new
                {
                    success = true,
                    message = "Quotation created successfully",
                    data = quotation
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
