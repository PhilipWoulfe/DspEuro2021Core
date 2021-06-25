﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;
using TodoListService.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TodoListService.Enums;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using TodoListService.Exceptions;

namespace TodoListService.Services
{
    public class ScoringService : IScoringService
    {
        private const int _correctScore = 2;
        private const int _correctResult = 1;
        private const int _correctHomeScore = 1;
        private const int _correctAwayScore = 1;
        private const int _correctMarginScore = 1;
        
        private const int _predictSecondRound = 3;
        private const int _predictQuarterFinals = 6;
        private const int _predictSemiFinals = 10;
        private const int _predictFinals = 15;
        private const int _predictWinner = 20;
        
        private readonly HttpClient _httpClient;

        public ScoringService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Points ScoreMatch(Match match, UserSelection userSelection, IEnumerable<Match> matches)
        {
            Points points = new();

            int score = 0;
            List<string> reasons = new();

            IEnumerable<Match> last16 = matches.Where(x => x.Stage == Stage.LAST_16);
            IEnumerable<Match> quarterFinals = matches.Where(x => x.Stage == Stage.QUARTER_FINAL);
            IEnumerable<Match> semiFinals = matches.Where(x => x.Stage == Stage.SEMI_FINAL);
            IEnumerable<Match> final = matches.Where(x => x.Stage == Stage.FINAL);


            switch (match.Stage)
            {
                case Stage.LAST_16:
                    if (last16.Where(x => x.HomeTeam.Name == userSelection.HomeTeam.Name).Any())
                    {
                        score += _predictSecondRound;
                        reasons.Add("Predicted Home Team Qualified |");
                    }

                    if (last16.Where(x => x.AwayTeam.Name == userSelection.AwayTeam.Name).Any())
                    {
                        score += _predictSecondRound;
                        reasons.Add("Predicted Away Team Qualified |");
                    }
                    break;
                case Stage.QUARTER_FINAL:
                    if (quarterFinals.Where(x => x.HomeTeam.Name == userSelection.HomeTeam.Name).Any())
                    {
                        score += _predictQuarterFinals;
                        reasons.Add("Predicted Home Team Qualified |");
                    }

                    if (quarterFinals.Where(x => x.AwayTeam.Name == userSelection.AwayTeam.Name).Any())
                    {
                        score += _predictQuarterFinals;
                        reasons.Add("Predicted Away Team Qualified |");
                    }
                    break;
                case Stage.SEMI_FINAL:
                    if (semiFinals.Where(x => x.HomeTeam.Name == userSelection.HomeTeam.Name).Any())
                    {
                        score += _predictSemiFinals;
                        reasons.Add("Predicted Home Team Qualified |");
                    }

                    if (semiFinals.Where(x => x.AwayTeam.Name == userSelection.AwayTeam.Name).Any())
                    {
                        score += _predictSemiFinals;
                        reasons.Add("Predicted Away Team Qualified |");
                    }
                    break;
                case Stage.FINAL:
                    if (final.Where(x => x.HomeTeam.Name == userSelection.HomeTeam.Name).Any())
                    {
                        score += _predictFinals;
                        reasons.Add("Predicted Home Team Qualified |");
                    }

                    if (final.Where(x => x.AwayTeam.Name == userSelection.AwayTeam.Name).Any())
                    {
                        score += _predictFinals;
                        reasons.Add("Predicted Away Team Qualified |");
                    }
                    break;
            }


            if (match.Status == Status.FINISHED)
            {
                var actualHomeScore = (match.Score.FullTime.HomeTeam ?? 0) + (match.Score.ExtraTime.HomeTeam ?? 0);
                var actualAwayScore = (match.Score.FullTime.AwayTeam ?? 0) + (match.Score.ExtraTime.AwayTeam ?? 0);

                if (match.Score.Penalties.HomeTeam != null)
                {
                    if (match.Score.Penalties.HomeTeam > match.Score.Penalties.AwayTeam)
                    {
                        actualHomeScore++;
                    }
                    else
                    {
                        actualAwayScore++;
                    }

                }

                if (userSelection.HomeTeamScore == actualHomeScore)
                {
                    score += _correctHomeScore;
                    reasons.Add("Correct Home Score |");
                }
                if (userSelection.AwayTeamScore == actualAwayScore)
                {
                    score += _correctAwayScore;
                    reasons.Add(" Correct Away Score |");
                }
                if (userSelection.HomeTeamScore == actualHomeScore && userSelection.AwayTeamScore == actualAwayScore)
                {
                    score += _correctScore;
                    reasons.Add(" Correct Score |");
                }
                if (userSelection.HomeTeamScore > userSelection.AwayTeamScore && actualHomeScore > actualAwayScore
                    || userSelection.HomeTeamScore < userSelection.AwayTeamScore && actualHomeScore < actualAwayScore
                    || userSelection.HomeTeamScore == userSelection.AwayTeamScore && actualHomeScore == actualAwayScore)
                {
                    score += _correctResult;
                    reasons.Add(" Correct Result |");

                    if (match.Stage == Stage.FINAL)
                    {
                        score += _predictWinner;
                        reasons.Add(" Predicted Winner |");
                    }

                    if (userSelection.HomeTeamScore - userSelection.AwayTeamScore == actualHomeScore - actualAwayScore)
                    {
                        score += _correctMarginScore;
                        reasons.Add(" Correct Margin |");
                    }
                }
            }

            points.Score = score;
            points.Reasons = reasons;


            return points;
        }

        public async Task<JsonResponse> DeserializeOptimizedFromStreamCallAsync(string apiKey, string endPoint)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
            using (var request = new HttpRequestMessage(HttpMethod.Get, endPoint))
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return DeserializeJsonFromStream<JsonResponse>(stream);

                var content = await StreamToStringAsync(stream);
                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Content = content
                };
            }
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }
    }

}
