using FactStore.Api.Services;
using FactStore.Jobs.Services;
using FactStore.Jobs.Tasks;
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
    public class ExternalFactConfigController : BaseApiController<ExternalFactConfigController>
    {
        private readonly IFactStoreService _factStoreService;
        private readonly IBackgroundProcessingService _backgroundProcessingService;

        public ExternalFactConfigController(IFactStoreService factStoreService, IBackgroundProcessingService backgroundProcessingService)
        {
            _factStoreService = factStoreService ?? throw new ArgumentNullException(nameof(factStoreService));
            _backgroundProcessingService = backgroundProcessingService;
        }

        /// <summary>
        /// Returns all ExternalFactConfigs
        /// </summary>
        /// <param name="pageIndex">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ExternalFactParameters externalFactParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllExternalFactConfigsAsync(externalFactParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get one ExternalFactConfig by id
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="key">The fact's key</param>
        /// <returns></returns>
        [HttpGet]
        [Route("FactType/{factTypeName}/Facts/{key}")]
        [Produces("application/ld+json")]
        public async Task<IActionResult> GetById(string factTypeName, string key)
        {
            try
            {
                var externalFactConfig = await _factStoreService.GetExternalFactConfigAsync(factTypeName, key);

                if (externalFactConfig == null)
                    return NotFound();
                else
                    return Ok(externalFactConfig);
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
        /// Insert one ExternalFactConfig in the database
        /// </summary>
        /// <param name="externalFactConfig">The create externalFactConfigs</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExternalFactConfig externalFactConfig)
        {
            try
            {
                var externalFactConfigExists = await _factStoreService.GetExternalFactConfigAsync(externalFactConfig.FactTypeName, externalFactConfig.Key);
                if (externalFactConfigExists != null)
                    throw new ValidationException("ExternalFactConfig bestaat al");

                var createdExternalFactConfig = await _factStoreService.CreateExternalFactConfigAsync(externalFactConfig);

                return Ok(createdExternalFactConfig);
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
        /// Update an ExternalFactConfig from the database
        /// </summary>
        /// <param name="externalFactConfig">The update externalFactConfig</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpPut()]
        public async Task<ActionResult> Update([FromBody] UpdateExternalFactConfig externalFactConfig)
        {
            try
            {
                var updatedExternalFactConfig = await _factStoreService.UpdateExternalFactConfigAsync(externalFactConfig);

                return Ok(updatedExternalFactConfig);
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
        /// Update the Fact Value of the External Fact
        /// </summary>
        /// <param name="externalFactConfig">The update externalFactConfig</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpPut()]
        [Route("UpdateFactValue")]
        public async Task<ActionResult> UpdateExternalFactFactValue([FromBody] ExternalFactConfig externalFactConfig)
        {
            try
            {
                _backgroundProcessingService.RegisterEnqueuedJob<IUpdateFactValueTask>(x => x.Invoke(externalFactConfig));
                return Ok(externalFactConfig);
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
        /// Delete an ExternalFactConfig from the database
        /// </summary>
        /// <param name="id">The ExternalFactConfig's Id</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpDelete]
        [Route("FactType/{factTypeName}/Facts/{key}")]
        public async Task<IActionResult> Delete(string factTypeName, string key)
        {
            try
            {
                await _factStoreService.DeleteExternalFactConfigAsync(factTypeName, key);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
