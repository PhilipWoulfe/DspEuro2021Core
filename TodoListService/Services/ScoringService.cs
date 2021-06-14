using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;
using TodoListService.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TodoListService.Enums;

namespace TodoListService.Services
{
    public class ScoringService
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
        
        private const int _predictGoldenBoot = 25;

        public static Points ScoreMatch(Match match, UserSelection userSelection)
        {
            Points points = new Points();

            int score = 0;
            List<string> reasons = new();

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

                    if (userSelection.HomeTeamScore - userSelection.AwayTeamScore == actualHomeScore - actualAwayScore)
                    {
                        score += _correctMarginScore;
                        reasons.Add(" Correct Margin |");
                    }
                }

                points.Score = score;
                points.Reasons = reasons;
            }
            else
            {
                return null;
            }

            return points;
        }
    }
}
