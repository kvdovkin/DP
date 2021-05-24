using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Infrastructure.Storage
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly Dictionary<string, IConnectionMultiplexer> _dbDic;

        public RedisStorage()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(Constants.host);
            _dbDic = new Dictionary<string, IConnectionMultiplexer>() 
            {
                {Constants.RusId, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.RusDB, EnvironmentVariableTarget.User)) },
                {Constants.EUId, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.EUDB, EnvironmentVariableTarget.User)) },
                {Constants.OtherId, ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable(Constants.OtherDB, EnvironmentVariableTarget.User)) }
            };
        }
        

        public void Store(string sKey, string key, string value)
        {
            IDatabase db = GetConnection(sKey).GetDatabase();
            db.StringSet(key, value);
        }
        public void StoreValue(string keyText, string sKey, string value)
        {
            IDatabase db = GetConnection(sKey).GetDatabase();
            db.SetAdd(keyText, value);
        }
        public string Load(string sKey, string key)
        {
            IDatabase db = GetConnection(sKey).GetDatabase();
            return db.StringGet(key);
        }
        public string GetSKey(string id)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            return db.StringGet(id);
        }

        public bool CheckingKey(string sKey, string key)
        {
            IDatabase db = GetConnection(sKey).GetDatabase();
            return db.KeyExists(key);
        }
        public bool CheckingValue(string valueId, string sKey, string value)
        {
            IDatabase db = GetConnection(sKey).GetDatabase();
            return db.SetContains(valueId, value);
        }
        public void StoreSKey(string id, string sKey)
        {
            IDatabase db = _connectionMultiplexer.GetDatabase();
            db.StringSet(id, sKey);
        }

        private IConnectionMultiplexer GetConnection(string sKey)
        {
            IConnectionMultiplexer _connection;

            if (_dbDic.TryGetValue(sKey, out _connection))
            {
                return _connection;
            }

            else
            {
                return _connectionMultiplexer;
            }
        }
    }
}
