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
    public class FactController : BaseApiController<FactController>
    {
        private readonly IFactStoreService _factStoreService;

        public FactController(IFactStoreService factStoreService)
        {
            _factStoreService = factStoreService ?? throw new ArgumentNullException(nameof(factStoreService));
        }

        /// <summary>
        /// Returns all facts
        /// </summary>
        /// <param name="factParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FactParameters factParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllFactsAsync(factParameters));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Returns all facttype facts
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="factParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>   
        [HttpGet]
        [Route("FactType/{factTypeName}")]
        public async Task<IActionResult> GetAllFactTypeFacts([FromRoute] string factTypeName, [FromQuery] FactParameters factParameters)
        {
            try
            {
                return Ok(await _factStoreService.GetAllFactsAsync(factTypeName, factParameters));
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
        /// Returns all facttype facts as key values
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="factParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>   
        [HttpGet]
        [Route("FactType/{factTypeName}/KeyValues")]
        public async Task<IActionResult> GetAllFactTypeFactsKeyValues([FromRoute] string factTypeName, [FromQuery] FactParameters factParameters)
        {
            try
            {
                var result = await _factStoreService.GetAllFactsAsync(factTypeName, factParameters);

                var dict = new Dictionary<string, string>();
                foreach (var item in result.Items)
                {
                    dict.Add(item.Key, item.Value);
                }

                return Ok(dict);
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
        /// Get one fact by facttype and key
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
                var fact = await _factStoreService.GetFactAsync(factTypeName, key);

                if (fact == null)
                    return NotFound();
                else
                    return Ok(fact);
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
        /// Insert one fact in the database
        /// </summary>
        /// <param name="fact">The create fact</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Fact fact)
        {
            try
            {
                var factExists = await _factStoreService.GetFactAsync(fact.FactTypeName, fact.Key);
                if (factExists != null)
                    throw new ValidationException("Fact bestaat al");

                var createdFact = await _factStoreService.CreateFactAsync(fact);

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
        /// Update a fact from the database
        /// </summary>
        /// <param name="fact">The update fact</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpPut()]
        public async Task<ActionResult> Update([FromBody] UpdateFact fact)
        {
            try
            {
                var updatedFact = await _factStoreService.UpdateFactAsync(fact);

                return Ok(updatedFact);
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
        /// Upsert batch facts
        /// </summary>
        /// <param name="facts">The facts batch</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin)]
        [HttpPut()]
        [Route("Batch-Upsert")]
        public async Task<ActionResult> BatchUpsert([FromBody] List<Fact> facts)
        {
            try
            {
                foreach (Fact fact in facts)
                {
                    var factExists = await _factStoreService.GetFactAsync(fact.FactTypeName, fact.Key);
                    if (factExists != null)
                    {
                        var updateFact = new UpdateFact();
                        updateFact.Description = fact.Description;
                        updateFact.NewKey = fact.Key;
                        updateFact.PreviousKey = fact.Key;
                        updateFact.PreviousFactTypeName = fact.FactTypeName;
                        updateFact.NewFactTypeName = fact.FactTypeName;
                        updateFact.Value = fact.Value;
                        updateFact.Description = fact.Description;
                        await _factStoreService.UpdateFactAsync(updateFact);
                    }
                    else
                    {
                        await _factStoreService.CreateFactAsync(fact);
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
        /// Delete a fact from the database
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="key">The fact's Key</param>
        /// <returns></returns>
        [AuthorizeRoles(Roles.Admin, Roles.SuperAdmin, Roles.Writer)]
        [HttpDelete]
        [Route("FactType/{factTypeName}/Facts/{key}")]
        public async Task<IActionResult> Delete(string factTypeName, string key)
        {
            try
            {
                await _factStoreService.DeleteFactAsync(factTypeName, key);

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
