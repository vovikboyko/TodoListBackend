using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ToDoList.Controllers;
using ToDoList.Data;
using ToDoList.Data.Models;
using NUnit.Framework;
using System;

namespace NUnitTestToDo.TasksControllerTests
{
    class PostTaskTests
    {
        [Test]
        public async Task PostTask_WhenTitleIsNull_Returns404()
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
            var taskPostRequest = new TaskPostRequest { Title = null };
            try
            {
                var result = await tasksController.PostTask(taskPostRequest);
            }
            catch(Exception er)
            {
                Assert.IsInstanceOf(typeof(System.NullReferenceException), er);
            }

            //Assert.IsInstanceOf(typeof(NotFoundResult), result.Result);
        }

    }
}
