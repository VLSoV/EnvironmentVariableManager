using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace EnvManager.Services;

public class CommentStorageService
{
    private readonly string _commentsFilePath;
    private readonly ILogger<EnvironmentService> _logger;
    public CommentStorageService(ILogger<EnvironmentService> logger)
    {
        // Файл с комментариями хранится локально в той же папке, что и приложение
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _commentsFilePath = Path.Combine(appDirectory, "env_comments.json");

        _logger = logger;
    }

    /// <summary>
    /// Читает все комментарии из локального JSON-файла
    /// </summary>
    public Dictionary<string, string> ReadComments()
    {
        if (!File.Exists(_commentsFilePath))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            var json = File.ReadAllText(_commentsFilePath);
            _logger.LogInformation("Комментарии к переменным среды прочитаны");
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                   ?? new Dictionary<string, string>();
        }
        catch (Exception)
        {
            // Если файл не найден или поврежден, возвращаем пустой словарь
            _logger.LogInformation("Файл с комментариями не найден или поврежден");
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Сохраняет все комментарии в локальный JSON-файл
    /// </summary>
    public void SaveAllComments(Dictionary<string, string> comments)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(comments, options);
        File.WriteAllText(_commentsFilePath, json);
        _logger.LogInformation("Комментарии к переменным среды записаны");
    }
}