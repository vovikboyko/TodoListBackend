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
    class DeleteTaskTests
    {
        [Test]
        public async Task DeleteTask_WhenTaskNotFound_ReturnsNotFound()
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

            var result = await tasksController.DeleteTask(1);

            Assert.IsInstanceOf(typeof(NotFoundResult), result);

        }

        [Test]
        public async Task DeleteTask_WhenTaskIsFound_ReturnsNoContent()
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

            var result = await tasksController.DeleteTask(1);

            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }
    }
}
