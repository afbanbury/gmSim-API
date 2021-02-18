using System;
using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Controllers;
using gmSim_API.Services;
using Microsoft.Extensions.Logging;

namespace gmSim_API.ProcessServices
{
    public class DivisionalFinalsProcessService
    {
        private readonly ILogger<FixturesController> _logger;
        private readonly GameService _gameService;
        private readonly FixturesService _fixturesService;
        private readonly StandingsService _standingsService;
        private readonly TeamsService _teamsService;

        public DivisionalFinalsProcessService(ILogger<FixturesController> logger,
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

        public void PlayFinals()
        {
            GameDocument game = _gameService.Get();

            if (game.NextWeekType == WeekType.DivisionalFinals)
            {
                List<FixturesDocument> fixtures = _fixturesService.GetThisWeeksFixtures(game.Season, game.Week);

                foreach (var fixture in fixtures)
                {
                    PlayGame(fixture.Id);
                }
            }


            var nextWeek = game.Week + 1;
            var nextWeekGameType = WeekType.Championship;

            UpdateChampionshipFixture();

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
        
        private void UpdateChampionshipFixture()
        {
            GameDocument game = _gameService.Get();
            var winners = new Dictionary<string, string>();

            foreach (var conference in game.Conferences)
            {
                var fixture = _fixturesService.GetFinalsFixtures(game.Season, conference);

                if (fixture.HomeScore > fixture.AwayScore)
                {
                    winners.Add(conference, fixture.HomeTeamId);
                }
                else
                {
                    winners.Add(conference, fixture.AwayTeamId);
                }
            }

            var scheduleStrength = 0;
            string homeTeam = "";
            string awayTeam = "";
            
            foreach (var conference in game.Conferences)
            {
                winners.TryGetValue(conference, out string finalist);
                var leader = _standingsService.GetTeamStandings(finalist, game.Season);

                if (leader.ScheduleWeight >= scheduleStrength)
                {
                    awayTeam = homeTeam;
                    homeTeam = leader.TeamId;
                    scheduleStrength = leader.ScheduleWeight;
                }
                else
                {
                    awayTeam = leader.TeamId;
                }
            }
            
            _fixturesService.UpdateChampionship(game.Season, homeTeam, awayTeam);
        }
    }
}