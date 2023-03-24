using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

using SignatureValidation.Helpers;

using SignatureValidation.Pages;
using SignatureValidation.Services;
using SignatureValidation.ViewModels;

namespace SignatureValidation.Extensions;

public static class AddServicesExtension
{
    /// <summary>
    /// Add View Models to DI Container
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder.ConfigureServices(services =>
        {
            _ = services.AddSingleton<BaseViewModel>();
            _ = services.AddSingleton<MainWindowViewModel>();
            _ = services.AddSingleton<ResultManagementViewModel>();
            _ = services.AddSingleton<RepoManagementViewModel>();
        });

        return hostBuilder;
    }

    /// <summary>
    /// Add Helpers & Other Services to DI Container
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder AddHelpers(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder.ConfigureServices(services =>
        {
            _ = services.AddSingleton<FileHelper>();
            _ = services.AddSingleton<OpenFileDialog>();
            _ = services.AddSingleton<SaveFileDialog>();
            _ = services.AddSingleton<DataAccessService>();
        });

        return hostBuilder;
    }

    /// <summary>
    /// Add UI Page, Windows & Controls to DI Container
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder AddPages(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder.ConfigureServices(services =>
        {
            _ = services.AddSingleton<MainWindow>();
            _ = services.AddSingleton<ResultManagementControl>();
            _ = services.AddSingleton<RepoManagementControl>();
        });
        return hostBuilder;
    }

    /// <summary>
    /// Add App Settings to DI Container
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static IHostBuilder AddConfiguration(this IHostBuilder host)
    {
        _ = host.ConfigureAppConfiguration(c =>
        {
            _ = c.AddJsonFile("appsettings.json");
            _ = c.AddEnvironmentVariables();
        });

        return host;
    }
}