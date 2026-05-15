using System.IO;
using System.Windows;

namespace EnvManager.Services;

public class CommentMonitorService : IDisposable
{
    private FileSystemWatcher _watcher;
    private readonly string _filePath;

    public event Action FileChanged;

    public CommentMonitorService()
    {
        // Файл с комментариями хранится локально в той же папке, что и приложение
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(appDirectory, "env_comments.json");

        EnsureDirectoryExists();
        string directory = Path.GetDirectoryName(_filePath);
        string fileName = Path.GetFileName(_filePath);

        _watcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += (s, e) => Application.Current.Dispatcher.Invoke(() => FileChanged?.Invoke());
    }

    private void EnsureDirectoryExists()
    {
        string directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}