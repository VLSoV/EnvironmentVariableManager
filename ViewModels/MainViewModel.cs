using EnvManager.Models;
using EnvManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EnvManager.ViewModels;

public class MainViewModel : ViewModel
{
    private readonly EnvironmentService _envService;
    private readonly CommentStorageService _commentService;

    public ObservableCollection<EnvironmentVariable> Variables { get; } = new();

    public MainViewModel(EnvironmentService envService, CommentStorageService commentService)
    {
        _envService = envService;
        _commentService = commentService;

        LoadData();
    }

    public void LoadData()
    {
        var values = _envService.ReadVariables();
        var comments = _commentService.ReadComments();

        // Отписываемся от старых элементов
        foreach (var item in Variables)
            item.PropertyChanged -= OnItemPropertyChanged;

        Variables.Clear();
        foreach (var kvp in values)
        {
            var vm = new EnvironmentVariable
            {
                Name = kvp.Key,
                Value = kvp.Value,
                Comment = comments.TryGetValue(kvp.Key, out var c) ? c : string.Empty
            };
            vm.PropertyChanged += OnItemPropertyChanged;
            Variables.Add(vm);
        }
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is EnvironmentVariable variable)
        {
            switch (e.PropertyName)
            {
                case nameof(EnvironmentVariable.Value):
                    _envService.SetVariable(variable);
                    break;

                case nameof(EnvironmentVariable.Comment):
                    _commentService.SetComment(variable);
                    break;
            }
        }
    }
}