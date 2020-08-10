using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F3N.Hoard.Shared;
using F3N.Hoard.Storage;
using F3N.Hoard.Utils;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace F3N.Hoard.BlazorLocalStorage
{
    public class BlazorStore : ProtectedSessionStorage, IStorage
    {
        private Dictionary<string, Type> _keyTypes;
        
        public BlazorStore(IJSRuntime jsRuntime, IDataProtectionProvider dataProtectionProvider) : base(jsRuntime, dataProtectionProvider)
        {
            _keyTypes = new Dictionary<string, Type>();
        }

        public async Task<T> Get<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            return await base.GetAsync<T>(key);
        }

        public async Task<T> GetByKey<T>(string key)
        {
            return await base.GetAsync<T>(key);
        }

        public async Task<List<T>> GetAll<T>()
        {
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T))
                .Select(x => x.Key);

            List<T> values = new List<T>();
            await keys.ForEach(async x =>
            {
                var val = await base.GetAsync<T>(x);
                values.Add(val);
            });
            return values;
        }

        public async Task Save<T>(string id, T data)
        {
            string key = Utility.MakeKey<T>(id);
            _keyTypes.TryAdd(key, typeof(T));
            await base.SetAsync(key, data);
        }

        public async Task SaveByKey<T>(string key, T data)
        {
            _keyTypes.TryAdd(key, typeof(T));
            await base.SetAsync(key, data);
        }

        public async Task SaveAllByKey<T>(Dictionary<string, T> data)
        {
            foreach (var keyValuePair in data)
            {
                _keyTypes.TryAdd(keyValuePair.Key, typeof(T));
                await base.SetAsync(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public async Task Delete<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            await base.DeleteAsync(key);
        }

        public async Task DeleteByKey<T>(string key)
        {
            await base.DeleteAsync(key);
        }

        public async Task DeleteAll<T>()
        {
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T))
                .Select(x => x.Key);

            await keys.ForEach(async x =>
            {
                await base.DeleteAsync(x);
            });
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public Task Flush()
        {
            throw new NotImplementedException();
        }
    }
}