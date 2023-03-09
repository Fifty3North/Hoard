using F3N.Hoard;
using F3N.Hoard.BlazorWasmStorage;
using Hoard.SampleLogic.Counter;
using Hoard.SampleLogic.Forecast;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hoard.SampleWASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ForecastStore>();
            builder.Services.AddScoped<CounterStore>();

            // Required for Blazor Server Protected Storage
            builder.Services.AddScoped<IStorage, BrowserStorage>();
            await builder.Build().RunAsync();
        }
    }
}
