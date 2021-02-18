using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Models;
using gmSim_API.Controllers;
using gmSim_API.Services;
using Microsoft.Extensions.Logging;

namespace gmSim_API.ProcessServices
{
    public class RegularSeasonWeekProcessService
    {
        private readonly ILogger<FixturesController> _logger;
        private readonly GameService _gameService;
        private readonly FixturesService _fixturesService;
        private readonly StandingsService _standingsService;
        private readonly TeamsService _teamsService;
        private const int RegularSeasonLength = 17;
        
        public RegularSeasonWeekProcessService(ILogger<FixturesController> logger,
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

        public void PlayWeek()
        {
            GameDocument game = _gameService.Get();
            
            if (game.NextWeekType == WeekType.RegularSeason)
            {
                List<FixturesDocument> fixtures = _fixturesService.GetThisWeeksFixtures(game.Season, game.Week);

                foreach (var fixture in fixtures)
                {
                    PlayGame(fixture.Id);
                    UpdateResult(fixture.Id, game.Season);
                }

                UpdateDivisionRankings();
                UpdateScheduleStrength();
            }
            

            var nextWeek = game.Week + 1;
            var nextWeekGameType = game.NextWeekType;

            if (nextWeek > RegularSeasonLength)
            {
                UpdateFinalsFixtures();
                nextWeekGameType = WeekType.DivisionalFinals;
            }

            _gameService.NewWeek(nextWeek, nextWeekGameType);
        }

        private void PlayGame(string fixtureId)
        {
            Random rnd = new Random();

            var homeScore = rnd.Next(40);
            var awayScore = rnd.Next(35);
            _fixturesService.UpdateFixture(fixtureId, homeScore, awayScore);
        }

        private void UpdateResult(string fixtureId, int season)
        {
            int homeWin, awayWin, tie;
            homeWin = awayWin = tie = 0;
            var fixture = _fixturesService.Get(fixtureId);
            var homeStanding = _standingsService.GetTeamStandings(fixture.HomeTeamId, season);
            var awayStanding = _standingsService.GetTeamStandings(fixture.AwayTeamId, season);

            if (fixture.HomeScore > fixture.AwayScore)
            {
                homeWin = 1;
                _standingsService.UpdateStreak(homeStanding.Id, 'W');
                _standingsService.UpdateLastFive(homeStanding.Id, 'W');
                _standingsService.UpdateStreak(awayStanding.Id, 'L');
                _standingsService.UpdateLastFive(awayStanding.Id, 'L');
            }
            else if (fixture.AwayScore > fixture.HomeScore)
            {
                awayWin = 1;
                _standingsService.UpdateStreak(homeStanding.Id, 'L');
                _standingsService.UpdateLastFive(homeStanding.Id, 'L');
                _standingsService.UpdateStreak(awayStanding.Id, 'W');
                _standingsService.UpdateLastFive(awayStanding.Id, 'W');
            }
            else
            {
                tie = 1;
                _standingsService.UpdateStreak(homeStanding.Id, 'T');
                _standingsService.UpdateLastFive(homeStanding.Id, 'T');
                _standingsService.UpdateStreak(awayStanding.Id, 'T');
                _standingsService.UpdateLastFive(awayStanding.Id, 'T');
            }

            if (fixture.Type != "NORTH FINAL" && fixture.Type != "SOUTH FINAL" && fixture.Type != "CHAMPIONSHIP")
            {
                _standingsService.UpdateOverallStandings(homeStanding.Id, homeWin, awayWin, tie, fixture.HomeScore,
                    fixture.AwayScore);
                _standingsService.UpdateOverallStandings(awayStanding.Id, awayWin, homeWin, tie, fixture.AwayScore,
                    fixture.HomeScore);
                _standingsService.UpdateHomeStandings(homeStanding.Id, homeWin, awayWin, tie);
                _standingsService.UpdateAwayStandings(awayStanding.Id, awayWin, homeWin, tie);
            }

            if (fixture.Type == "DIVISION")
            {
                _standingsService.UpdateDivisionStandings(homeStanding.Id, homeWin, awayWin, tie);
                _standingsService.UpdateDivisionStandings(awayStanding.Id, awayWin, homeWin, tie);
            }
            else if (fixture.Type == "INTRA-CONFERENCE")
            {
                _standingsService.UpdateConferenceStandings(homeStanding.Id, homeWin, awayWin, tie);
                _standingsService.UpdateConferenceStandings(awayStanding.Id, awayWin, homeWin, tie);               
            }
        }

        private void UpdateDivisionRankings()
        {
            GameDocument game = _gameService.Get();

            foreach (var conference in game.Conferences)
            {
                foreach (var division in game.Divisions)
                {
                    var divisionStandings = _standingsService.GetDivisionStandings(conference, division, game.Season);
                    var newDivisionRank = divisionStandings.OrderByDescending(o => o.OverallPct)
                        .ThenByDescending(o => o.DivisionPct)
                        .ThenByDescending(o => o.ConferencePct)
                        .ThenByDescending(o => (o.OverallPointsFor - o.OverallPointsAgainst)).ToList();
                    var rank = 1;
                    foreach (var record in newDivisionRank)
                    {
                        _standingsService.UpdateDivisionRank(record.Id, rank);
                        rank++;
                    }
                }
            }
        }

        private void UpdateScheduleStrength()
        {
            var teams = _teamsService.Get();
            var game = _gameService.Get();

            foreach (var team in teams)
            {
                var fixtures = _fixturesService.GetTeamFixturesForSeason(team.Id, game.Season);
                var strength = 0;

                foreach (var fixture in fixtures)
                {
                    string opponent;
                    
                    if (fixture.HomeTeamId == team.Id && fixture.Played)
                    {
                        opponent = fixture.AwayTeamId;
                    }
                    else
                    {
                        opponent = fixture.HomeTeamId;
                    }

                    var oppStandings = _standingsService.GetTeamStandings(opponent, game.Season);
                    strength += oppStandings.OverallWins;
                }

                _standingsService.UpdateScheduleWeight(team.Id, game.Season, strength);
            }
        }

        private void UpdateFinalsFixtures()
        {
            GameDocument game = _gameService.Get();
            
            foreach (var conference in game.Conferences)
            {
                int scheduleStrength = 0;
                string homeTeam = "";
                string awayTeam = "";
                
                foreach (var division in game.Divisions)
                {
                    var leader = _standingsService.GetDivisionLeaders(conference, division, game.Season);
                    Console.WriteLine("Conference = " + conference);
                    Console.WriteLine("Division = " + division);
                    Console.WriteLine("leader = " + leader.TeamId);

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
                Console.WriteLine("homeTeam = " + homeTeam);
                Console.WriteLine("awayTeam = " + awayTeam);

                _fixturesService.UpdateFinals(game.Season, conference, homeTeam, awayTeam);
            }
        }
    }
}