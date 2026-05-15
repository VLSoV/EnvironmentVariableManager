using EnvManager.Models;

namespace EnvManager.Interfaces
{
    public interface IEnvironmentService
    {
        Dictionary<string, string> ReadVariables();
        void SetVariable(EnvironmentVariable variable);
    }
}