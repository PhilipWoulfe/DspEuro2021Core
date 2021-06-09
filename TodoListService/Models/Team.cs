using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Interfaces.Models;

namespace TodoListService.Models
{
    public class Team : IEntity
    {
        public string Id { get; set; }
        public int ApiId { get; set; }
        public string Name { get; set; }
        public string CrestUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAmendedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
