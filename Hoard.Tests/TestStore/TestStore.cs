using F3N.Hoard;
using F3N.Hoard.Storage;
using F3N.Hoard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoard.Tests.TestStore
{
    public class TestStore : IStorage
    {
        private Dictionary<string, string> _keyTypes;
        private Dictionary<string, object> _store;

        public TestStore()
        {
            _keyTypes = new Dictionary<string, string>();
            _store = new Dictionary<string, object>();
        }

        public Task<T> Get<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            return Task.FromResult((T)_store[key]);
        }

        public Task<T> GetByKey<T>(string key)
        {
            return Task.FromResult((T)_store[key]);
        }

        public Task<List<T>> GetAll<T>()
        {
            if (_keyTypes.Count == 0)
            {
                try
                {
                    var kt = (Dictionary<string, string>)_store["KeyTypes"];
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
            keys.ForEach(x =>
            {
                var val = (T)_store[x];
                values.Add(val);
            });

            return Task.FromResult(values);
        }

        public async Task Save<T>(string id, T data)
        {
            string key = Utility.MakeKey<T>(id);
            _keyTypes.TryAdd(key, typeof(T).ToString());
            _store.Add(key,data);
            await SaveKeyTypes();
        }

        public async Task SaveByKey<T>(string key, T data)
        {
            _keyTypes.TryAdd(key, typeof(T).ToString());
            _store.Add(key, data);
            await SaveKeyTypes();
        }

        public async Task SaveAllByKey<T>(Dictionary<string, T> data)
        {
            foreach (var keyValuePair in data)
            {
                _keyTypes.TryAdd(keyValuePair.Key, typeof(T).ToString());
                _store.Add(keyValuePair.Key, keyValuePair.Value);
            }

            await SaveKeyTypes();
        }

        public Task Delete<T>(string id)
        {
            var key = Utility.MakeKey<T>(id);
            _keyTypes.Remove(key);
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public Task DeleteByKey<T>(string key)
        {
            _keyTypes.Remove(key);
            _store.Remove(key);
            return Task.CompletedTask;
        }

        public Task DeleteAll<T>()
        {
            var keys = _keyTypes
                .Where(x => x.Value == typeof(T).ToString())
                .Select(x => x.Key);

            keys.ForEach(x =>
            {
                _keyTypes.Remove(x);
                _store.Remove(x);
            });

            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _store.Clear();
            _keyTypes.Clear();
            return Task.CompletedTask;
        }

        public Task Flush()
        {
            throw new NotImplementedException();
        }

        private Task SaveKeyTypes()
        {
            _store.Add("KeyTypes", _keyTypes);
            return Task.CompletedTask;
        }
    }
}
