using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SignatureValidation.Extensions;

using System.Windows;
using System.Windows.Navigation;

namespace SignatureValidation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }
    public static NavigationService? NavigationService { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .AddViewModels()
            .AddHelpers()
            .AddPages()
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();
        // Get a reference to the NavigationService that navigated to this Page
        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        NavigationService = NavigationService.GetNavigationService(startupForm);
        startupForm.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }
}