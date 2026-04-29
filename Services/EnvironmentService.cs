using EnvManager.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace EnvManager.Services;

public class EnvironmentService(
    ILogger<EnvironmentService> logger,
    CommentStorageService commentService,
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
    /// Загружает значения переменных среды и комментарии к ним
    /// </summary>
    public void LoadVariables(ObservableCollection<EnvironmentVariable> variableCollection)
    {
        var values = ReadVariables();
        var comments = commentService.ReadComments();

        variableCollection.Clear();
        foreach (var kvp in values)
        {
            variableCollection.Add(new EnvironmentVariable
            {
                Name = kvp.Key,
                Value = kvp.Value,
                Comment = comments.ContainsKey(kvp.Key) ? comments[kvp.Key] : string.Empty
            });
        }
        logger.LogInformation("Переменные среды загружены");
    }

    /// <summary>
    /// Сохраняет значения переменных среды и комментарии к ним
    /// </summary>
    public void SaveVariables(ObservableCollection<EnvironmentVariable> variableCollection)
    {
        var newValues = variableCollection.ToDictionary(v => v.Name, v => v.Value ?? string.Empty);
        var newComments = variableCollection.ToDictionary(v => v.Name, v => v.Comment ?? string.Empty);

        WriteAll(newValues, newComments);

        commentService.SaveAllComments(newComments);
    }

    /// <summary>
    /// Читает значения пользовательских переменных среды для заданного набора имён.
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
    /// Устанавливает значения переменных среды и фиксирует изменения в лог.
    /// </summary>
    public void WriteAll(Dictionary<string, string> newValues, Dictionary<string, string> comments)
    {
        var oldValues = ReadVariables();

        foreach (var kvp in newValues)
        {
            var name = kvp.Key;
            var newValue = kvp.Value ?? string.Empty;
            Environment.SetEnvironmentVariable(name, newValue, EnvironmentVariableTarget.User);
        }

        // Оповещение системы
        NotifyEnvironmentChange();

        // Формирование сообщения в лог
        var sb = new StringBuilder();
        sb.AppendLine("\n=== Переменные среды успешно изменены ===");
        foreach (var name in variableNames)
        {
            var oldVal = oldValues.ContainsKey(name) ? oldValues[name] : "<none>";
            var newVal = newValues.ContainsKey(name) ? newValues[name] : "<removed>";

            sb.AppendLine($"Переменная: {name}");
            sb.AppendLine($"Значение: '{oldVal}' -> '{newVal}'");

            // Логируем комментарий, если он есть
            if (comments != null && comments.ContainsKey(name))
            {
                sb.AppendLine($"Комментарий: '{comments[name]}'");
            }
            sb.AppendLine();
        }
        logger.LogInformation("{Message}", sb.ToString());
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