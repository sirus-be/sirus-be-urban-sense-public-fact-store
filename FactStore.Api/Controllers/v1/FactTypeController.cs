using FactStore.Api.Services;
using FactStore.Models;
using FactStore.Models.Authorization;
using FactStore.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace FactStore.Api.Controllers.v1
{
    [Authorize]
    public class FactTypeController : BaseApiController<FactTypeController>
    {
        private readonly IFactStoreService _factStoreService;

        public FactTypeController(IFactStoreService factStoreService)
        {
            _factStoreService = factStoreService ?? throw new ArgumentNullException(nameof(factStoreService));
        }

        /// <summary>
        /// Returns all facttypes
        /// </summary>
        /// <param name="factTypeParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FactTypeParameters factTypeParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllFactTypesAsync(factTypeParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get one facttype by id
        /// </summary>
        /// <param name="id">The factType's Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{name}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var factType = await _factStoreService.GetFactTypeByNameAsync(name);

                if (factType == null)
                    return NotFound();
                else
                    return Ok(factType);
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
        /// Insert one facttype in the database
        /// </summary>
        /// <param name="factType">The create facttype</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin,Roles.SuperAdmin)] 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FactType factType)
        {
            try
            {
                var factTypeExists = await _factStoreService.GetFactTypeByNameAsync(factType.Name);
                if (factTypeExists != null)
                    throw new ValidationException("FactType bestaat al");

                var createdFact = await _factStoreService.CreateFactTypeAsync(factType);

                return Ok(createdFact);
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
        /// Update a facttype from the database
        /// </summary>
        /// <param name="factType">The update facttype</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin)]
        [HttpPut()]
        public async Task<ActionResult> Update([FromBody] UpdateFactType factType)
        {
            try
            {
                var updatedFactType = await _factStoreService.UpdateFactTypeAsync(factType);

                return Ok(updatedFactType);
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
        /// Upsert batch facttypes
        /// </summary>
        /// <param name="factTypes">The facttypes batch</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin)]
        [HttpPut()]
        [Route("Batch-Upsert")]
        public async Task<ActionResult> BatchUpsert([FromBody] List<FactType> factTypes)
        {
            try
            {
                foreach (FactType factType in factTypes)
                {
                    var factTypeExists = await _factStoreService.GetFactTypeByNameAsync(factType.Name);
                    if (factTypeExists != null)
                    {
                        var updateFactType = new UpdateFactType();
                        updateFactType.Description = factType.Description;
                        updateFactType.NewName = factType.Name;
                        updateFactType.PreviousName = factType.Name;
                        updateFactType.Roles = factType.Roles;
                        await _factStoreService.UpdateFactTypeAsync(updateFactType);
                    }
                    else
                    {
                        await _factStoreService.CreateFactTypeAsync(factType);
                    }
                }
                return Ok();
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
        /// Delete a facttype and all related facts from the database
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin)]
        [HttpDelete]
        [Route("{factTypeName}")]
        public async Task<IActionResult> Delete(string factTypeName)
        {
            try
            {
                await _factStoreService.DeleteFactTypeAsync(factTypeName);

                return Ok();
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
    }
}
