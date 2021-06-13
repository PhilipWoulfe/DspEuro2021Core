using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListService.Models
{
    public class Points
    {

        public int Score { get; set; }

        public List<string> Reasons { get; set; }
    }
}
