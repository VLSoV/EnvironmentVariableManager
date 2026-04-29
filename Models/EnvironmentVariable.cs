using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnvManager.Models;

public class EnvironmentVariable : INotifyPropertyChanged
{
    private string _name;
    private string _value;
    private string _comment;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public string Value
    {
        get => _value;
        set { _value = value; OnPropertyChanged(); }
    }

    public string Comment
    {
        get => _comment;
        set { _comment = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}