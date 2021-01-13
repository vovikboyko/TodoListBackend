using Microsoft.Extensions.Caching.Memory;
using ToDoList.Data.Models;

namespace ToDoList.Data
{
    public class TaskCache: ITaskCache
    {
        private MemoryCache _cache { get; set; }
        public TaskCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100
            });
        }

        private string GetCacheKey(int taskId) => $"Task-{taskId}";

        public TaskGetSingleResponse Get(int taskId)
        {
            TaskGetSingleResponse task;
            _cache.TryGetValue(GetCacheKey(taskId), out task);
            return task;
        }

        public void Set(TaskGetSingleResponse task)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
            _cache.Set(GetCacheKey(task.TaskId), task, cacheEntryOptions);
        }

        public void Remove(int taskId)
        {
            _cache.Remove(GetCacheKey(taskId));
        }
    }
}