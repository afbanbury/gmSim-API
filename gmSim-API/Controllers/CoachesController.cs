using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/coaches")]
    public class CoachesController : ControllerBase
    {
        private readonly ILogger<CoachesController> _logger;
        private readonly CoachesService _coachesService;

        public CoachesController(ILogger<CoachesController> logger, CoachesService coachesService)
        {
            _logger = logger;
            _coachesService = coachesService;
        }

        [HttpGet]
        public ActionResult<List<CoachesDocument>> GetCoaches()
        {
            return Ok(_coachesService.Get());
        }
        
        [HttpGet("{id}", Name ="GetCoach")]
        public ActionResult<CoachesDocument> GetCoach(string id)
        {
            return Ok(_coachesService.Get(id));
        }
        
        [HttpPost("CreateCoach")]
        public ActionResult<CoachesDocument> CreateCoach(CoachesDocument coach)
        {
            _coachesService.Create(coach);

            return CreatedAtRoute("GetCoach", new { id = coach.Id.ToString() }, coach);
        }
    }
}