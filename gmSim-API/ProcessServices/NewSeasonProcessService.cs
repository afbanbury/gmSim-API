using System;
using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Controllers;
using gmSim_API.Services;
using Microsoft.Extensions.Logging;

namespace gmSim_API.ProcessServices
{
    public class NewSeasonProcessService
    {
        private readonly ILogger<FixturesController> _logger;
        private readonly GameService _gameService;
        private readonly TeamsService _teamsService;
        private readonly FixturesService _fixturesService;
        private readonly StandingsService _standingsService;

        private int[] divWeeks = new[] { 1, 1, 2, 2, 4, 4, 5, 5, 7, 7, 8, 8, 11, 11, 12, 12, 14, 14, 15, 15 };
        private int[] divAteams = new[] { 1, 3, 5, 2, 3, 4, 1, 2, 4, 5, 2, 4, 1, 3, 1, 5, 4, 5, 2, 3 };
        private int[] divBteams = new[] { 2, 4, 1, 3, 1, 5, 4, 5, 2, 3, 1, 3, 5, 2, 3, 4, 1, 2, 4, 5 };
        
        private int[] intraConfWeeks = new[] { 3, 3, 3, 3, 3, 6, 6, 6, 6, 6, 9, 9, 9, 9, 9, 13, 13, 13, 13, 13, 16, 16, 16, 16, 16 };
        private int[] IntraConfAteams = new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };
        private int[] IntraConfBteams = new[] { 1, 2, 3, 4, 5, 2, 3, 4, 5, 1, 3, 4, 5, 1, 2, 4, 5, 1, 2, 3, 5, 1, 2, 3, 4 };
        
