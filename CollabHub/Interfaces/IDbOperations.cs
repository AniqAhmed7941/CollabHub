using CollabHub.Models;

namespace CollabHub.Interfaces
{
    public interface IDbOperations
    {
        bool UserExists(string email);
        int InsertUser(string name, string email);

        bool ProjectExists(string name);
        int CreateProject(string name, string description = "");
        int UpdateProjectStatus(string name, bool status);
        bool UpdateProjectTime(int Id, TimeSpan estimateTime);

        bool AddUserProject(int userId, int projectId);
        bool UserProjectExists(int userId, int projectId);
        bool RemoveUserProject(int userId, int projectId);

        bool AddTask(int userId, int projectId, string description, int difficulty);
        bool UpdateTaskStatus(int taskId, bool status);
        List<Tasks> GetTasks(int projectId);

        List<int> GetUserList(int projectId);
        bool SendNotification(int userId, string message);
        bool UpdateTaskPriority(int taskId, int priority);
        int GetProjectId(int taskId);
        int getUserRole(int userId);
        bool ProjectExists(int projectId);
        bool UpdateProjectName(int projectId, string newProjectName);
        bool ValidateCredentials(string username, string password);
        bool UpdateUser(int userId, string newusername);
        bool TaskExists(int taskId);
        bool UpdateTask(int newuserId, int taskId);
    }
}
