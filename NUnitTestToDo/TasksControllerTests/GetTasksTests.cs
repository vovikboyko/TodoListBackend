using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using ToDoList.Controllers;
using ToDoList.Data;
using ToDoList.Data.Models;
using NUnit.Framework;

namespace NUnitTestToDo.TasksControllerTests
{
    class GetTasksTests
    {
        [Test]
        public async Task GetTasks_NoParams_RetAllTasks()
        {
            var mockTasks = new List<TaskGetManyResponse>();
            for (int i = 1; i <= 10; i++)
            {
                mockTasks.Add(new TaskGetManyResponse
                {
                    TaskId = 1,
                    Title = $"Test content {i}",
                    isDone = false,
                    Created = new DateTime()
                });
            }

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository
                .Setup(repo => repo.GetTasks())
                .Returns(() => Task.FromResult(mockTasks.AsEnumerable()));

            var mockConfiguratioRoot = new Mock<IConfigurationRoot>();
            mockConfiguratioRoot
                .Setup(config => config[It.IsAny<string>()])
                .Returns("some setting");

            var tasksController = new TasksController(
                mockDataRepository.Object,
                null,
                null,
                null,
                mockConfiguratioRoot.Object);

            var result = await tasksController.GetTasks();

            Assert.AreEqual(10, result.Count());
            mockDataRepository.Verify(
                mock => mock.GetTasks(), Times.Once());
        }


    }
}
