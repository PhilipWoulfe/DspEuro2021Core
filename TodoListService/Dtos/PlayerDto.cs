using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Enums;

namespace TodoListService.Dtos
{
    public class PlayerDto
    {
        public string Id { get; set; }

        public string PlayerName { get; set; }

        public int? Points { get; set; }

        public ICollection<PlayerMatchDto> Matches { get; set; }

        public SortedDictionary<Group, List<GroupTableDto>> GroupTables { get; set; }

        public string GoldenBoot { get; set; }

        public int GoldenBootPoints { get; set; }

        public bool CompetionOver;

    }
}
