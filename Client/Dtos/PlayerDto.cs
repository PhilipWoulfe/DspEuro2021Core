using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListClient.Enums;

namespace TodoListClient.Dtos
{
    public class PlayerDto
    {
        public string Id { get; set; }

        public string PlayerName { get; set; }

        public int? Points { get; set; }

        [BindProperty]
        public IList<PlayerMatchDto> Matches { get; set; }

        public SortedDictionary<Group, List<GroupTableDto>> GroupTables { get; set; }
        
        public string GoldenBoot { get; set; }
    }
}
