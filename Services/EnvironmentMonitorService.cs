using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace EnvManager.Services;

/// <summary>
/// Сервис отслеживания изменений переменных среды за пределами приложения
/// </summary>
public class EnvironmentMonitorService : IDisposable
{
    private const int WM_SETTINGCHANGE = 0x001A;

    // Окно, которое будет слушать сообщения ОС
    private Window _targetWindow; 
    private HwndSource _hwndSource;
    private bool _isDisposed;

    public event Action EnvironmentChanged;

    public void Initialize(Window window)
    {
        if (_targetWindow != null)
            throw new InvalidOperationException("Окно уже выбрано!");

        _targetWindow = window;
        _targetWindow.SourceInitialized += OnSourceInitialized;
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
        _hwndSource = PresentationSource.FromVisual(_targetWindow) as HwndSource;
        if (_hwndSource != null)
        {
            _hwndSource.AddHook(new HwndSourceHook(WndProc));
        }
    }

    // Основной обработчик оконных сообщений
    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_SETTINGCHANGE)
        {
            // lParam может указывать на строку с именем изменившейся области
            string changedArea = Marshal.PtrToStringAuto(lParam);

            // Проверяем, что это именно уведомление об изменении переменных среды
            if (string.Equals(changedArea, "Environment", StringComparison.OrdinalIgnoreCase))
            {
                // Запускаем событие в потоке UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EnvironmentChanged?.Invoke();
                });
            }
        }
        return IntPtr.Zero;
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        if (_hwndSource != null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }

        if (_targetWindow != null)
        {
            _targetWindow.SourceInitialized -= OnSourceInitialized;
            _targetWindow.Closed -= OnWindowClosed;
            _targetWindow = null;
        }

        _isDisposed = true;
    }
}