using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using F3N.Hoard.State;
using F3N.Hoard.Storage;
using F3N.Hoard.Utils;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace F3N.Hoard.BlazorLocalStorage
{
    public class KeyVal
    {
        public string Key { get; set; }
        public string ValType { get; set; }
    }
    
    public class BlazorStore : ProtectedLocalStorage, IStorage
    {
        private Dictionary<string, string> _keyTypes;
        //private List<KeyVal> _keyTypes;
        
        public BlazorStore(IJSRuntime jsRuntime, IDataProtectionProvider dataProtectionProvider) : base(jsRuntime, dataProtectionProvider)
        {
            _keyTypes = new Dictionary<string, string>();
            //_keyTypes = new List<KeyVal>();
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
            if (_keyTypes.Count == 0)
            {
                try
                {
                    var kt = await base.GetAsync<Dictionary<string, string>>("KeyTypes");
                    if (kt != null) _keyTypes = kt;
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
                var val = await base.GetAsync<T>(x);
                values.Add(val);
            });
            return values;
        }

        public async Task Save<T>(string id, T data)
        {
            string key = Utility.MakeKey<T>(id);
            _keyTypes.TryAdd(key, typeof(T).ToString());
            //_keyTypes.Add(new KeyVal(){Key=key, ValType=typeof(T).ToString()});
            await base.SetAsync(key, data);
            await SaveKeyTypes();
        }

        public async Task SaveByKey<T>(string key, T data)
        {
            _keyTypes.TryAdd(key, typeof(T).ToString());
            //_keyTypes.Add(new KeyVal(){Key=key, ValType=typeof(T).ToString()});
            await base.SetAsync(key, data);
            await SaveKeyTypes();
        }

        public async Task SaveAllByKey<T>(Dictionary<string, T> data)
        {
            foreach (var keyValuePair in data)
            {
                _keyTypes.TryAdd(keyValuePair.Key, typeof(T).ToString());
                //_keyTypes.Add(new KeyVal(){Key=keyValuePair.Key, ValType=typeof(T).ToString()});
                await base.SetAsync(keyValuePair.Key, keyValuePair.Value);
            }

            await SaveKeyTypes();
        }

        public async Task Delete<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            _keyTypes.Remove(key);
            await base.DeleteAsync(key);
        }

        public async Task DeleteByKey<T>(string key)
        {
            _keyTypes.Remove(key);
            await base.DeleteAsync(key);
        }

        public async Task DeleteAll<T>()
        {
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T).ToString())
                .Select(x => x.Key);

            await keys.ForEach(async x =>
            {
                _keyTypes.Remove(x);
                await base.DeleteAsync(x);
            });
        }

        public async Task Clear()
        {
            await _keyTypes.Keys.ForEach(async x =>
            {
                await base.DeleteAsync(x);
            });
            _keyTypes.Clear();
        }

        public Task Flush()
        {
            throw new NotImplementedException();
        }

        private async Task SaveKeyTypes()
        {
            await base.SetAsync("KeyTypes", _keyTypes);
        }
    }

}