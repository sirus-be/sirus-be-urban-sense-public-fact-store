using FactStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FactStore.Jobs.Tasks
{
    public class UpdateFactValueTask : IUpdateFactValueTask
    {
        private readonly ILogger<UpdateFactValueTask> _logger;
        private readonly string _factStoreConnectionString;
        private HttpClient _httpClient;

        public UpdateFactValueTask(IConfiguration configuration, ILogger<UpdateFactValueTask> logger)
        {
            _logger = logger;
            _factStoreConnectionString = configuration.GetConnectionString("FactStoreDatabase");
        }

        public async Task Invoke(ExternalFactConfig externalFactConfig)
        {
            try
            {
                _logger.LogInformation($"Processing started");

                _logger.LogInformation("RetrieveFactValue started");
                var factValue = await RetrieveFactValue(externalFactConfig);
                _logger.LogInformation("RetrieveFactValue ended");

                _logger.LogInformation("UpdateFactValue started");
                await UpdateFactValue(externalFactConfig, factValue);
                _logger.LogInformation("UpdateFactValue ended");

                _logger.LogInformation($"Processing ended");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {ex.Message}");
                throw;
            }
        }

        private async Task<string> RetrieveFactValue(ExternalFactConfig externalFactConfig)
        {
            _httpClient = new HttpClient();
            if (externalFactConfig.Authentication)
                SetAuthorizationHeader(externalFactConfig.TokenAuthorizationHeader);

            var response = await _httpClient.GetAsync(externalFactConfig.Url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        private void SetAuthorizationHeader(string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private async Task UpdateFactValue(ExternalFactConfig externalFactConfig, string factValue)
        {
            await using var conn = new NpgsqlConnection(_factStoreConnectionString);
            await conn.OpenAsync();

            //Get factType
            await using var cmd = new NpgsqlCommand($"select \"Id\" from \"FactTypes\" where \"Name\" = @factTypeName and \"IsDeleted\" = False", conn);
            cmd.Parameters.AddWithValue("@factTypeName", externalFactConfig.FactTypeName);
            var factTypeId = (int)cmd.ExecuteScalar();
            if (factTypeId==0)
            {
                _logger.LogError($"FactType {externalFactConfig.FactTypeName} not found");
                return;
            }

            // Get Fact
            await using var cmd2 = new NpgsqlCommand($"select \"Id\" from \"Facts\" where \"Key\" = @key  and \"FactTypeId\" = @factTypeId and \"IsDeleted\" = False", conn);
            cmd2.Parameters.AddWithValue("@key", externalFactConfig.Key);
            cmd2.Parameters.AddWithValue("@factTypeId", factTypeId);
            var factId = (int)cmd2.ExecuteScalar();
            if (factId == 0)
            {
                _logger.LogError($"Fact {externalFactConfig.Key} not found");
                return;
            }

            //Update fact
            await using var cmd3 = new NpgsqlCommand("Update \"Facts\" SET \"Value\" = @factValue, \"LastModifiedOn\" = NOW(), \"LastModifiedBy\" = 'BackgroundJob UpdateFactValue' Where \"Id\" = @factId", conn);
            cmd3.Parameters.AddWithValue("@factValue", factValue);
            cmd3.Parameters.AddWithValue("@factId", factId);
            await cmd3.ExecuteNonQueryAsync();
        }
    }
}
