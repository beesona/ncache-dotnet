using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using ncache.models;
using Newtonsoft.Json;

namespace ncahe_dotnet.Managers
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;

        public CacheManager(IDistributedCache distributedCache)
        {
            _cache = distributedCache;
        }

        public async Task<bool> SetHashAsync(HashModel hashData)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hashData.fields));
                await _cache.SetAsync(hashData.key, data);
            }
            catch (Exception ex)
            {
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

                    return returnData;
                }
                else
                {
                    return new HashModel();
                }

            }
            catch (Exception ex)
            {
                return new HashModel();
                throw ex;
            }
        }
    }
}
