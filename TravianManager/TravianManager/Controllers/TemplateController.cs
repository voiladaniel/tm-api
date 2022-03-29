using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravianManager.Core;
using TravianManager.Core.Data;
using TravianManager.Core.Managers;

namespace TravianManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ICalculator _calculator;
        private readonly ITemplateManager _templateManager;
        private readonly ILogger _logger;
        public TemplateController(ITemplateManager templateManager, ICalculator calculator, ILogger logger)
        {
            _calculator = calculator;
            _templateManager = templateManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("DeletePlan")]
        public async Task<ActionResult> DeletePlan([FromBody] Template plan)
        {
            try
            {
                await _templateManager.DeletePlan(plan);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("AddPlan")]
        public async Task<ActionResult> AddPlan([FromBody] Template plan)
        {
            try
            {
                var newPlan = await _templateManager.AddPlan(plan);

                return Ok(newPlan);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetDefensePlans")]
        public async Task<ActionResult> GetDefensePlans([FromQuery] int UserID)
        {
            try
            {
                var plans = await _templateManager.GetDefensePlans(UserID);
                return Ok(plans.Select(x => new
                {
                    x.UserID,
                    x.TemplateID,
                    x.Name
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetAttackers")]
        public async Task<ActionResult> GetAttackers([FromQuery] int TemplateID, [FromQuery] int UserID)
        {
            try
            {
                var attackers = await _templateManager.GetAttackers(TemplateID, UserID);

                return Ok(attackers);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetSpies")]
        public async Task<ActionResult> GetSpies([FromQuery] int AttackerID, [FromQuery] int SpyID, [FromQuery] int TroopSpeed, [FromQuery] int TournamentSquare)
        {
            try
            {
                var attacker =  _calculator.GetSpies(AttackerID, SpyID, TroopSpeed, TournamentSquare);

                return Ok(attacker);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetDefenders")]
        public async Task<ActionResult> GetDefenders([FromQuery] int TemplateID, [FromQuery] int UserID)
        {
            try
            {
                var attackers = await _templateManager.GetDefenders(TemplateID, UserID);

                return Ok(attackers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("AddDefender")]
        public async Task<ActionResult> AddDefender([FromBody] Defender defender)
        {
            try
            {
                var result = await _templateManager.AddDefender(defender);
                if(result)
                    _calculator.RefreshDataPerAttacker(defender.AttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("UpdateAttacker")]
        public async Task<ActionResult> UpdateAttacker([FromBody] Attacker attacker)
        {
            try
            {
                await _templateManager.UpdateAttacker(attacker);
                _calculator.RefreshDataPerAttacker(attacker.AttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("DeleteDefender")]
        public async Task<ActionResult> DeleteDefender([FromQuery] int DefenderID, [FromQuery] int AttackerID)
        {
            try
            {
                await _templateManager.DeleteDefender(DefenderID);
                _calculator.RefreshDataPerAttacker(AttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("DeleteDefenders")]
        public async Task<ActionResult> DeleteDefenders([FromQuery] int AttackerID)
        {
            try
            {
                await _templateManager.DeleteDefenders(AttackerID);
                _calculator.RefreshDataPerAttacker(AttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetSettings")]
        public async Task<ActionResult> GetSettings([FromQuery] int TemplateID)
        {
            try
            {
                var settings = await _templateManager.GetSettings(TemplateID);

                return Ok(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("UpdateSettings")]
        public async Task<ActionResult> UpdateSettings([FromBody] Setting settings)
        {
            try
            {
                await _templateManager.UpdateSettings(settings);
                await _calculator.RefreshDataPerTemplate(settings.TemplateID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("DeleteAttacker")]
        public async Task<ActionResult> DeleteAttacker([FromQuery] int AttackerID)
        {
            try
            {
                await _templateManager.DeleteAttacker(AttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }
    }


}