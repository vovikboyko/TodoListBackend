using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ToDoList.Data.Models
{
    public class TaskPutRequest
    {
        [StringLength(100)]
        public string Title { get; set; }
        public string Content { get; set; }
        public bool isDone { get; set; }
    }
}
