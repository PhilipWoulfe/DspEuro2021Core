using System;
using System.Collections.Generic;
using TodoListClient.Enums;
using TodoListClient.Models;

namespace TodoListClient.Dtos
{
    public class PlayerMatchDto
    {
        public string Id { get; set; }
        public DateTime UtcDate { get; set; }
        public Status Status { get; set; }
        public Stage Stage { get; set; }
        public string HomeTeam { get; set; }
        public int? HomeScore { get; set; }
        public string AwayTeam { get; set; }
        public int? AwayScore { get; set; }
        public int? Points { get; set; }
        public List<string> Reasons { get; set; }
    }
}