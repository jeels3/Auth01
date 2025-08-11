using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradesCompany_AW.Application.DTOs;

namespace TradesCompany_AW.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministratorController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: api/roles
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            return Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(new { message = "Role name cannot be empty" });

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return BadRequest(new { message = "Role already exists" });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Role created successfully" });
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Role updated successfully" });
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Role deleted successfully" });
        }

        [HttpGet("{roleId}/claims")]
        public async Task<IActionResult> GetRoleClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims);
        }

        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> AddClaimToRole(string roleId, [FromBody] ClaimDto claimDto)
        {
            if (string.IsNullOrWhiteSpace(claimDto.Type) || string.IsNullOrWhiteSpace(claimDto.Value))
                return BadRequest(new { message = "Claim type and value are required" });

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var result = await _roleManager.AddClaimAsync(role, new Claim(claimDto.Type, claimDto.Value));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim added successfully" });
        }

        [HttpDelete("{roleId}/claims")]
        public async Task<IActionResult> RemoveClaimFromRole(string roleId, [FromBody] ClaimDto claimDto)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found" });

            var result = await _roleManager.RemoveClaimAsync(role, new Claim(claimDto.Type, claimDto.Value));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim removed successfully" });
        }    
    }
}
