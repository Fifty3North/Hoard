using Akavache;
using F3N.Hoard;
using F3N.Hoard.Sqlite;
using Maui.Hoard;
using Maui.ViewModels;
using Microsoft.Extensions.Logging;
using System.Security.AccessControl;

namespace Maui;

public static class MauiProgram
{
    private static string _applicationName = "MauiSample";

    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.RegisterServices();
		builder.RegisterViewModels();
        builder.RegisterPages();

        return builder.Build();
	}

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        //BlobCache.ApplicationName = _applicationName;
        SqliteConfig.Initialise(_applicationName, () => BlobCache.Secure);
        mauiAppBuilder.Services.AddSingleton<IStorage, BlobCacheStorage>();
        mauiAppBuilder.Services.AddSingleton<CounterStore>();
        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<CounterViewModel>();
        return mauiAppBuilder;
    }
    public static MauiAppBuilder RegisterPages(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<MainPage>();
        return mauiAppBuilder;
    }
}
