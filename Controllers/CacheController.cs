using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ncache.models;
using ncahe_dotnet.Managers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ncache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ILogger<CacheController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _http;
        private readonly ICacheManager _cacheManager;

        public CacheController(
            ILogger<CacheController> logger,
            IDistributedCache distributedCache,
            IHttpClientFactory httpClientFactory,
            ICacheManager cacheManager)
        {
            _logger = logger;
            _cache = distributedCache;
            _http = httpClientFactory;
            _cacheManager = cacheManager;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<string> Get(string id, string value)
        {
            try
            {
                var cacheKey = id;
                var existingData = await _cache.GetAsync(cacheKey);
                if (existingData != null)
                {
                    _logger.Log(LogLevel.Information, "Fetched from cache : " + Encoding.UTF8.GetString(existingData));
                    return "Fetched from cache : " + Encoding.UTF8.GetString(existingData);
                }
                else
                {
                    var setData = value != null ? Encoding.UTF8.GetBytes(value) : Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString());
                    await _cache.SetAsync(cacheKey, setData);
                    _logger.Log(LogLevel.Information, "Added to cache : " + Encoding.UTF8.GetString(setData));
                    return "Added to cache : " + Encoding.UTF8.GetString(setData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Reading or writing to cache");
                return "Error in CacheController";
            }
        }

        [HttpGet]
        [Route("fromurl/{id}")]
        public async Task<string> Get(string id)
        {
            //"https%3A%2F%2Fjsonplaceholder.typicode.com%2Ftodos%2F"
            string returnData = string.Empty;
            var dataFromCache = await GetCacheData(id);
            if (dataFromCache != "")
            {
                return dataFromCache;
            }
            else
            {
                var dataFromUrl = await GetUrlData(id);
                returnData = await SetCacheData(id, dataFromUrl);
            }
            return returnData;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> SetHash([FromBody]HashModel setHashObject)
        {
            var response = await _cacheManager.SetHashAsync(setHashObject);

            if (response == true)
            {
                var resp = await _cacheManager.SetHashAsync(setHashObject);
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        [Route("hash/{key}")]
        public async Task<ActionResult> GetHash(string key)
        {
            try
            {
                var dataFromCache = await _cacheManager.GetHashByKeyAsync(key);
                return Ok(dataFromCache);
            }
            catch
            {
                return Ok(new HashModel());
            }
        }

        private async Task<string> GetUrlData(string url)
        {
            var decodedUrl = System.Web.HttpUtility.UrlDecode(url);
            var request = new HttpRequestMessage(HttpMethod.Get, decodedUrl);

            var client = _http.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsByteArrayAsync();
                var result = Encoding.UTF8.GetString(responseStream);

                return result;
            }
            return string.Empty;
        }

        private async Task<string> GetCacheData(string id)
        {
            try
            {
                var cacheKey = id;
                var existingData = await _cache.GetAsync(cacheKey);
                if (existingData != null)
                {
                    _logger.Log(LogLevel.Information, "Fetched from cache : " + Encoding.UTF8.GetString(existingData));
                    return Encoding.UTF8.GetString(existingData);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Reading or writing to cache");
                return "Error in CacheController";
            }

        }

        private async Task<string> SetCacheData(string id, string value)
        {
            try
            {
                var setData = Encoding.UTF8.GetBytes(value);
                await _cache.SetAsync(id, setData);
                _logger.Log(LogLevel.Information, "Added to cache : " + Encoding.UTF8.GetString(setData));
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Reading or writing to cache");
                return "Error in CacheController";
            }

        }
    }
}
