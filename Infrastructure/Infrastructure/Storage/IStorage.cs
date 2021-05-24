using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
     public interface IStorage
     {
         void Store(string sKey, string key, string value);
         void StoreValue(string valueId, string sKey, string value);
         string Load(string sKey, string key);
         string GetSKey(string id);
         bool CheckingKey(string sKey, string key);
         bool CheckingValue(string valueId, string sKey, string value); 
         void StoreSKey(string id, string sKey);
        

     }
}

