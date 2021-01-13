using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Data.Models
{
    public class TaskPostFullRequest
    {
        public string Title { get; set; }
        public DateTime Created { get; set; }
    }
}
