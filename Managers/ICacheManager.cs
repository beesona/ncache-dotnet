using ncache.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ncahe_dotnet.Managers
{
    public interface ICacheManager
    {
        public Task<bool> SetHashAsync(HashModel hashData);

        public Task<HashModel> GetHashByKeyAsync(string key);
    }
}
