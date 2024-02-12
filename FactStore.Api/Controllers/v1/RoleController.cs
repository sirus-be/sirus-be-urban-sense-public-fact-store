using FactStore.Api.Services;
using FactStore.Models;
using FactStore.Models.Authorization;
using FactStore.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace FactStore.Api.Controllers.v1
{
    [Authorize]
    public class RoleController : BaseApiController<RoleController>
    {
        private readonly IFactStoreService _factStoreService;

        public RoleController(IFactStoreService factStoreService)
        {
            _factStoreService = factStoreService ?? throw new ArgumentNullException(nameof(factStoreService));
        }

        /// <summary>
        /// Returns all roles
        /// </summary>
        /// <param name="roleParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] RoleParameters roleParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllRolesAsync(roleParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Returns all facttype roles
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="roleParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>   
        [HttpGet]
        [Route("FactType/{factTypeName}")]
        public async Task<IActionResult> GetAllFactTypeRoles([FromRoute] string factTypeName, [FromQuery]RoleParameters roleParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllRolesAsync(factTypeName, roleParameters));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get one role by id
        /// </summary>
        /// <param name="id">The role's Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [Produces("application/ld+json")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var role = await _factStoreService.GetRoleByNameAsync(name);

                if (role == null)
                    return NotFound();
                else
                    return Ok(role);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Insert one role in the database
        /// </summary>
        /// <param name="role">The create role</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            try
            {
                var roleExists = await _factStoreService.GetRoleByNameAsync(role.Name);
                if (roleExists != null)
                    throw new ValidationException("Role bestaat al");

                var createdRole = await _factStoreService.CreateRoleAsync(role);

                return Ok(createdRole);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Conflict(new System.Web.Http.HttpError { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Update a role from the database
        /// </summary>
        /// <param name="role">The update role</param>
        /// <returns></returns>
        [HttpPut()]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult> Update([FromBody] UpdateRole role)
        {
            try
            {
                var updatedRole = await _factStoreService.UpdateRoleAsync(role);

                return Ok(updatedRole);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
