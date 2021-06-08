using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Interfaces.Models;

namespace TodoListService.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public bool IsPaid { get; set; }
        public bool IsAdmin { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAmendedDate { get; set; }
        public int UpdatedBy { get; set; }

    }
}
