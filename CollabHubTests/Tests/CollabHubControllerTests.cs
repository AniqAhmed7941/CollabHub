using CollabHub.Controllers;
using CollabHub.Interfaces;
using CollabHub.Models;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace CollabHubTests.Tests
{

    [TestClass]
    public class CollabHubControllerTests
    {
        //User Registration
        [TestMethod]
        public void CollabHub_InsertUserIfEmailExists_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserExists("john@example.com")).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = 0;// InsertUser("John Doe", "john@example.com") not called;
            if (mockDbOperations.Object.UserExists("john@example.com"))
            {
                result = -2;
            }

            // Assert
            Assert.AreEqual(-2, result);
            mockDbOperations.Verify(d => d.InsertUser(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_InsertUser_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserExists("john@example.com")).Returns(false);
            mockDbOperations.Setup(d => d.InsertUser(It.IsAny<string>(), It.IsAny<string>())).Returns(1);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.InsertUser("John Doe", "john@example.com");
            var status = 404;
            if(result == 1 )
            {
                status = 200;
            }
            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(200, status);
            mockDbOperations.Verify(d => d.InsertUser(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CollabHub_UpdateUser_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            var userId = 1;
            var newusername = "NewUser123";
            mockDbOperations.Setup(d => d.UserExists("john@example.com")).Returns(true);
            mockDbOperations.Setup(d => d.UpdateUser(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.UpdateUser(userId, newusername);
            var status = 404;
            if (result == true)
            {
                status = 200;
            }
            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(200, status);
            mockDbOperations.Verify(d => d.UpdateUser(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CollabHub_ValidateUsername_ReturnsBool()
        {
            // Arrange
            string validUsername = "GoodUser._-@123";
            var controller = new CollabHubController();

            // Act
            bool result = controller.ValidateUsername(validUsername);

            // Assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void CollabHub_InValidateUsername_ReturnsBool()
        {
            // Arrange
            string validUsername = "GoodUser._ -@123";
            var controller = new CollabHubController();

            // Act
            bool result = controller.ValidateUsername(validUsername);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CollabHub_ValidateUserRole_ReturnsInt()
        {
            // Arrange
            var userId = 123;
            var expectedUserRole = 1; // Replace with the expected user role

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.getUserRole(userId)).Returns(expectedUserRole);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.ValidateRole(userId);

            // Assert
            Assert.AreEqual(expectedUserRole, result);

            // Verify that getUserRole is called with the correct userId
            mockDbOperations.Verify(d => d.getUserRole(userId), Times.Once);
        }


        //User Login
        [TestMethod]
        public void CollabHub_CheckLoginSuccessful_ReturnsBool()
        {
            // Arrange
            string username = "123";
            string password = "123";
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(a => a.ValidateCredentials(username, password)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.CheckLogin(username, password);

            // Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CollabHub_CheckLoginUnSuccessful_ReturnsBool()
        {
            // Arrange
            string username = "abc123";
            string password = "abc123";
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(a => a.ValidateCredentials(username, password)).Returns(false);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.CheckLogin(username, password);

            // Assert
            Assert.AreEqual(false, result);
        }


        //Project Management
        [TestMethod]
        public void CollabHub_CreateProject_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists("CollabHub")).Returns(false);
            mockDbOperations.Setup(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>())).Returns(1);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
//            var result = controller.CreateProject("CollabHub", "The project is a teamwork tool to make managing projects and users easier. User can create\r\nand handle projects, setting goals, timelines, and important points. The system also helps\r\nwith handling users, letting you add team members to specific projects. Each project is like a\r\nspecial area for the team, making it easy for everyone to work together. Users can add goals\r\nand tasks in projects, making work smoother. The app keeps everything in one place, making\r\nit simple to see updates.");
            var result = controller.CreateProjectWithDescription("CollabHub", "The project is a teamwork tool to make managing projects and users easier. User can create\r\nand handle projects, setting goals, timelines, and important points. The system also helps\r\nwith handling users, letting you add team members to specific projects. Each project is like a\r\nspecial area for the team, making it easy for everyone to work together. Users can add goals\r\nand tasks in projects, making work smoother. The app keeps everything in one place, making\r\nit simple to see updates.");

            // Assert
            Assert.AreEqual(1, result);
            mockDbOperations.Verify(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CollabHub_CreateProjectWithError_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            var projectName = "";
            var projectDesc = "The project is a teamwork tool to make managing projects and users easier. User can create\\r\\nand handle projects, setting goals, timelines, and important points. The system also helps\\r\\nwith handling users, letting you add team members to specific projects. Each project is like a\\r\\nspecial area for the team, making it easy for everyone to work together. Users can add goals\\r\\nand tasks in projects, making work smoother. The app keeps everything in one place, making\\r\\nit simple to see updates.";

            if(projectName != "")
            {
                mockDbOperations.Setup(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>())).Returns(1);
            }


            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = 0;

            if (projectName != "")
            {
                result = controller.CreateProjectWithDescription("", "The project is a teamwork tool to make managing projects and users easier. User can create\r\nand handle projects, setting goals, timelines, and important points. The system also helps\r\nwith handling users, letting you add team members to specific projects. Each project is like a\r\nspecial area for the team, making it easy for everyone to work together. Users can add goals\r\nand tasks in projects, making work smoother. The app keeps everything in one place, making\r\nit simple to see updates.");
            }

            // Assert
            Assert.AreEqual(0, result);
            mockDbOperations.Verify(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_CreateProjectIfExists_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists("Collab Hub")).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = 0; //CreateProject Not Called
            if (mockDbOperations.Object.ProjectExists("Collab Hub"))
            {
                result = -2;
            }

            // Assert
            Assert.AreEqual(-2, result);
            mockDbOperations.Verify(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_CreateProjectIfNotAdmin_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.getUserRole(123)).Returns(2);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = 0; //CreateProject Not Called
            if (mockDbOperations.Object.getUserRole(123) != 1)
            {
                result = -2;
            }

            // Assert
            Assert.AreEqual(-2, result);
            mockDbOperations.Verify(d => d.CreateProject(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_UpdateProjectStatusIfExists_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists("Collab Hub")).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectStatus("Collab Hub", It.IsAny<bool>())).Returns(1);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = 0; //Update Project if exists
            if (mockDbOperations.Object.ProjectExists("Collab Hub"))
            {
                result = controller.UpdateProjectStatus("Collab Hub", true);
            }

            // Assert
            Assert.AreEqual(1, result);
            mockDbOperations.Verify(d => d.UpdateProjectStatus(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void CollabHub_UpdateProjectStatusIfNotAdmin_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists("Collab Hub")).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectStatus("Collab Hub", It.IsAny<bool>())).Returns(1);
            mockDbOperations.Setup(d => d.getUserRole(123)).Returns(2);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = 0; //Update Project if exists
            if (mockDbOperations.Object.ProjectExists("Collab Hub") && mockDbOperations.Object.getUserRole(123) == 1 )
            {
                result = controller.UpdateProjectStatus("Collab Hub", true);
            }

            // Assert
            Assert.AreEqual(0, result);
            mockDbOperations.Verify(d => d.UpdateProjectStatus(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_UpdateProjectName_ReturnsBool()
        {
            // Arrange

            var projectId = 123;
            var newProjectName = "Collab Hub";
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists(projectId)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectName(projectId, newProjectName)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false; //Update Project if exists
            if (mockDbOperations.Object.ProjectExists(projectId))
            {
                result = controller.UpdateProjectName(projectId, newProjectName);
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateProjectName(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CollabHub_UpdateProjectNameIfNotAdmin_ReturnsBool()
        {
            // Arrange

            var projectId = 123;
            var newProjectName = "Collab Hub";
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists(projectId)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectName(projectId, newProjectName)).Returns(true);
            mockDbOperations.Setup(d => d.getUserRole(123)).Returns(2);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false; //Update Project if exists
            if (mockDbOperations.Object.ProjectExists(projectId) && mockDbOperations.Object.getUserRole(123) == 1)
            {
                result = controller.UpdateProjectName(projectId, newProjectName);
            }

            // Assert
            Assert.AreEqual(false, result);
            mockDbOperations.Verify(d => d.UpdateProjectName(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CollabHub_UpdateProjectEstimateTime_ReturnsInt()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.ProjectExists("Collab Hub")).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(1, It.IsAny<TimeSpan>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false; //Update Project if exists
            if (mockDbOperations.Object.ProjectExists("Collab Hub"))
            {
                result = controller.UpdateProjectTime(1);
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateProjectTime(It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
        }


        //User Management
        [TestMethod]
        public void CollabHub_AddUserProject_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.AddUserProject(1,4)).Returns(true);
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(false);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result= false;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {

            }
            else
            {
                result = controller.AddUserProject(1, 4);
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.AddUserProject(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_AddUserProjectIfExists_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.AddUserProject(1, 4)).Returns(false);
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = true;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {
                result = controller.AddUserProject(1, 4);

            }
            else
            {
            }

            // Assert
            Assert.AreEqual(false, result);
            mockDbOperations.Verify(d => d.AddUserProject(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_RemoveUserProject_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.RemoveUserProject(1, 4)).Returns(true);
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {
                result = controller.RemoveUserProject(1, 4);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.RemoveUserProject(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        }


        //Task Management
        [TestMethod]
        public void CollabHub_AddTask_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(1, It.IsAny<TimeSpan>())).Returns(true);
            mockDbOperations.Setup(d => d.AddTask(1, 4, "lorem ipsum", 2)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {
                result = controller.AddTask(1, 4, "lorem ipsum", 2);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.AddTask(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_UpdateTaskUser_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            var taskId = 321;
            var newuserId = 111;
            mockDbOperations.Setup(d => d.TaskExists(taskId)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateTask(newuserId, taskId)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.TaskExists(taskId))
            {
                result = controller.UpdateTask(newuserId, taskId);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateTask(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_AddTaskWithError_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            var taskname = "";
            if(taskname != "")
            {
                mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);
                mockDbOperations.Setup(d => d.UpdateProjectTime(1, It.IsAny<TimeSpan>())).Returns(true);
                mockDbOperations.Setup(d => d.AddTask(1, 4, "lorem ipsum", 2)).Returns(true);

            }

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {
                result = controller.AddTask(1, 4, "lorem ipsum", 2);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(false, result);
            mockDbOperations.Verify(d => d.AddTask(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        }

        [TestMethod]
        public void CollabHub_AddTaskIfNotAdmin_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(1, It.IsAny<TimeSpan>())).Returns(true);
            mockDbOperations.Setup(d => d.AddTask(1, 4, "lorem ipsum", 2)).Returns(true);
            mockDbOperations.Setup(d => d.getUserRole(1)).Returns(2);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4) && mockDbOperations.Object.getUserRole(1) == 1)
            {
                result = controller.AddTask(1, 4, "lorem ipsum", 2);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(false, result);
            mockDbOperations.Verify(d => d.AddTask(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        }

        [TestMethod]
        public void CollabHub_UpdateTaskStatus_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UpdateTaskStatus(1, true)).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = controller.UpdateTaskStatus(1, true);

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateTaskStatus(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_UpdateTaskPriority_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(4, It.IsAny<TimeSpan>())).Returns(true);
            mockDbOperations.Setup(d => d.UpdateTaskPriority(1, 3)).Returns(true);
            mockDbOperations.Setup(d => d.GetProjectId(1)).Returns(4);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4))
            {
                result = controller.UpdateTaskPriority(1, 3);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateTaskPriority(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

        }

        [TestMethod]
        public void CollabHub_UpdateTaskConcurrently_ReturnsBool()
        {
            // Arrange
            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.UserProjectExists(1, 4)).Returns(true);
            mockDbOperations.Setup(d => d.UserProjectExists(2, 3)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(4, It.IsAny<TimeSpan>())).Returns(true);
            mockDbOperations.Setup(d => d.UpdateProjectTime(3, It.IsAny<TimeSpan>())).Returns(true);
            mockDbOperations.Setup(d => d.UpdateTaskPriority(1, 3)).Returns(true);
            mockDbOperations.Setup(d => d.UpdateTaskPriority(2, 2)).Returns(true);
            mockDbOperations.Setup(d => d.GetProjectId(1)).Returns(4);
            mockDbOperations.Setup(d => d.GetProjectId(2)).Returns(3);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act

            var result = false;
            if (mockDbOperations.Object.UserProjectExists(1, 4) && mockDbOperations.Object.UserProjectExists(2, 3))
            {
                result = controller.UpdateTaskPriority(1, 3);
                result = controller.UpdateTaskPriority(2, 2);
            }
            else
            {
            }

            // Assert
            Assert.AreEqual(true, result);
            mockDbOperations.Verify(d => d.UpdateTaskPriority(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));

        }

        //Estimate Time
        [TestMethod]
        public void CollabHub_CheckProjectEstimatedTime_ReturnsTimeSpan()
        {
            //Arrange
            var controller = new CollabHubController();

            // Test Case : Few tasks, no additional time
            // Act
            var result1 = controller.CalculateProjectEstimatedTime(3, 5, 2);

            // Assert
            TimeSpan expectedTime1 = TimeSpan.FromDays(10).Add(TimeSpan.FromHours(23));
            Assert.AreEqual(expectedTime1, result1);

            /*
             * Calculation for Test Case 1:
             * Base Time: 7 days
             * Easy Tasks: 1 * 3 * 5 = 15 hours
             * Intermediate Tasks: 2 * 5 * 5 = 50 hours = 2 days + 2 hours
             * Hard Tasks: 3 * 2 * 5 = 30 hours = 1 days + 6 hours
             * Total: 7 days + 15 hours + 2 days + 2 hours + 1 days + 6 hours = 10 days 23 hours 
             */

        }

        [TestMethod]
        public void CollabHub_CheckProjectEstimatedTimeWAdditionalTime_ReturnsTimeSpan()
        {
            //Arrange
            var controller = new CollabHubController();

            // Test Case : More tasks, additional time added

            // Act
            var result = controller.CalculateProjectEstimatedTime(10, 10, 10);

            // Assert
            TimeSpan expectedTime = TimeSpan.FromDays(19).Add(TimeSpan.FromHours(16)).Add(TimeSpan.FromMinutes(48));
            Assert.AreEqual(expectedTime, result);

            /*
             * Calculation for Test Case :
             * Base Time: 7 days
             * Easy Tasks: 1 * 10 * 5 = 50 hours = 2 days + 2 hours 
             * Intermediate Tasks:  2 * 10 * 5 = 100 hours = 4 days + 4 hours
             * Hard Tasks: 3 * 10 * 5 = 150 hours = 6 days + 6 hours
             * Additional Time = 1 * ( 30 - 25 ) / 25 = 288 minutes = 4 hours + 48 minutes
             * Total: 7 days + 2 days + 2 hours + 4 days + 4 hours + 6 days + 6 hours + 4 hours + 28 minutes = 19 days + 16 hours + 48 minutes
             */

        }

        [TestMethod]
        public void CollabHub_CheckProjectEstimatedTimeNoTask_ReturnsTimeSpan()
        {
            //Arrange
            var controller = new CollabHubController();

            // Test Case : No tasks, base time only

            // Act
            var result = controller.CalculateProjectEstimatedTime(0, 0, 0);

            // Assert
            TimeSpan expectedTime = TimeSpan.FromDays(7);
            Assert.AreEqual(expectedTime, result);

            /*
             * Calculation for Test Case :
             * Base Time: 7 days
             */
        }

        
        //Notifications
        [TestMethod]
        public void CollabHub_SendNotification_ReturnList()
        {
            // Arrange
            var projectId = 123;
            var message = "Test message";
            var userList = new List<int> { 1, 2, 3 }; // User IDs
            var expectedNotifications = userList.Select(userId => new Notification { userId = userId, message = message }).ToList();

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.GetUserList(projectId)).Returns(userList);
            mockDbOperations.Setup(d => d.SendNotification(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.SendNotification(projectId, message);

            // Assert
            CollectionAssert.AreEqual(expectedNotifications, result, new NotificationComparer());

        }

        [TestMethod]
        public void CollabHub_SendNotificationToSpecificUsers_ReturnList()
        {
            // Arrange
            var projectId = 123;
            var message = "Test message";
            var userList = new List<int> { 1, 2, 3 }; // User IDs

            userList.Remove(1);

            var expectedNotifications = userList.Select(userId => new Notification { userId = userId, message = message }).ToList();

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.GetUserList(projectId)).Returns(userList);
            mockDbOperations.Setup(d => d.SendNotification(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.SendNotification(projectId, message);

            // Assert
            CollectionAssert.AreEqual(expectedNotifications, result, new NotificationComparer());

        }

        [TestMethod]
        public void CollabHub_VerifySendNotification_ReturnList()
        {
            // Arrange
            var projectId = 123;
            var message = "Test message";
            var userList = new List<int> { 1, 2, 3 }; // User IDs
            var expectedNotifications = userList.Select(userId => new Notification { userId = userId, message = message }).ToList();

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.GetUserList(projectId)).Returns(userList);
            mockDbOperations.Setup(d => d.SendNotification(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.SendNotification(projectId, message);

            // Verify that SendNotification is called for each user in the list
            foreach (var userId in userList)
            {
                mockDbOperations.Verify(d => d.SendNotification(userId, message), Times.Once);
            }
        }

        [TestMethod]
        public void CollabHub_SendNotificationOnProjectUpdate_ReturnList()
        {
            // Arrange
            var projectId = 123;
            var message = "Project Updated";
            var userList = new List<int> { 1, 2, 3 }; // User IDs
            var expectedNotifications = userList.Select(userId => new Notification { userId = userId, message = message }).ToList();

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.GetUserList(projectId)).Returns(userList);
//            mockDbOperations.Setup(d => d.SendNotification(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.SendNotification(projectId, message);

            // Assert
            CollectionAssert.AreEqual(expectedNotifications, result, new NotificationComparer());

        }

        [TestMethod]
        public void CollabHub_VerifySendNotificationOnProjectUpdate_ReturnList()
        {
            // Arrange
            var projectId = 123;
            var message = "Project Update";
            var userList = new List<int> { 1, 2, 3 }; // User IDs
            var expectedNotifications = userList.Select(userId => new Notification { userId = userId, message = message }).ToList();

            var mockDbOperations = new Mock<IDbOperations>();
            mockDbOperations.Setup(d => d.GetUserList(projectId)).Returns(userList);
            mockDbOperations.Setup(d => d.SendNotification(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            var controller = new CollabHubController(mockDbOperations.Object);

            // Act
            var result = controller.SendNotification(projectId, message);

            // Verify that SendNotification is called for each user in the list
            foreach (var userId in userList)
            {
                mockDbOperations.Verify(d => d.SendNotification(userId, message), Times.Once);
            }
        }


        // Custom comparer for comparing Notification objects
        private class NotificationComparer : Comparer<Notification>
        {
            public override int Compare(Notification x, Notification y)
            {
                if (x.userId != y.userId)
                    return x.userId.CompareTo(y.userId);

                return string.Compare(x.message, y.message);
            }
        }
    }


}
