using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Data.Models;

namespace ToDoList.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<TaskGetManyResponse>> GetTasks();
        //Task<IEnumerable<TaskGetManyResponse>> GetTasksWithAnswers();
        Task<IEnumerable<TaskGetManyResponse>> GetTasksBySearch(string search);
        Task<IEnumerable<TaskGetManyResponse>> GetTasksBySearchWithPaging(string search, int pageNumber, int pageSize);
        Task<IEnumerable<TaskGetManyResponse>> GetUnansweredTasks();
        Task<IEnumerable<TaskGetManyResponse>> GetUnansweredTasksAsync();
        Task<TaskGetSingleResponse> GetTask(int taskId);
        Task<bool> TaskExists(int taskId);
        Task<AnswerGetResponse> GetAnswer(int answerId);
        Task<TaskGetSingleResponse> PostTask(TaskPostFullRequest task);
        Task<TaskGetSingleResponse> PutTask(int taskId, TaskPutRequest task);
        Task DeleteTask(int taskId);
        Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer);
    }
}
