using EnvManager.ViewModels;

namespace EnvManager.Models;

public class EnvironmentVariable : ViewModel
{
    private string _name;
    private string _value;
    private string _comment;

    public string Name
    {
        get => _name;
        set => Set(ref _name, value);
    }

    public string Value
    {
        get => _value;
        set => Set(ref _value, value);
    }

    public string Comment
    {
        get => _comment;
        set => Set(ref _comment, value);
    }
}