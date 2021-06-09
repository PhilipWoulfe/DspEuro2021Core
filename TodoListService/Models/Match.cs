using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Enums;
using TodoListService.Interfaces.Models;

namespace TodoListService.Models
{
    public class Match : IEntity
    {
        public string Id { get; set; }
        public int MatchApiId { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public Status Status { get; set; }
        public MatchType MatchType { get; set; }
        public Group Group { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastAmendedDate { get; set; }
        public int UpdatedBy { get; set; }

    }
}
