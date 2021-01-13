using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ToDoList.Data.Models;
using static Dapper.SqlMapper;

namespace ToDoList.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
        public async Task<AnswerGetResponse> GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Get_ByAnswerId 
                    @AnswerId = @AnswerId", 
                    new { AnswerId = answerId });
            }
        }

        public async Task<TaskGetSingleResponse> GetTask(int taskId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (GridReader results = await connection.QueryMultipleAsync(
                    @"EXEC dbo.Task_GetSingle @TaskId = @TaskId; ",
                    new { TaskId = taskId }))
                {
                    var task = (await results.ReadAsync<TaskGetSingleResponse>()).FirstOrDefault();
                   
                    return task;
                }
            }
        }

        public async Task<IEnumerable<TaskGetManyResponse>> GetTasks()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TaskGetManyResponse>("EXEC dbo.Task_GetMany");
            }
        }

        public async Task<IEnumerable<TaskGetManyResponse>> GetUnansweredTasksAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TaskGetManyResponse>("EXEC dbo.Task_GetUnanswered");
            }
        }

        //public async Task<IEnumerable<TaskGetManyResponse>> GetTasksWithAnswers()
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var taskDictionary = new Dictionary<int, TaskGetManyResponse>();
        //        return (await connection.QueryAsync<TaskGetManyResponse, AnswerGetResponse, TaskGetManyResponse>("EXEC dbo.Task_GetMany_WithAnswers",
        //          map: (q, a) =>
        //          {
        //              TaskGetManyResponse task;

        //              if (!taskDictionary.TryGetValue(q.TaskId, out task))
        //              {
        //                  task = q;
        //                  task.Answers = new List<AnswerGetResponse>();
        //                  taskDictionary.Add(task.TaskId, task);
        //              }
        //              task.Answers.Add(a);
        //              return task;
        //          },
        //          splitOn: "TaskId"))
        //          .Distinct()
        //          .ToList();
        //    }
        //}

        public async Task<IEnumerable<TaskGetManyResponse>> GetTasksBySearch(string search)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TaskGetManyResponse>(@"EXEC dbo.Task_GetMany_BySearch 
                    @Search = @Search", 
                    new { Search = search });
            }
        }

        public async Task<IEnumerable<TaskGetManyResponse>> GetTasksBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new { Search = search, PageNumber = pageNumber, PageSize = pageSize };
                return await connection.QueryAsync<TaskGetManyResponse>(@"EXEC dbo.Task_GetMany_BySearch_WithPaging
                    @Search = @Search, @PageNumber = @PageNumber, @PageSize = @PageSize", 
                    parameters);
            }
        }

        public async Task<IEnumerable<TaskGetManyResponse>> GetUnansweredTasks()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TaskGetManyResponse>("EXEC dbo.Task_GetUnanswered");
            }
        }

        

        public async Task<bool> TaskExists(int taskId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<bool>(@"EXEC dbo.Task_Exists 
                    @TaskId = @TaskId", 
                    new { TaskId = taskId });
            }
        }

        public async Task<TaskGetSingleResponse> PostTask(TaskPostFullRequest task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var taskId = await connection.QueryFirstAsync<int>(@"EXEC dbo.Task_Post 
                    @Title = @Title,
                    @Created = @Created", 
                    task);
                return await GetTask(taskId);
            }
        }

        public async Task<TaskGetSingleResponse> PutTask(int taskId, TaskPutRequest task)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(@"EXEC dbo.Task_Put 
                    @TaskId = @TaskId, @Title = @Title, @isDone= @isDone",
                    new { TaskId = taskId, task.Title, task.isDone });
                return await GetTask(taskId);
            }
        }

        public async Task DeleteTask(int taskId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(@"EXEC dbo.Task_Delete 
                    @TaskId = @TaskId", 
                    new { TaskId = taskId });
            }
        }

        public async Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Post 
                    @TaskId = @TaskId, @Content = @Content, @UserId = @UserId, @UserName = @UserName, @Created = @Created", 
                    answer);
            }
        }
    }
}
