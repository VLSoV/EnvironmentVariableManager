using EnvManager.Common;
using EnvManager.Interfaces;
using EnvManager.Services;
using EnvManager.ViewModels;
using EnvManager.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using System.Windows;

namespace EnvManager;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Настройка Serilog
        var logFile = Path.Combine("logs", "test-sms-wpf-app-.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Чтение конфигурации
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var variableNames = configuration.GetSection("EnvironmentVariables").Get<List<string>>() ?? new List<string>();

        // Настройка DI
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddSerilog(dispose: true));
        services.AddSingleton(variableNames);
        services.AddScoped<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<IEnvironmentMonitorService, EnvironmentMonitorService>();
        services.AddScoped<ICommentStorageService, CommentStorageService>();
        services.AddScoped<ICommentMonitorService, CommentMonitorService>();
        services.AddScoped<MainViewModel>();
        services.AddScoped<MainWindow>();

        services.Configure<FileSettings>(configuration.GetSection(nameof(FileSettings)));

        ServiceProvider = services.BuildServiceProvider();

        // Запуск главного окна
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}