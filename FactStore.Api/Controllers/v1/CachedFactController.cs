using FactStore.Api.Services;
using FactStore.Models.Authorization;
using FactStore.Models.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace FactStore.Api.Controllers.v1
{
    [Authorize]
    public class CachedFactController : BaseApiController<CachedFactController>
    {
        private readonly IFactStoreService _factStoreService;
        private readonly IDistributedCache _distributedCache;

        public CachedFactController(IFactStoreService factStoreService, IDistributedCache distributedCache)
        {
            _factStoreService = factStoreService ?? throw new ArgumentNullException(nameof(factStoreService));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        /// <summary>
        /// Returns all facttype facts as key values
        /// </summary>
        /// <param name="factTypeName">The factType's name</param>
        /// <param name="factParameters">Parameters for sorting, paging, filtering,...</param>
        /// <returns></returns>   
        [HttpGet]
        [AuthorizeRoles(Roles.SuperAdmin)]
        [Route("FactType/{factTypeName}/KeyValues")]
        public async Task<IActionResult> GetAllFactTypeFactsKeyValues([FromRoute] string factTypeName, [FromQuery] FactParameters factParameters)
        {
            try
            {
                var cacheKey = $"FactType-{factTypeName}-Page{factParameters.PageIndex}-Size{factParameters.PageSize}";
                var cachedKeyValues = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedKeyValues))
                {
                    var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedKeyValues);
                    return Ok(jsonDict);
                }
                else
                {
                    var result = await _factStoreService.GetAllFactsAsync(factTypeName, factParameters);

                    var dict = new Dictionary<string, string>();
                    foreach (var item in result.Items)
                    {
                        dict.Add(item.Key, item.Value);
                    }

                    var jsonDict = JsonConvert.SerializeObject(dict);
                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(120));

                    await _distributedCache.SetStringAsync(cacheKey, jsonDict, cacheEntryOptions);

                    return Ok(dict);
                }               
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
