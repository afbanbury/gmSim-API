using System;
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

        [HttpPost ("NextWeek", Name ="NextWeek")]
        public ActionResult NextWeek()
        {
            var current = _gameService.Get();

            Console.WriteLine("Next Week Type" + current.NextWeekType);
            switch (current.NextWeekType)
            {
                case WeekType.EndSeason:
                    Console.WriteLine("Process new season");
                    _newSeasonProcessService.SetUpNewSeason();
                    break;
                case WeekType.OffSeason:
                    break;
                case WeekType.PostSeason:

                    break;
                case WeekType.RegularSeason:
                    _playWeekProcessService.SetUpPlayWeek();
                    break;
            }

            return Ok(_gameService.Get());
        }
    }
}