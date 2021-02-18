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
        private readonly DivisionalFinalsProcessService _divisionalFinalsProcessService;
        private readonly RegularSeasonWeekProcessService _regularSeasonWeekProcessService;

        public GameController(ILogger<GameController> logger, 
            GameService gameService, 
            NewSeasonProcessService newSeasonProcessService, 
            DivisionalFinalsProcessService divisionalFinalsProcessService,
            RegularSeasonWeekProcessService regularSeasonWeekProcessService)
        {
            _logger = logger;
            _gameService = gameService;
            _newSeasonProcessService = newSeasonProcessService;
            _divisionalFinalsProcessService = divisionalFinalsProcessService;
            _regularSeasonWeekProcessService = regularSeasonWeekProcessService;
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
            
            switch (current.NextWeekType)
            {
                case WeekType.EndSeason:
                    _newSeasonProcessService.SetUpNewSeason();
                    break;
                case WeekType.OffSeason:
                    break;
                case WeekType.DivisionalFinals:
                    _divisionalFinalsProcessService.PlayFinals();
                    break;
                case WeekType.Championship:

                    break;
                case WeekType.RegularSeason:
                    _regularSeasonWeekProcessService.PlayWeek();
                    break;
            }

            return Ok(_gameService.Get());
        }
    }
}