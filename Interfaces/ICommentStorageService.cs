using EnvManager.Models;

namespace EnvManager.Interfaces
{
    public interface ICommentStorageService
    {
        Dictionary<string, string> ReadComments();
        void SetComment(EnvironmentVariable variable);
    }
}