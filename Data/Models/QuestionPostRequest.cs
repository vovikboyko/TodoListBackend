using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Data.Models
{
    public class TaskPostRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
    }
}
