using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3N.Hoard.Sqlite
{
    
    public class SqliteConfig
    {
        public static IStorage Storage { get; private set; }
        public static IBlobCache Database { get; private set; }
        public static void Initialise(string applicationName, Func<IBlobCache> blobCacheResolver)
        {
            BlobCache.ApplicationName = applicationName;
            BlobCache.EnsureInitialized();
            Database = blobCacheResolver.Invoke();
            Storage = new BlobCacheStorage(Database);
        }

        public static async Task FlushAll()
        {
            await ((IBlobCache)Storage).Flush();
        }

        public static async Task Shutdown()
        {
            await BlobCache.Shutdown();
        }
    }
}
