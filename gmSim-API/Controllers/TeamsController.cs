using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/teams")]
    //TODO add automapper to convert from TeamDocument to the entity model
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> _logger;
        private readonly TeamsService _teamsService;

        public TeamsController(ILogger<TeamsController> logger, TeamsService teamsService)
        {
            _logger = logger;
            _teamsService = teamsService;
        }

        [HttpGet]
        public ActionResult<List<TeamsDocument>> GetTeams()
        {
            return Ok(_teamsService.Get());
        }

        [HttpGet("{id}", Name ="GetTeam")]
        public ActionResult<TeamsDocument> GetTeam(string id)
        {
            return Ok(_teamsService.Get(id));
        }
        
        [HttpGet("{conference}/{division}", Name ="GetDivisionTeams")]
        public ActionResult<TeamsDocument> GetDivisionTeams(string conference, string division)
        {
            return Ok(_teamsService.GetDivisionTeams(conference, division));
        }
        
        [HttpPost("CreateTeam")]
        public ActionResult<TeamsDocument> CreateTeam(TeamsDocument team)
        {
            _teamsService.Create(team);

            return CreatedAtRoute("GetTeam", new { id = team.Id.ToString() }, team);
        }
    }
}