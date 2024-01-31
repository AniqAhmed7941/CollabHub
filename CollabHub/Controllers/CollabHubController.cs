using CollabHub.Interfaces;
using CollabHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CollabHub.Controllers
{
    /*
    public class CollabHubController : Controller
    {

        private readonly string _connectionString = "Data Source=.;Initial Catalog=CollabHub;Integrated Security=True; TrustServerCertificate=True\r\n";

        public IActionResult Index()
        {
            return View();
        }

        public int insertUser(string Name, string email)
        {
            int res = -2;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM USERS WHERE EMAIL = @Email";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return res;
                        }
                        else
                        {
                            string query = "INSERT INTO USERS (Name, Email, Status, Joined_at) VALUES " +
                                "(@Name, @Email, 1, GETDATE())";

                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                command2.Parameters.AddWithValue("@Name", Name);
                                command2.Parameters.AddWithValue("@PhoneNumber", email);

                                res = command.ExecuteNonQuery();
                            }
                        }
                    }

                }

            }

            return res;
        }
    }

    */

    public class CollabHubController : Controller
    {
        private readonly IDbOperations _dbOperations;

        public IActionResult Index()
        {
            return View();
        }
        public CollabHubController(IDbOperations dbOperations)
        {
            _dbOperations = dbOperations ?? throw new ArgumentNullException(nameof(dbOperations));
        }

        public CollabHubController()
        {
        }

        public int InsertUser(string name, string email)
        {
            int res = _dbOperations.InsertUser(name, email);

            // Process result if needed

            return res;
        }

        public bool UpdateUser(int userId, string newusername)
        {
            bool res = _dbOperations.UpdateUser(userId, newusername);

            return res;
        }
        public bool ValidateUsername(string username)
        {
            // Check the length of the username
            if (username.Length < 5 || username.Length > 15)
            {
                return false;
            }

            // Check if it starts and ends with a letter or number
            if (!char.IsLetterOrDigit(username[0]) || !char.IsLetterOrDigit(username[username.Length - 1]))
            {
                return false;
            }

            // Check if the username contains only allowed characters
            Regex regex = new Regex("^[a-zA-Z0-9_@.-]+$");

            return regex.IsMatch(username);
        }
        public int ValidateRole(int userId) {

            int userrole = _dbOperations.getUserRole(userId);

            return userrole; 
        }
        public bool CheckLogin(string username, string password)
        {
            bool res = _dbOperations.ValidateCredentials(username, password);

            return res;
        }


        public int CreateProject(string name)
        {
            int res = _dbOperations.CreateProject(name);

            return res;
        }

        public int CreateProjectWithDescription(string name, string description)
        {
            int res = _dbOperations.CreateProject(name, description);

            return res;
        }

        public int UpdateProjectStatus(string name, bool status)
        {
            int res = _dbOperations.UpdateProjectStatus(name, status);

            return res;

        }

        public bool UpdateProjectTime(int projectId)
        {
            /*
            List<Tasks> tasks = GetTasks(projectId);
            int noOfEasyTasks = tasks.Count(task => task.difficulty == 1);
            int noOfInterTasks = tasks.Count(task => task.difficulty == 2);
            int noOfHardTasks = tasks.Count(task => task.difficulty == 3);
            */

            int noOfEasyTasks = 10;
            int noOfInterTasks = 10;
            int noOfHardTasks = 10;

            TimeSpan est = CalculateProjectEstimatedTime(noOfEasyTasks,noOfInterTasks,noOfHardTasks);

            bool res = _dbOperations.UpdateProjectTime(projectId, est);
            
            return res;
        }

        public bool UpdateProjectName(int projectId, string newProjectName)
        {
            bool res = _dbOperations.UpdateProjectName(projectId, newProjectName);

            return res;
        }


        public bool AddUserProject(int userId, int projectId)
        {
            bool res = _dbOperations.AddUserProject(userId, projectId);

            return res;
        }

        public bool RemoveUserProject(int userId, int projectId)
        {
            bool res = _dbOperations.RemoveUserProject(userId, projectId);

            return res;

        }


        public bool AddTask(int userId, int projectId, string description, int difficulty)
        {
            bool res = _dbOperations.AddTask(userId, projectId, description, difficulty);

            if(res)
            {
                UpdateProjectTime(projectId);
            }
            return res;

        }

        public List<Tasks> GetTasks(int projectId)
        {
            List<Tasks> res = _dbOperations.GetTasks(projectId);

            return res;

        }

        public bool UpdateTaskStatus(int taskId, bool status)
        {
            bool res = _dbOperations.UpdateTaskStatus(taskId, status);

            return res;

        }
        public bool UpdateTask(int newuserId, int taskId)
        {
            bool res = _dbOperations.UpdateTask(newuserId, taskId);

            return res;
        }

        public bool UpdateTaskPriority(int taskId, int priority)
        {
            bool res = _dbOperations.UpdateTaskPriority(taskId, priority);

            int projectId = _dbOperations.GetProjectId(taskId);

            if (res)
            {
                UpdateProjectTime(projectId);
            }

            return res;

        }


        public TimeSpan CalculateProjectEstimatedTime(int noOfEasyTasks , int noOfInterTasks, int noOfHardTasks)
        {
            const int ThresholdForAdditionalTime = 25;
            const int DifficultyFactorEasy = 1;
            const int DifficultyFactorIntermediate = 2;
            const int DifficultyFactorHard = 3;

            TimeSpan est = TimeSpan.Zero;

            TimeSpan baseTime = TimeSpan.FromDays(7);
            TimeSpan difficultyFactor = TimeSpan.FromHours(5);
            TimeSpan noOfTaskFactor = TimeSpan.FromDays(1);

            int noOfTasks = noOfEasyTasks + noOfInterTasks + noOfHardTasks;

            TimeSpan easy = (difficultyFactor * DifficultyFactorEasy * noOfEasyTasks);
            TimeSpan inter = (difficultyFactor * DifficultyFactorIntermediate * noOfInterTasks);
            TimeSpan hard = (difficultyFactor * DifficultyFactorHard * noOfHardTasks);

            est = baseTime + easy + inter + hard;

            if (noOfTasks > ThresholdForAdditionalTime)
            {
                est += noOfTaskFactor * (noOfTasks - ThresholdForAdditionalTime) / ThresholdForAdditionalTime;
            }

            return est;
        }


        public List<Notification> SendNotification(int projectId, string message)
        {
            List<int> users = _dbOperations.GetUserList(projectId);

            List<Notification> notification = new List<Notification>();

            foreach (int userId in users)
            {
                notification.Add(new Notification
                {
                    userId = userId,
                    message = message
                });

                SendNotificationToUser(userId, message);

            }

            return notification;

        }
        public void SendNotificationToUser(int userId, string message)
        {

            //Implement to send notifications

            bool notificationSent = _dbOperations.SendNotification(userId, message);

        }

    }

}
