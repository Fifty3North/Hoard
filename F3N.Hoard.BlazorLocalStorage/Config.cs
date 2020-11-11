namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlazorLocalStorageExtensions
    {
        public static void AddBlazorLocalStorage(this IServiceCollection services) 
        {
            services.AddProtectedBrowserStorage();
        }
    }
}
