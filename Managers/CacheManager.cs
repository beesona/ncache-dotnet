using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using ncache.models;
using Newtonsoft.Json;

namespace ncahe_dotnet.Managers
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheManager> _logger;

        public CacheManager(IDistributedCache distributedCache, ILogger<CacheManager> logger)
        {
            _cache = distributedCache;
            _logger = logger;
        }

        public async Task<bool> SetHashAsync(HashModel hashData)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hashData.fields));
                await _cache.SetAsync(hashData.key, data);
                _logger.LogInformation($"Added Key {hashData.key} to the cache. {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving key {hashData.key} to the cache. {DateTime.Now}");
                return false;
                throw ex;
            }

            return true;

        }

        public async Task<HashModel> GetHashByKeyAsync(string key)
        {
            try
            {
                var data = await _cache.GetAsync(key);

                if (data != null)
                {
                    var dataAsString = Encoding.UTF8.GetString(data);
                    HashModel returnData = new HashModel();
                    returnData.key = key;
                    returnData.fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsString);

                    _logger.LogInformation($"Retrieved Key {key} from the cache. {DateTime.Now}");

                    return returnData;
                }
                else
                {
                    return new HashModel();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving key {key} from the cache. {DateTime.Now}");
                return new HashModel();
                throw ex;
            }
        }
    }
}
