using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Data;
using ToDoList.Data.Models;
using Microsoft.AspNetCore.SignalR;
using ToDoList.Hubs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHubContext<TasksHub> _taskHubContext;
        private readonly ITaskCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _auth0UserInfo;

        public TasksController(IDataRepository dataRepository, IHubContext<TasksHub> taskHubContext, ITaskCache taskCache, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _dataRepository = dataRepository;
            _taskHubContext = taskHubContext;
            _cache = taskCache;
            _clientFactory = clientFactory;
            _auth0UserInfo = $"{configuration["Auth0:Authority"]}userinfo";
        }

        [HttpGet]
        public async Task<IEnumerable<TaskGetManyResponse>> GetTasks()
        {
            return await _dataRepository.GetTasks();
        }

        [HttpGet("unanswered")]
        public async Task<IEnumerable<TaskGetManyResponse>> GetUnansweredTasks()
        {
            return await _dataRepository.GetUnansweredTasksAsync();
            //return await _dataRepository.GetTasks();

        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TaskGetSingleResponse>> GetTask(int taskId)
        {
            var task = _cache.Get(taskId);
            if (task == null)
            {
                task = await _dataRepository.GetTask(taskId);
                if (task == null)
                {
                    return NotFound();
                }
                _cache.Set(task);
            }
            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskGetSingleResponse>> PostTask(TaskPostRequest taskPostRequest)
        {
            //taskPostRequest.Title = null;
            var savedTask = await _dataRepository.PostTask(new TaskPostFullRequest
            {
                Title = taskPostRequest.Title,
                Created = DateTime.UtcNow
            });
            try
            {
                return CreatedAtAction(nameof(GetTask), new
                {
                    taskId = savedTask.TaskId
                }, savedTask);
            }
            catch
            {
                throw;
            }
            finally { }
        }

        [HttpPut("{taskId}")]
        public async Task<ActionResult<TaskGetSingleResponse>> PutTask(int taskId, TaskPutRequest taskPutRequest)
        {
            var task = await _dataRepository.GetTask(taskId);
            if (task == null)
            {
                return NotFound();
            }
            taskPutRequest.Title = string.IsNullOrEmpty(taskPutRequest.Title) ? task.Title : taskPutRequest.Title;
            taskPutRequest.isDone = !task.isDone;
            var savedTask = await _dataRepository.PutTask(taskId, taskPutRequest);

            await _taskHubContext.Clients.Group($"Task-{savedTask.TaskId}")
                .SendAsync("ReceiveTask", _dataRepository.GetTask(savedTask.TaskId));

            _cache.Remove(savedTask.TaskId);
            return savedTask;
        }

        [HttpDelete("{taskId}")]
        public async Task<ActionResult> DeleteTask(int taskId)
        {
            var task = await _dataRepository.GetTask(taskId);
            if (task == null)
            {
                return NotFound();
            }
            await _dataRepository.DeleteTask(taskId);
            _cache.Remove(taskId);
            return NoContent();
        }

        [HttpPost("answer")]
        public async Task<ActionResult<AnswerGetResponse>> PostAnswer(AnswerPostRequest answerPostRequest)
        {
            var taskExists = await _dataRepository.TaskExists(answerPostRequest.TaskId.Value);
            if (!taskExists)
            {
                return NotFound();
            }
            var savedAnswer = await _dataRepository.PostAnswer(new AnswerPostFullRequest
            {
                TaskId = answerPostRequest.TaskId.Value,
                Content = answerPostRequest.Content,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName = await GetUserName(),
                Created = DateTime.UtcNow
            });

            _cache.Remove(answerPostRequest.TaskId.Value);

            await _taskHubContext.Clients.Group($"Task-{answerPostRequest.TaskId.Value}").SendAsync("ReceiveTask", _dataRepository.GetTask(answerPostRequest.TaskId.Value));

            return savedAnswer;
        }

        private async Task<string> GetUserName()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _auth0UserInfo);
            request.Headers.Add("Authorization", Request.Headers["Authorization"].First());

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return user.Name;
            }
            else
            {
                return "";
            }
        }

    }
}
