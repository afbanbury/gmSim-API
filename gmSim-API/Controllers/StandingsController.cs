using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/standings")]
    public class StandingsController : ControllerBase
    {
        private readonly ILogger<StandingsController> _logger;
        private readonly StandingsService _standingsService;

        public StandingsController(ILogger<StandingsController> logger, StandingsService standingsService)
        {
            _logger = logger;
            _standingsService = standingsService;
        }

        [HttpGet]
        public ActionResult<List<StandingsDocument>> GetTeams()
        {
            return Ok(_standingsService.Get());
        }
        
        [HttpGet("{id}", Name ="GetRecord")]
        public ActionResult<StandingsDocument> GetRecord(string id)
        {
            return Ok(_standingsService.Get(id));
        }
        
        [HttpPost("CreateRecord")]
        public ActionResult<StandingsDocument> CreateRecord(StandingsDocument record)
        {
            _standingsService.Create(record);

            return CreatedAtRoute("GetRecord", new { id = record.Id.ToString() }, record);
        }

        [HttpGet("{conference}/{division}/{season}", Name = "GetDivisionStandings")]
        public ActionResult<List<StandingsDocument>> GetDivisionStandings(string conference, string division, int season)
        {
            return Ok(_standingsService.GetDivisionStandings(conference, division, season));
        }
    }
}