using System.Windows;

namespace EnvManager.Interfaces
{
    public interface IEnvironmentMonitorService
    {
        event Action EnvironmentChanged;

        void Initialize(Window window);
    }
}