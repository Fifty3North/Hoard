using Akavache;
using F3N.Hoard.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using F3N.Hoard.Shared;


namespace F3N.Hoard.Storage
{
    public class BlobCacheStorage<TBlobCache> : IStorage
            where TBlobCache : IBlobCache
    {
        private readonly TBlobCache _db;

        public BlobCacheStorage(TBlobCache db)
        {
            _db = db;
        }

        public async Task<T> Get<T>(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string key = Utility.MakeKey<T>(id);
                return await GetByKey<T>(key);
            }

            return default(T);
        }

        public async Task<T> GetByKey<T>(string key)
        {
            try
            {
                return await _db.GetObject<T>(key);
            }
            catch (KeyNotFoundException ex)
            {
                HandleException(ex, "GetByKey (key not found)", false);
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetByKey");
            }

            return default(T);
        }

        public async Task<List<T>> GetAll<T>()
        {
            try
            {
                IEnumerable<T> objects = await _db.GetAllObjects<T>();
                return objects.ToList();
            }
            catch (JsonSerializationException ex)
            {
                HandleException(ex, "GetAll (Json serialization");
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetAll");
            }

            return new List<T>();
        }

        public async Task Save<T>(string id, T data)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string key = Utility.MakeKey<T>(id);
                await SaveByKey<T>(key, data);
            }
        }

        public async Task SaveByKey<T>(string key, T data)
        {
            if (data != null && !string.IsNullOrEmpty(key))
            {
                try
                {
                    await _db.InsertObject(key, data);
                }
                catch (Exception ex)
                {
                    HandleException(ex, "SaveByKey");
                }
            }
        }

        public async Task SaveAllByKey<T>(Dictionary<string, T> data)
        {
            if (data != null && data.Count > 0)
            {
                try
                {
                    await _db.InsertAllObjects(data);
                }
                catch (Exception ex)
                {
                    HandleException(ex, "SaveAllByKey");
                }
            }
        }

        public async Task Delete<T>(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string key = Utility.MakeKey<T>(id);
                await DeleteByKey<T>(key);
            }
        }

        public async Task DeleteByKey<T>(string key)
        {
            try
            {
                await _db.InvalidateObject<T>(key);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DeleteByKey");
            }
        }

        public async Task DeleteAll<T>()
        {
            try
            {
                await _db.InvalidateAllObjects<T>();
            }
            catch (Exception ex)
            {
                HandleException(ex, "DeleteAll");
            }
        }

        public async Task Clear()
        {
            try
            {
                await _db.InvalidateAll();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Clear");
            }
        }

        public async Task Flush()
        {
            try
            {
                await _db?.Flush();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Flush");
            }
        }

        private void HandleException(Exception ex, string message = null, bool diagnosticWorthy = true)
        {
            System.Diagnostics.Debug.WriteLine($"Error in BlobCacheStorage ({_db.GetType().Name}) {message}: {ex}");
        }
    }
}
