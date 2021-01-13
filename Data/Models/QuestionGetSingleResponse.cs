using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Data.Models
{
    public class TaskGetSingleResponse
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public bool isDone { get; set; }
        public DateTime Created { get; set; }
    }
}
