using EnvManager.Common;
using EnvManager.Interfaces;
using Microsoft.Extensions.Options;
using System.IO;
using System.Windows;

namespace EnvManager.Services;

public class CommentMonitorService : IDisposable, ICommentMonitorService
{
    private FileSystemWatcher _watcher;
    private readonly string _filePath;

    public event Action FileChanged;

    public CommentMonitorService(IOptions<FileSettings> options)
    {
        // Файл с комментариями хранится локально в той же папке, что и приложение
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var fileName = options.Value.CommentStorageFileName;
        _filePath = Path.Combine(appDirectory, fileName);

        _watcher = new FileSystemWatcher(appDirectory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true
        };

        _watcher.Changed += (s, e) => Application.Current.Dispatcher.Invoke(() => FileChanged?.Invoke());
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}