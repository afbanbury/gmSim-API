using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/fixtures")]

    public class FixturesController : ControllerBase
    {
        private readonly ILogger<FixturesController> _logger;
        private readonly FixturesService _fixturesService;

        public FixturesController(ILogger<FixturesController> logger, FixturesService fixturesService)
        {
            _logger = logger;
            _fixturesService = fixturesService;
        }

        [HttpGet]
        public ActionResult<List<FixturesDocument>> GetFixtures()
        {
            return Ok(_fixturesService.Get());
        }
        
        [HttpGet("ForSeason/{season}", Name ="GetThisSeasonsFixtures")]
        public ActionResult<FixturesDocument> GetThisSeasonsFixtures(int season)
        {
            return Ok(_fixturesService.GetThisSeasonsFixtures(season));
        }
        
        [HttpGet("ForWeek/{season}/{week}", Name ="GetThisWeeksFixtures")]
        public ActionResult<FixturesDocument> GetThisWeeksFixtures(int season, int week)
        {
            return Ok(_fixturesService.GetThisWeeksFixtures(season, week));
        }
        
        [HttpGet("GetTeamFixturesForSeason/{teamId}/{season}", Name ="GetTeamFixturesForSeason")]
        public ActionResult<FixturesDocument> GetTeamFixturesForSeason(string teamId, int season)
        {
            return Ok(_fixturesService.GetTeamFixturesForSeason(teamId, season));
        }
        
        [HttpGet("{id}", Name ="GetFixture")]
        public ActionResult<FixturesDocument> GetFixture(string id)
        {
            return Ok(_fixturesService.Get(id));
        }

        [HttpPost("CreateFixture")]
        public ActionResult<FixturesDocument> CreateFixture(FixturesDocument fixture)
        {
            _fixturesService.Create(fixture);

            return CreatedAtRoute("GetFixture", new { id = fixture.Id.ToString() }, fixture);
        }
    }
}
    
