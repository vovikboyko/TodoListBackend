using ToDoList.Data.Models;

namespace ToDoList.Data
{
    public interface ITaskCache
    {
        TaskGetSingleResponse Get(int taskId);
        void Remove(int taskId);
        void Set(TaskGetSingleResponse task);
    }
}