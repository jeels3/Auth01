using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TradesCompany_AW.Application.Repository;
using TradesCompany_AW.Domain.Entities;
using TradesCompany_AW.Web.Services;
using TradesCompany_AW.Web.ViewModel;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IServiceRepository _servicetypeRepository;
        private readonly IGenericRepository<ServiceType> _serviceTypeGenericRepository;
        private readonly ILogger<AdminController> _logger;
        private readonly ImageService _imageService;

        public AdminController(
            IServiceRepository servicetypeRepository,
            IGenericRepository<ServiceType> serviceTypeGenericRepository,
            ILogger<AdminController> logger,
            ImageService imageService)
        {
            _servicetypeRepository = servicetypeRepository ?? throw new ArgumentNullException(nameof(servicetypeRepository));
            _serviceTypeGenericRepository = serviceTypeGenericRepository ?? throw new ArgumentNullException(nameof(serviceTypeGenericRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageService = imageService;
        }

        /// <summary>
        /// Gets a service by service name
        /// </summary>
        /// <param name="serviceName">The name of the service to search for</param>
        /// <returns>Service details if found</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByServiceName(string serviceName)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    _logger.LogWarning("GetByServiceName called with empty or null service name");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service name is required and cannot be empty",
                        errors = new[] { "ServiceName parameter is required" }
                    });
                }

                // Trim and validate length
                serviceName = serviceName.Trim();
                if (serviceName.Length > 100) // Assuming max length of 100
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service name is too long (maximum 100 characters)",
                        errors = new[] { "ServiceName exceeds maximum length" }
                    });
                }

                _logger.LogInformation("Searching for service with name: {ServiceName}", serviceName);

                var service = await _servicetypeRepository.GetServiceByServiceName(serviceName);

                if (service == null)
                {
                    _logger.LogInformation("Service not found with name: {ServiceName}", serviceName);
                    return NotFound(new
                    {
                        success = false,
                        message = "Service not found",
                        serviceName = serviceName
                    });
                }

                _logger.LogInformation("Service found successfully: {ServiceName}", serviceName);
                return Ok(new
                {
                    success = true,
                    data = service,
                    message = "Service retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service by name: {ServiceName}", serviceName);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving the service",
                    error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Adds a new service
        /// </summary>
        /// <param name="service">The service to add</param>
        /// <returns>Success message if added successfully</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add([FromForm] ServiceTypeViewModel service)
        {
            try
            {
                // Input validation
                if (service == null)
                {
                    _logger.LogWarning("Add method called with null service");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service data is required",
                        errors = new[] { "Request body cannot be null" }
                    });
                }

                // Model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    _logger.LogWarning("Model validation failed for service: {Errors}", string.Join(", ", errors));
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errors
                    });
                }

                // Additional business logic validation
                if (string.IsNullOrWhiteSpace(service.ServiceName))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service name is required",
                        errors = new[] { "ServiceName is required" }
                    });
                }

                // Check for duplicate service name
                var existingService = await _servicetypeRepository.GetServiceByServiceName(service.ServiceName.Trim());
                if (existingService != null)
                {
                    _logger.LogWarning("Attempted to add duplicate service: {ServiceName}", service.ServiceName);
                    return Conflict(new
                    {
                        success = false,
                        message = "A service with this name already exists",
                        serviceName = service.ServiceName
                    });
                }

                // Set audit fields if they exist
                if (service.GetType().GetProperty("CreatedDate") != null)
                {
                    service.GetType().GetProperty("CreatedDate")?.SetValue(service, DateTime.UtcNow);
                }

                _logger.LogInformation("Adding new service: {ServiceName}", service.ServiceName);
                var imagePath = await _imageService.SaveImageAsync(service.Image);
                ServiceType s = new ServiceType
                {
                    ServiceName = service.ServiceName,
                    imgLink = imagePath
                };

                await _serviceTypeGenericRepository.InsertAsync(s);
                await _serviceTypeGenericRepository.SaveAsync();

                _logger.LogInformation("Service added successfully: {ServiceName} with ID: {Id}",
                    service.ServiceName, service.Id);

                return CreatedAtAction(nameof(GetByServiceName),
                    new { serviceName = service.ServiceName },
                    new
                    {
                        success = true,
                        message = "Service added successfully",
                        data = new { id = service.Id, serviceName = service.ServiceName }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding service: {ServiceName}", service?.ServiceName);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while adding the service",
                    error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Updates an existing service
        /// </summary>
        /// <param name="service">The service to update</param>
        /// <returns>Success message if updated successfully</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] ServiceType service)
        {
            try
            {
                // Input validation
                if (service == null)
                {
                    _logger.LogWarning("Update method called with null service");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Service data is required",
                        errors = new[] { "Request body cannot be null" }
                    });
                }

                // Model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    _logger.LogWarning("Model validation failed for service update: {Errors}", string.Join(", ", errors));
                    return BadRequest(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errors
                    });
                }

                // Check if ID is provided
                if (service.Id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Valid service ID is required",
                        errors = new[] { "Id must be greater than 0" }
                    });
                }

                // Check if service exists
                var existingService = await _serviceTypeGenericRepository.GetByIdAsync(service.Id);
                if (existingService == null)
                {
                    _logger.LogWarning("Attempted to update non-existent service with ID: {Id}", service.Id);
                    return NotFound(new
                    {
                        success = false,
                        message = "Service not found",
                        serviceId = service.Id
                    });
                }

                // Check for duplicate service name (excluding current service)
                if (!string.IsNullOrWhiteSpace(service.ServiceName))
                {
                    var duplicateService = await _servicetypeRepository.GetServiceByServiceName(service.ServiceName.Trim());
                    if (duplicateService != null && duplicateService.Id != service.Id)
                    {
                        _logger.LogWarning("Attempted to update service with duplicate name: {ServiceName}", service.ServiceName);
                        return Conflict(new
                        {
                            success = false,
                            message = "A service with this name already exists",
                            serviceName = service.ServiceName
                        });
                    }
                }

                // Set audit fields if they exist
                if (service.GetType().GetProperty("UpdatedDate") != null)
                {
                    service.GetType().GetProperty("UpdatedDate")?.SetValue(service, DateTime.UtcNow);
                }

                _logger.LogInformation("Updating service: {ServiceName} with ID: {Id}", service.ServiceName, service.Id);

                await _serviceTypeGenericRepository.UpdateAsync(service);
                await _serviceTypeGenericRepository.SaveAsync();

                _logger.LogInformation("Service updated successfully: {ServiceName} with ID: {Id}",
                    service.ServiceName, service.Id);

                return Ok(new
                {
                    success = true,
                    message = "Service updated successfully",
                    data = new { id = service.Id, serviceName = service.ServiceName }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service with ID: {Id}", service?.Id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while updating the service",
                    error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Deletes a service by ID
        /// </summary>
        /// <param name="id">The ID of the service to delete</param>
        /// <returns>Success message if deleted successfully</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Input validation
                if (id <= 0)
                {
                    _logger.LogWarning("Delete method called with invalid ID: {Id}", id);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Valid service ID is required",
                        errors = new[] { "Id must be greater than 0" }
                    });
                }

                // Check if service exists
                var service = await _serviceTypeGenericRepository.GetByIdAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent service with ID: {Id}", id);
                    return NotFound(new
                    {
                        success = false,
                        message = "Service not found",
                        serviceId = id
                    });
                }

                // Check for dependencies (if you have related entities)
                // This is a placeholder - implement based on your business logic
                var hasDependencies = await CheckServiceDependencies(id);
                if (hasDependencies)
                {
                    _logger.LogWarning("Attempted to delete service with dependencies: {Id}", id);
                    return Conflict(new
                    {
                        success = false,
                        message = "Cannot delete service as it has related records",
                        serviceId = id
                    });
                }

                _logger.LogInformation("Deleting service: {ServiceName} with ID: {Id}", service.ServiceName, id);

                await _serviceTypeGenericRepository.DeleteAsync(service);
                await _serviceTypeGenericRepository.SaveAsync();

                _logger.LogInformation("Service deleted successfully: ID {Id}", id);

                return Ok(new
                {
                    success = true,
                    message = "Service deleted successfully",
                    deletedServiceId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting service with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while deleting the service",
                    error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Gets all services with optional pagination
        /// </summary>
        /// <param name="page">Page number (starting from 1)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>List of all services</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Page number must be greater than 0",
                        errors = new[] { "Page parameter must be >= 1" }
                    });
                }

                if (pageSize < 1 || pageSize > 1000)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Page size must be between 1 and 1000",
                        errors = new[] { "PageSize parameter must be between 1 and 1000" }
                    });
                }

                _logger.LogInformation("Getting all services - Page: {Page}, PageSize: {PageSize}", page, pageSize);

                var services = await _serviceTypeGenericRepository.GetAllAsync();

                // Apply pagination
                var totalCount = services.Count();
                var paginatedServices = services
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                _logger.LogInformation("Retrieved {Count} services out of {TotalCount}",
                    paginatedServices.Count, totalCount);

                return Ok(new
                {
                    success = true,
                    data = paginatedServices,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    },
                    message = $"Retrieved {paginatedServices.Count} services successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all services");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while retrieving services",
                    error = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Checks if a service has dependencies that prevent deletion
        /// </summary>
        /// <param name="serviceId">The service ID to check</param>
        /// <returns>True if has dependencies, false otherwise</returns>
        private async Task<bool> CheckServiceDependencies(int serviceId)
        {
            try
            {
                // Implement your business logic here
                // For example, check if service is used in orders, bookings, etc.
                // This is a placeholder implementation

                // Example:
                // var hasOrders = await _orderRepository.HasOrdersForService(serviceId);
                // var hasBookings = await _bookingRepository.HasBookingsForService(serviceId);
                // return hasOrders || hasBookings;

                return false; // Placeholder - always return false for now
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking dependencies for service ID: {ServiceId}", serviceId);
                return true; // Return true to prevent deletion if we can't check dependencies
            }
        }
    }

    // Custom validation attributes (optional)
    public class ServiceNameValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string serviceName)
            {
                return !string.IsNullOrWhiteSpace(serviceName) &&
                       serviceName.Trim().Length >= 2 &&
                       serviceName.Trim().Length <= 100;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be between 2 and 100 characters long.";
        }
    }
}