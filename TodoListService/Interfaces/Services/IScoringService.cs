using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListService.Models;

namespace TodoListService.Interfaces.Services
{
    public interface IScoringService
    {
        Points ScoreMatch(Match match, UserSelection userSelection, IEnumerable<Match> matches);
        Task<JsonResponse> DeserializeOptimizedFromStreamCallAsync(string apiKey, string endPoint);

    }
}
