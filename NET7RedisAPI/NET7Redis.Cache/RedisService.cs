using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NET7Redis.Cache
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _multiplexer;
        public RedisService(string url,string password)
        {
            var options = ConfigurationOptions.Parse(url);
            options.Password = password;
            _multiplexer = ConnectionMultiplexer.Connect(options);
        }
        public IDatabase GetDatabase(int dbIndex)
        {
            return _multiplexer.GetDatabase(dbIndex);
        }
    }
}
