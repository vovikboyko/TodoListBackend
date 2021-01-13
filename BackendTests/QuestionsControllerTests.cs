using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using QandA.Controllers;
using QandA.Data;
using QandA.Data.Models;

namespace BackendTests
{
    public class TasksControllerTests
    {
        [Fact]
        public async void GetTasks_WhenNoParameters_ReturnsAllTasks()
        {
            var mockTasks = new List<TaskGetManyResponse>();
            for (int i = 1; i <= 10; i++)
            {
                mockTasks.Add(new TaskGetManyResponse
                {
                    TaskId = 1,
                    Title = $"Test title {i}",
                });
            }

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository
              .Setup(repo => repo.GetTasks())
              .Returns(() => Task.FromResult(mockTasks.AsEnumerable()));

            var mockConfigurationRoot = new Mock<IConfigurationRoot>();
            mockConfigurationRoot.SetupGet(config => 
                config[It.IsAny<string>()]).Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object, null, 
                null, null, mockConfigurationRoot.Object);

            var result = await tasksController.GetTasks(null, false);

            Assert.Equal(10, result.Count());
            mockDataRepository.Verify(mock => mock.GetTasks(), Times.Once());
        }

        [Fact]
        public async void GetTasks_WhenHaveSearchParameter_ReturnsCorrectTasks()
        {
            var mockTasks = new List<TaskGetManyResponse>();
            mockTasks.Add(new TaskGetManyResponse
            {
                TaskId = 1,
                Title = "Test",
            });

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository
              .Setup(repo => repo.GetTasksBySearchWithPaging("Test", 1, 20))
              .Returns(() => Task.FromResult(mockTasks.AsEnumerable()));

            var mockConfigurationRoot = new Mock<IConfigurationRoot>();
            mockConfigurationRoot.SetupGet(config =>
                config[It.IsAny<string>()]).Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object, null, 
                null, null, mockConfigurationRoot.Object);

            var result = await tasksController.GetTasks("Test", false);

            Assert.Single(result);
            mockDataRepository.Verify(mock => mock.GetTasksBySearchWithPaging("Test", 1, 20), Times.Once());
        }

        [Fact]
        public async void GetTask_WhenTaskNotFound_Returns404()
        {
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository
              .Setup(repo => repo.GetTask(1))
              .Returns(() => Task.FromResult(default(TaskGetSingleResponse)));

            var mockTaskCache = new Mock<ITaskCache>();
            mockTaskCache
              .Setup(cache => cache.Get(1))
              .Returns(() => null);

            var mockConfigurationRoot = new Mock<IConfigurationRoot>();
            mockConfigurationRoot.SetupGet(config =>
                config[It.IsAny<string>()]).Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object, null, mockTaskCache.Object, 
                null, mockConfigurationRoot.Object);

            var result = await tasksController.GetTask(1);
            
            var actionResult = Assert.IsType<ActionResult<TaskGetSingleResponse>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void GetTask_WhenTaskIsFound_ReturnsTask()
        {
            var mockTask = new TaskGetSingleResponse
            {
                TaskId = 1,
                Title = "test"
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository
              .Setup(repo => repo.GetTask(1))
              .Returns(() => Task.FromResult(mockTask));

            var mockTaskCache = new Mock<ITaskCache>();
            mockTaskCache
             .Setup(cache => cache.Get(1))
             .Returns(() => mockTask);

            var mockConfigurationRoot = new Mock<IConfigurationRoot>();
            mockConfigurationRoot.SetupGet(config =>
                config[It.IsAny<string>()]).Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object, null, mockTaskCache.Object, 
                null, mockConfigurationRoot.Object);

            var result = await tasksController.GetTask(1);

            var actionResult = Assert.IsType<ActionResult<TaskGetSingleResponse>>(result);
            var taskResult = Assert.IsType<TaskGetSingleResponse>(actionResult.Value);
            Assert.Equal(1, taskResult.TaskId);
        }
    }
}
