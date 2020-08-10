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
    public class LocalStorage
    {
        public static IStorage SecureStorage { get; private set; }
        public static IStorage LocalMachineStorage { get; private set; }

        public static void Initialise(string applicationName)
        {
            BlobCache.ApplicationName = applicationName;

            BlobCache.EnsureInitialized();

            SecureStorage = new BlobCacheStorage<IBlobCache>(BlobCache.Secure);
            LocalMachineStorage = new BlobCacheStorage<IBlobCache>(BlobCache.LocalMachine);
        }

        public static async Task FlushAll()
        {
            await SecureStorage?.Flush();
            await LocalMachineStorage?.Flush();
        }

        public static async Task Shutdown()
        {
            await BlobCache.Shutdown();
        }
    }
}
