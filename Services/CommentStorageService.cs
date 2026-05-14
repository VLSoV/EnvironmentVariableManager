using EnvManager.Models;
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
            // Если файл поврежден, возвращаем пустой словарь
            _logger.LogError("Файл с комментариями {CommentsFilePath} поврежден", _commentsFilePath);
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Сохраняет новый комментарий в локальный JSON-файл
    /// </summary>
    public void SetComment(EnvironmentVariable variable)
    {
        var name = variable.Name;
        var newComment = variable.Comment ?? string.Empty;

        var comments = ReadComments();
        comments[name] = newComment;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(comments, options);
        File.WriteAllText(_commentsFilePath, json);
        _logger.LogInformation("Комментарий {NewComment} к переменной среды {Name} записан", newComment, name);
    }
}