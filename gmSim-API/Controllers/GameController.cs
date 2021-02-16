using System.Collections.Generic;
using Entities.Models;
using gmSim_API.ProcessServices;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly GameService _gameService;
        private readonly NewSeasonProcessService _newSeasonProcessService;
        private readonly PlayWeekProcessService _playWeekProcessService;

        public GameController(ILogger<GameController> logger, GameService gameService, NewSeasonProcessService newSeasonProcessService, PlayWeekProcessService playWeekProcessService)
        {
            _logger = logger;
            _gameService = gameService;
            _newSeasonProcessService = newSeasonProcessService;
            _playWeekProcessService = playWeekProcessService;
        }

        [HttpGet]
        public ActionResult<GameDocument> GetGame()
        {
            return Ok(_gameService.Get());
        }
        
        [HttpPut( "NewSeason")]
        public ActionResult<GameDocument> NewSeason()
        {
            _newSeasonProcessService.SetUpNewSeason();
            return Ok(_gameService.Get());
        }
        
        [HttpPut( "PlayWeek")]
        public ActionResult<GameDocument> PlayWeek()
        {
            _playWeekProcessService.SetUpPlayWeek();
            return Ok(_gameService.Get());
        }
    }
}