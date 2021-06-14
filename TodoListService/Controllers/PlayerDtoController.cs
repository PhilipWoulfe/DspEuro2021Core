/*
 The MIT License (MIT)

Copyright (c) 2018 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Dtos;
using TodoListService.Interfaces.Services;
using TodoListService.Models;
using TodoListService.Enums;
using TodoListService.Services;

namespace TodoListService.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PlayerDtoController : Controller
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICosmosMatchDbService _cosmosMatchDbService;
        private readonly ICosmosUserDbService _cosmosUserDbService;

        public PlayerDtoController(IHttpContextAccessor contextAccessor, ICosmosMatchDbService cosmosMatchDbService,
            ICosmosUserDbService cosmosUserDbService)
        {
            _contextAccessor = contextAccessor;
            _cosmosMatchDbService = cosmosMatchDbService;
            _cosmosUserDbService = cosmosUserDbService;
        }

        // GET: api/values
        [HttpGet("{id}")]
        public async Task<PlayerDto> Get(string id)
        {
            var matches = await _cosmosMatchDbService.GetMatchesAsync("SELECT * FROM c");
            var user = await _cosmosUserDbService.GetUserAsync(id);

            PlayerDto player = new() {
                Id = user.Id,
                PlayerName = user.Username,
                Matches = new Collection<PlayerMatchDto>(),
                Points = 0
            };

            // check player matches against

            foreach (var userMatch in user.UserSelection)
            {
                var match = matches.FirstOrDefault(x => x.Id == userMatch.Id);
                var points = ScoringService.ScoreMatch(match, userMatch);

                player.Points += points?.Score;

                player.Matches.Add(new PlayerMatchDto() { 
                    Id = match.Id,
                    UtcDate = match.UtcDate,
                    Status = match.Status,
                    Stage = match.Stage,
                    HomeTeam = match.HomeTeam.Name,
                    AwayTeam = match.AwayTeam.Name,
                    HomeScore = userMatch.HomeTeamScore,
                    AwayScore = userMatch.AwayTeamScore,
                    Points = points?.Score,
                    Reasons = points?.Reasons
                });
                //if (match.Stage == Stage.GROUP_STAGE) {
                //    player.GroupTables.Matches.Add(match.Group, user.UserSelection)
                //}
            }

            player.GroupTables = CalculateTables(user.UserSelection, matches);

            player.GoldenBoot = user.GoldenBoot;

            return player;
        }

        // PATCH api/values
        [HttpPatch("{id}")]
        //[ValidateAntiForgeryToken]
        public async Task<PlayerDto> Patch(string id, [FromBody] PlayerDto player)
        {
            User user;
            if (ModelState.IsValid)
            {
                user = await _cosmosUserDbService.GetUserAsync(id);

                user.GoldenBoot = player.GoldenBoot;

                foreach(var match in player.Matches)
                {
                    var sel = user.UserSelection.Where(x => x.Id == match.Id).FirstOrDefault();
                    sel.HomeTeam = new Team() { Name = match.HomeTeam };
                    sel.HomeTeamScore = match.HomeScore;
                    sel.AwayTeam = new Team() { Name = match.AwayTeam };
                    sel.AwayTeamScore = match.AwayScore;
                }

                await _cosmosUserDbService.UpdateUserAsync(id, user);
            }

            return player;
        }

        private SortedDictionary<Group, List<GroupTableDto>> CalculateTables(List<UserSelection> userSelection, IEnumerable<Match> matches)
        {
            SortedDictionary<Group, List<GroupTableDto>> tables = new ();
            Dictionary<Group, List<UserSelection>> groups = new();



            foreach (var userMatch in userSelection)
            {
                var match = matches.FirstOrDefault(x => x.Id == userMatch.Id);
                var key = match.Group;
                if (key != null)
                {
                    if (!groups.TryGetValue((Group)key, out List<UserSelection> existing))
                    {
                        existing = new List<UserSelection>();
                        groups[(Group)key] = existing;
                    }

                    // At this point we know that "existing" refers to the relevant list in the 
                    // dictionary, one way or another.
                    existing.Add(userMatch);
                }
                //if (match.Stage == Stage.GROUP_STAGE) {
                //    player.GroupTables.Matches.Add(match.Group, user.UserSelection)
                //
                //
                tables = CalculateTables(groups);
            }

            return tables;
        }

        private SortedDictionary<Group, List<GroupTableDto>> CalculateTables(Dictionary<Group, List<UserSelection>> groups)
        {
            SortedDictionary<Group, List<GroupTableDto>> tables = new();

            foreach (var v in groups)
            {
                tables.Add(v.Key, CalculateTables(v.Value));
            }

            return tables;
        }

        private List<GroupTableDto> CalculateTables(List<UserSelection> selections)
        {
            List<GroupTableDto> table = new();

            foreach(var selection in selections)
            {
                var teamA = GetTeam(selection.HomeTeam.Name, table);
                var teamB = GetTeam(selection.AwayTeam.Name, table);

                var homeScore = selection.HomeTeamScore ?? 0;
                var awayScore = selection.AwayTeamScore ?? 0;

                teamA = Update(teamA, homeScore, awayScore);
                teamB = Update(teamB, awayScore, homeScore);

                if (homeScore == awayScore)
                {
                    teamA.Drawn++;
                    teamB.Drawn++;
                }
                else if (homeScore > awayScore)
                {
                    teamA.Won++;
                    teamB.Lost++;
                }
                else
                {
                    teamA.Lost++;
                    teamB.Won++;
                } 
            }

            return table.OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ToList();
        }

        private GroupTableDto Update(GroupTableDto team, int forTeam, int againstTeam)
        {
            team.Played++;
            team.GoalsFor += forTeam;
            team.GoalsAgainst += againstTeam;

            return team;
        }

        private GroupTableDto GetTeam(string teamName, List<GroupTableDto> table)
        {
            var team = table.Where(i => i.Name == teamName).FirstOrDefault();

            if (team == null)
            {
                team = new GroupTableDto()
                {
                    Name = teamName,
                };

                table.Add(team);
            }

            return team;
        } 

        // GET: api/values
        [HttpGet()]
        public async Task<IEnumerable<PlayerDto>> Get()
        {
            var users = await _cosmosUserDbService.GetUsersAsync("SELECT * FROM c");
            var matches = await _cosmosMatchDbService.GetMatchesAsync("SELECT * FROM c");

            users = users.Where(x => !x.IsDeleted);

            IList<PlayerDto> players = new List<PlayerDto>();
            if(users == null)
            {
                return null;
            }

            foreach(var user in users)
            {
                var points = 0;

                if(user.UserSelection == null)
                {
                    return null;
                }

                foreach(var userSelection in user.UserSelection) {
                    var match = matches.FirstOrDefault(x => x.Id == userSelection.Id);
                    var score = ScoringService.ScoreMatch(match, userSelection);
                    var point = score?.Score ?? 0;
                    
                    points += point;
                }

                players.Add(new()
                {
                    Id = user.Id,
                    PlayerName = user.Username,
                    Points = points
                });
            }

            return players.OrderByDescending(x => x.Points);
        }


        //// PATCH api/values
        //[HttpPatch("{id}")]
        ////[ValidateAntiForgeryToken]
        //public async Task<Match> Patch(string id, [FromBody] Match match)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _cosmosMatchDbService.UpdateMatchAsync(id, match);
        //        //return RedirectToAction("Index");
        //    }

        //    return match;
        //}
    }
}