using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ToDoList.Controllers;
using ToDoList.Data;
using ToDoList.Data.Models;
using NUnit.Framework;

namespace NUnitTestToDo.TasksControllerTests
{
    class GetTaskTests
    {
        [Test]
        public async Task GetTask_WhenTaskNotFound_Returns404()
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
                mockDataRepository.Object,
                null,
                mockTaskCache.Object,
                null,
                mockConfigurationRoot.Object);

            var result = await tasksController.GetTask(1);

            Assert.IsInstanceOf(typeof(NotFoundResult), result.Result);
        }

        [Test]
        public async Task GetTask_WhenTaskIsFound_ReturnsTask()
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

            var mockQuestionCache = new Mock<ITaskCache>();

            mockQuestionCache
                .Setup(cache => cache.Get(1))
                .Returns(() => mockTask);

            var mockConfigurationRoot = new Mock<IConfigurationRoot>();

            mockConfigurationRoot.SetupGet(config =>
                config[It.IsAny<string>()]).Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object,
                null,
                mockQuestionCache.Object,
                null,
                mockConfigurationRoot.Object);

            var result = await tasksController.GetTask(1);

            Assert.IsInstanceOf(typeof(TaskGetSingleResponse), result.Value);

            Assert.AreEqual(1, result.Value.TaskId);
        }
    }
}
