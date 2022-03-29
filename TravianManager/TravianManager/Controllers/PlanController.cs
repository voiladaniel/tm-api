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
    public class PlanController : ControllerBase
    {
        private readonly IPlanManager _planManager;
        private readonly ICalculator _calculator;
        private readonly ILogger _logger;
        public PlanController(ICalculator calculator, IPlanManager planManager, ILogger logger)
        {
            _calculator = calculator;
            _planManager = planManager;
            _logger = logger;
        }

        //[HttpGet]
        //[Route("GetAttackers")]
        //public async Task<ActionResult> GetAttackers([FromQuery] int TemplateID, [FromQuery] int UserID)
        //{
        //    try
        //    {
        //        var attackers = await _templateManager.GetAttackers(TemplateID, UserID);

        //        return Ok(attackers);
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
        //        return BadRequest(ex);
        //    }
        //}

        [HttpPost]
        [Route("AddPlan")]
        public async Task<ActionResult> AddPlan([FromBody] Plan plan)
        {
            try
            {
                await _planManager.AddPlan(plan);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetPlans")]
        public async Task<ActionResult> GetPlans([FromQuery] int UserID)
        {
            try
            {
                var plans = await _planManager.GetPlans(UserID);
                return Ok(plans.Select(x => new
                {
                    x.UserID,
                    x.PlanID,
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
        [Route("GetPlanData")]
        public async Task<ActionResult> GetPlanData([FromQuery] int PlanID)
        {
            try
            {
                var plan = await _planManager.GetPlanData(PlanID);
                var setting = await _planManager.GetPlanSettings(PlanID);
                return Ok( new
                {
                    plan.PlanID,
                    plan.Name,
                    setting.Message,
                    setting.IncludeTTA,
                    setting.IncludeTTL,
                    setting.FakeMessage,
                    setting.RealMessage,
                    setting.TTAMessage,
                    setting.TTLMessage
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("AddOrUpdateTarget")]
        public async Task<ActionResult> AddOrUpdateTarget([FromBody] Target target)
        {
            try
            {
                var result = await _planManager.AddOrUpdateTarget(target);
                await _calculator.RefreshDataPerTarget(result.TargetID);

                //Refrash plan!

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("AddOrUpdatePlanAttacker")]
        public async Task<ActionResult> AddOrUpdatePlanAttacker([FromBody] PlanAttacker planAttacker)
        {
            try
            {
                var attackerData = await _planManager.AddOrUpdatePlanAttacker(planAttacker);
                if(!planAttacker.PlanAttackerID.Equals(0))
                {
                    var target = await _calculator.RefreshDataPerPlan(planAttacker.PlanID);
                    return Ok(new { attackerData, target });
                }
                else
                {
                    return Ok(new { attackerData });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetTargets")]
        public async Task<ActionResult> GetTargets([FromQuery] int PlanID)
        {
            try
            {
                var targets = await _planManager.GetTargets(PlanID);

                return Ok(targets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetPlanAttackers")]
        public async Task<ActionResult> GetPlanAttackers([FromQuery] int PlanID)
        {
            try
            {
                var targets = await _planManager.GetPlanAttackers(PlanID);

                return Ok(targets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }
        
        [HttpGet]
        [Route("DeletePlanAttacker")]
        public async Task<ActionResult> DeletePlanAttacker([FromQuery] int PlanAttackerID)
        {
            try
            {
                await _planManager.DeletePlanAttacker(PlanAttackerID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("DeleteTarget")]
        public async Task<ActionResult> DeleteTarget([FromQuery] int TargetID)
        {
            try
            {
                await _planManager.DeleteTarget(TargetID);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("AddOrUpdateAttackPlan")]
        public async Task<ActionResult> AddOrUpdateAttackPlan([FromBody] AttackPlanData attackPlanData)
        {
            try
            {
                if(attackPlanData.TargetID != -1)
                {
                    var result = await _planManager.AddOrUpdateAttackPlan(attackPlanData);
                    var targets = await _calculator.RefreshDataPerPlanAndPlanAttacker(attackPlanData.PlanID, attackPlanData.PlanAttackerID);
                    //_calculator.RefreshDataPerPlanDefender(result.PlanDefenderID);

                    return Ok(targets);
                }
                else
                {
                    var result = await _planManager.AddOrUpdateAllAttackPlan(attackPlanData);
                    var targets = await _calculator.RefreshDataPerPlan(attackPlanData.PlanID);

                    return Ok(targets);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("DeletePlanDefender")]
        public async Task<ActionResult> DeletePlanDefender([FromQuery] int PlanDefenderID, [FromQuery] int PlanAttackerID, [FromQuery] int PlanID)
        {
            try
            {
                await _planManager.DeletePlanDefender(PlanDefenderID);
                var targets = await _calculator.RefreshDataPerPlanAndPlanAttacker(PlanID, PlanAttackerID);

                return Ok(targets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("GetPlanSettings")]
        public async Task<ActionResult> GetPlanSettings([FromQuery] int planID)
        {
            try
            {
                var settings = await _planManager.GetPlanSettings(planID);

                return Ok(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("UpdatePlanSettings")]
        public async Task<ActionResult> UpdatePlanSettings([FromBody] PlanSetting planSetting)
        {
            try
            {
                await _planManager.UpdatePlanSettings(planSetting);
                var result = await _calculator.RefreshDataPerPlan(planSetting.PlanID);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

    }


}