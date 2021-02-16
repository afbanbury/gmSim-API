using System.Collections.Generic;
using Entities.Models;
using gmSim_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gmSim_API.Controllers
{
    [ApiController]
    [Route("api/gms")]
    public class GeneralManagersController : ControllerBase
    {
        private readonly ILogger<GeneralManagersController> _logger;
        private readonly GeneralManagersService _generalManagersService;

        public GeneralManagersController(ILogger<GeneralManagersController> logger, GeneralManagersService generalManagersService)
        {
            _logger = logger;
            _generalManagersService = generalManagersService;
        }

        [HttpGet]
        public ActionResult<List<GeneralManagersDocument>> GetGeneralManagers()
        {
            return Ok(_generalManagersService.Get());
        }
        
        [HttpGet("{id}", Name ="GetGM")]
        public ActionResult<GeneralManagersDocument> GetGM(string id)
        {
            return Ok(_generalManagersService.Get(id));
        }
        
        [HttpPost("CreateGM")]
        public ActionResult<GeneralManagersDocument> CreateGM(GeneralManagersDocument generalManager)
        {
            _generalManagersService.Create(generalManager);

            return CreatedAtRoute("GetGM", new { id = generalManager.Id.ToString() }, generalManager);
        }       
    }
}