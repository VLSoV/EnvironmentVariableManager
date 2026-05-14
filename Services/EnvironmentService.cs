using EnvManager.Models;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace EnvManager.Services;

public class EnvironmentService(
    ILogger<EnvironmentService> logger,
    List<string> variableNames)
{
    // Для оповещения системы об изменении среды
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
        uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

    private const uint HWND_BROADCAST = 0xffff;
    private const uint WM_SETTINGCHANGE = 0x001A;
    private const uint SMTO_ABORTIFHUNG = 0x0002;
    private const uint TIMEOUT = 5000;

    /// <summary>
    /// Читает значения пользовательских переменных среды.
    /// Если переменная не существует, возвращает пустую строку (значение по умолчанию).
    /// </summary>
    public Dictionary<string, string> ReadVariables()
    {
        var result = new Dictionary<string, string>();
        foreach (var name in variableNames)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            result[name] = value ?? string.Empty;
        }
        logger.LogInformation("Переменные среды прочитаны");
        return result;
    }

    /// <summary>
    /// Устанавливает значение одной переменной среды и пишет в лог.
    /// </summary>
    public void SetVariable(EnvironmentVariable variable)
    {
        var name = variable.Name;
        var newValue = variable.Value ?? string.Empty;

        if (!variableNames.Contains(name))
            return;

        var oldValue = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User) ?? string.Empty;

        Environment.SetEnvironmentVariable(name, newValue, EnvironmentVariableTarget.User);
        NotifyEnvironmentChange();

        logger.LogInformation("Variable '{Name}' changed: '{OldValue}' -> '{NewValue}'", name, oldValue, newValue);
    }

    private void NotifyEnvironmentChange()
    {
        SendMessageTimeout(
            (IntPtr)HWND_BROADCAST,
            WM_SETTINGCHANGE,
            UIntPtr.Zero,
            "Environment",
            SMTO_ABORTIFHUNG,
            TIMEOUT,
            out _);
    }
}