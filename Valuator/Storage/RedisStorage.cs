using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Valuator
{
    public class RedisStorage: IStorage
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");

        public RedisStorage()
        {

        }
        public void Store(string key, string value)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            db.StringSet(key, value);
        }

        public bool TextSignes(string prefix, string text)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            var _server = _connectionMultiplexer.GetServer("localhost", 6379);
            var keys = _server.Keys(pattern: "*" + prefix + "*");
            return keys.Select(x => Load(x)).Where(x => x == text).Any();
        }

        public string Load(string key)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            return db.StringGet(key);
        }

        public bool CheckingKey(string key)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();       
            return db.KeyExists(key);
        }
    }
}