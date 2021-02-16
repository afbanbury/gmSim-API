using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private readonly PlayersService _playersService;

        public PlayersController(ILogger<PlayersController> logger, PlayersService playersService)
        {
            _logger = logger;
            _playersService = playersService;
        }

        [HttpGet]
        public ActionResult<List<PlayersDocument>> GetPlayers()
        {
            return Ok(_playersService.Get());
        }
        
        [HttpGet("{id}", Name ="GetPlayer")]
        public ActionResult<PlayersDocument> GetPlayer(string id)
        {
            return Ok(_playersService.Get(id));
        }
        
        [HttpPost("CreatePlayer")]
        public ActionResult<PlayersDocument> CreatePlayer(PlayersDocument player)
        {
            _playersService.Create(player);

            return CreatedAtRoute("GetGM", new { id = player.Id.ToString() }, player);
        }      
    }
}