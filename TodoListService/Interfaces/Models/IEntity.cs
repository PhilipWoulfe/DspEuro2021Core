using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;

namespace TodoListService.Interfaces.Models
{
    interface IEntity
    {
        string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAmendedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
