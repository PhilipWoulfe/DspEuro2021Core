using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Interfaces.Models;

namespace TodoListService.Models
{
    public class UserSelections : IEntity
    {
        public int Id { get; set;  }
        public User User { get; set; }
        public Match Match { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAmendedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}
