using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F3N.Hoard.Storage;
using F3N.Hoard.Utils;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace F3N.Hoard.BlazorServerStorage
{
    public class ProtectedStorage : IStorage
    {
        private ProtectedBrowserStorage _protectedStorage;
        private Dictionary<string, string> _keyTypes;
    

        public ProtectedStorage(ProtectedBrowserStorage protectedStorage)
        {
            _protectedStorage = protectedStorage;
            _keyTypes = new Dictionary<string, string>();
        }

        public async Task<T> Get<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            var result = await _protectedStorage.GetAsync<T>(key);
            return result.Success ? result.Value : default(T);
        }

        public async Task<T> GetByKey<T>(string key)
        {
            var result = await _protectedStorage.GetAsync<T>(key);
            return result.Success ? result.Value : default(T);
        }

        public async Task<List<T>> GetAll<T>()
        {
            if (_keyTypes.Count == 0)
            {
                try
                {
                    var kt = await _protectedStorage.GetAsync<Dictionary<string, string>>("HD_PBS_KeyTypes");
                    if (kt.Success) _keyTypes = kt.Value;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                }
            }
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T).ToString())
                .Select(x => x.Key);

            List<T> values = new List<T>();
            await keys.ForEach(async x =>
            {
                var val = await _protectedStorage.GetAsync<T>(x);
                values.Add(val.Value);
            });
            return values;
        }

        public async Task Save<T>(string id, T data)
        {
            string key = Utility.MakeKey<T>(id);
            _keyTypes.TryAdd(key, typeof(T).ToString());
            await _protectedStorage.SetAsync(key, data);
            await SaveKeyTypes();
        }

        public async Task SaveByKey<T>(string key, T data)
        {
            _keyTypes.TryAdd(key, typeof(T).ToString());
            await _protectedStorage.SetAsync(key, data);
            await SaveKeyTypes();
        }

        public async Task SaveAllByKey<T>(Dictionary<string, T> data)
        {
            foreach (var keyValuePair in data)
            {
                _keyTypes.TryAdd(keyValuePair.Key, typeof(T).ToString());
                await _protectedStorage.SetAsync(keyValuePair.Key, keyValuePair.Value);
            }

            await SaveKeyTypes();
        }

        public async Task Delete<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            _keyTypes.Remove(key);
            await _protectedStorage.DeleteAsync(key);
        }

        public async Task DeleteByKey<T>(string key)
        {
            _keyTypes.Remove(key);
            await _protectedStorage.DeleteAsync(key);
        }

        public async Task DeleteAll<T>()
        {
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T).ToString())
                .Select(x => x.Key);

            await keys.ForEach(async x =>
            {
                _keyTypes.Remove(x);
                await _protectedStorage.DeleteAsync(x);
            });
        }

        public async Task Clear()
        {
            await _keyTypes.Keys.ForEach(async x =>
            {
                await _protectedStorage.DeleteAsync(x);
            });
            _keyTypes.Clear();
        }

        public Task Flush()
        {
            throw new NotImplementedException();
        }

        private async Task SaveKeyTypes()
        {
            await _protectedStorage.SetAsync("HD_PBS_KeyTypes", _keyTypes);
        }
    }
}