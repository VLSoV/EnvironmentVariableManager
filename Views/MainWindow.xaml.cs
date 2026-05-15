using EnvManager.Services;
using EnvManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace EnvManager.Views;

public partial class MainWindow : Window
{
    private readonly IServiceScope _scope;
    public MainWindow(IServiceProvider serviceProvider,
        MainViewModel mainViewModel)
    {
        InitializeComponent();

        // Создаем scope для времени жизни окна
        _scope = serviceProvider.CreateScope();

        // Создаем монитор и связываем с главным окном
        var monitor = _scope.ServiceProvider.GetRequiredService<EnvironmentMonitorService>();
        monitor.Initialize(this);

        DataContext = mainViewModel;
    }

    // Обработчики для кастомного окна

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    protected override void OnClosed(EventArgs e)
    {
        // Срабатывает во всех сервисах IDisposable
        _scope?.Dispose(); 
        base.OnClosed(e);
    }
}