        private int[] interConfWeeks = new[] { 10, 10, 10, 10, 10, 17, 17, 17, 17, 17 };
        private int[] InterConfAteams = new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5};
        private int[] InterConfBteams = new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5};

        public NewSeasonProcessService(ILogger<FixturesController> logger,
            GameService gameService,
            TeamsService teamsService,
            FixturesService fixturesService,
            StandingsService standingsService)
        {
            _logger = logger;
            _gameService = gameService;
            _teamsService = teamsService;
            _fixturesService = fixturesService;
            _standingsService = standingsService;
        }

        public void SetUpNewSeason()
        {
            GameDocument game = _gameService.Get();
            var nextSeason = game.Season + 1;

            Dictionary<string, string> teamRankings = GetDivisionalRankings();
            DivisionalFixtures(teamRankings, nextSeason);
            IntraConferenceFixtures(teamRankings, nextSeason);
            InterConferenceFixtures(teamRankings, nextSeason);
            ConferenceFinals(teamRankings, nextSeason);
            
            CreateStandingsForNewSeason(nextSeason);
            _gameService.NewSeason(nextSeason);
        }

        private Dictionary<string, string> GetDivisionalRankings()
        {
            GameDocument game = _gameService.Get();
            Dictionary<string, string> teamRankings = new Dictionary<string, string>();


            foreach (var conference in game.Conferences)
            {
                foreach (var division in game.Divisions)
                {
                    int rank = 1;
                    List<StandingsDocument> teams =
                        _standingsService.GetDivisionStandings(conference, division, game.Season);
                    
                    foreach (var team in teams)
                    {
                        var keyString = conference + division + rank.ToString();
                        teamRankings.Add(keyString, team.TeamId);
                        rank++;
                    }
                    
                }
            }

            return teamRankings;
        }
        
        private void DivisionalFixtures(Dictionary<string, string> teamRankings, int nextSeason)
        {
            GameDocument game = _gameService.Get();

            for (int i = 0; i < divWeeks.Length; i++)
            {
                foreach (var conference in game.Conferences)
                {
                    foreach (var division in game.Divisions)
                    {
                        var teamAKey = conference + division + divAteams[i];
                        var teamBKey = conference + division + divBteams[i];
                        CreateFixture(nextSeason, divWeeks[i], "DIVISION", teamRankings[teamAKey], teamRankings[teamBKey]);
                    }
                }
            }
        }
        
        private void IntraConferenceFixtures(Dictionary<string, string> teamRankings, int nextSeason)
        {
            GameDocument game = _gameService.Get();

            for (int i = 0; i < intraConfWeeks.Length; i++)
            {
                foreach (var conference in game.Conferences)
                {
                    var teamAKey = conference + "East" + IntraConfAteams[i];
                    var teamBKey = conference + "West" + IntraConfBteams[i];
                    CreateFixture(nextSeason, intraConfWeeks[i], "INTRA-CONFERENCE", teamRankings[teamAKey], teamRankings[teamBKey]);
                }
            }
        }
        
        private void InterConferenceFixtures(Dictionary<string, string> teamRankings, int nextSeason)
        {
            GameDocument game = _gameService.Get();

            for (int i = 0; i < interConfWeeks.Length; i++)
            {
                if (i < 5)
                {
                    // NorthEast vs SouthEast & NorthWest vs SouthWest
                    foreach (var division in game.Divisions)
                    {
                        var teamAKey = "North" + division + IntraConfAteams[i];
                        var teamBKey = "South" + division + IntraConfBteams[i];
                        CreateFixture(nextSeason, interConfWeeks[i], "INTER-CONFERENCE", teamRankings[teamAKey], teamRankings[teamBKey]);
                    }
                }
                else
                {
                    //NorthEast vs SouthWest & NorthWest vs SouthEast
                    var teamA1Key = "North" + "East" + IntraConfAteams[i];
                    var teamB1Key = "South" + "West" + IntraConfBteams[i];
                    CreateFixture(nextSeason, interConfWeeks[i], "INTER-CONFERENCE", teamRankings[teamA1Key], teamRankings[teamB1Key]);
                    
                    var teamA2Key = "North" + "West" + IntraConfAteams[i];
                    var teamB2Key = "South" + "East" + IntraConfBteams[i];
                    CreateFixture(nextSeason, interConfWeeks[i], "INTER-CONFERENCE", teamRankings[teamA2Key], teamRankings[teamB2Key]);
                }
            }
        }

        private void ConferenceFinals(Dictionary<string, string> teamRankings, int nextSeason)
        {
            CreateFixture(nextSeason, 18, "NORTH FINAL", teamRankings["NorthEast1"], teamRankings["NorthWest1"]);
            CreateFixture(nextSeason, 18, "SOUTH FINAL", teamRankings["SouthEast1"], teamRankings["SouthWest1"]);
            CreateFixture(nextSeason, 19, "CHAMPIONSHIP", teamRankings["SouthEast1"], teamRankings["NorthWest1"]);
        }
        private void CreateFixture(int season, int week, string type, string homeTeam, string awayTeam)
        {
            FixturesDocument newFixture = new FixturesDocument()
            {
                Season = season,
                Week = week,
                Type = type,
                HomeTeamId = homeTeam,
                AwayTeamId = awayTeam,
                Played = false,
                HomeScore = 0,
                AwayScore = 0
            };
            
            _fixturesService.Create(newFixture);
        }


        private void CreateStandingsForNewSeason(int season)
        {
            List<TeamsDocument> _teams = _teamsService.Get();
            foreach (var team in _teams)
            {
                StandingsDocument newRecord = new StandingsDocument
                {
                    Season = season,
                    TeamId = team.Id,
                    Conference = team.Conference,
                    Division = team.Division,
                    DivisionRank = 1,
                    ConferenceRank = 1,
                    OverallRank = 1,
                    OverallWins = 0,
                    OverallLosses = 0,
                    OverallTies = 0,
                    OverallPct = 0,
                    OverallPointsFor = 0,
                    OverallPointsAgainst = 0,
                    HomeWins = 0,
                    HomeLosses = 0,
                    HomeTies = 0,
                    AwayWins = 0,
                    AwayLosses = 0,
                    AwayTies = 0,
                    DivisionWins = 0,
                    DivisionLosses = 0,
                    DivisionTies = 0,
                    DivisionPct = 0,
                    ConferenceWins = 0,
                    ConferenceLosses = 0,
                    ConferenceTies = 0,
                    ConferencePct = 0,
                    CurrentStreak = 'W',
                    StreakLength = 0,
                    LastFive = new char[] {'-', '-', '-', '-', '-'},
                    ScheduleWeight = 0
                };

                _standingsService.Create(newRecord);
            }
        }
    }
}