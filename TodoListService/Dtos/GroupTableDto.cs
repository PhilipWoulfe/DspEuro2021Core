using System.Collections.Generic;
using TodoListService.Enums;
using TodoListService.Models;

namespace TodoListService.Dtos
{
    public class GroupTableDto
    {
        private const int POINTS_FOR_WIN = 3;
        private const int POINTS_FOR_DRAW = 1;
        public string Name { get; set; }

        public int Played { get; set; }
        
        public int Won { get; set; }
        
        public int Drawn { get; set; }
        
        public int Lost { get; set; }
        
        public int GoalsFor { get; set; }
        
        public int GoalsAgainst { get; set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public int Points => (Won * POINTS_FOR_WIN) + (Drawn * POINTS_FOR_DRAW);

    }
}