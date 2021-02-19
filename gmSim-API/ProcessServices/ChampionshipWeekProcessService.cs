using System;
using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Controllers;
using gmSim_API.Services;
using Microsoft.Extensions.Logging;

namespace gmSim_API.ProcessServices
{
    public class ChampionshipWeekProcessService
    {
        private readonly ILogger<FixturesController> _logger;
        private readonly GameService _gameService;
        private readonly FixturesService _fixturesService;
        private readonly StandingsService _standingsService;
        private readonly TeamsService _teamsService;

        public ChampionshipWeekProcessService(ILogger<FixturesController> logger,
            GameService gameService,
            FixturesService fixturesService,
            StandingsService standingsService,
            TeamsService teamsService)
        {
            _logger = logger;
            _gameService = gameService;
            _fixturesService = fixturesService;
            _standingsService = standingsService;
            _teamsService = teamsService;
        }

        public void PlayChampionship()
        {
            GameDocument game = _gameService.Get();

            if (game.NextWeekType == WeekType.Championship)
            {
                List<FixturesDocument> fixtures = _fixturesService.GetThisWeeksFixtures(game.Season, game.Week);

                foreach (var fixture in fixtures)
                {
                    PlayGame(fixture.Id);
                }
            }
            
            var nextWeek = game.Week + 1;
            var nextWeekGameType = WeekType.EndSeason;
            
            _gameService.NewWeek(nextWeek, nextWeekGameType);
        }
        
        private void PlayGame(string fixtureId)
        {
            Random rnd = new Random();

            var homeScore = rnd.Next(40);
            var awayScore = rnd.Next(35);

            while (homeScore == awayScore)
            {
                homeScore += rnd.Next(7);
                awayScore += rnd.Next(7);
            }
            
            _fixturesService.UpdateFixture(fixtureId, homeScore, awayScore);
        }
    }
}