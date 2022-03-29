using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TravianManager.Core;
using TravianManager.Core.Data;
using TravianManager.Core.DataProvider;
using TravianManager.Core.Managers;
using TravianManager.Data.Data;

namespace TravianManager.BusinessLogic.Managers
{
    public class PlanManager : IPlanManager
    {
        private readonly ILogger _logger;
        private readonly IPlanDataProvider _planDataProvider;

        public PlanManager(ILogger logger, IPlanDataProvider planDataProvider)
        {
            _logger = logger;
            _planDataProvider = planDataProvider;
        }

        public async Task<bool> AddPlan(Plan plan)
        {
            _logger.LogInformation("Starting Login");
            try
            {
               
                return _planDataProvider.AddPlan(plan);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<IEnumerable<Plan>> GetPlans(int userID)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.GetPlans(userID);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<Plan> GetPlanData(int planID)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.GetPlanData(planID);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<Target> AddOrUpdateTarget(Target target)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.AddOrUpdateTarget(target);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<PlanAttacker> AddOrUpdatePlanAttacker(PlanAttacker planAttacker)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.AddOrUpdatePlanAttacker(planAttacker);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }


        public async Task<IEnumerable<Target>> GetTargets(int planId)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                return await _planDataProvider.GetTargets(planId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<IEnumerable<PlanAttacker>> GetPlanAttackers(int planId)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                return await _planDataProvider.GetPlanAttackers(planId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        
        public async Task DeletePlanAttacker(int planAttackerId)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                await _planDataProvider.DeletePlanAttacker(planAttackerId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task DeleteTarget(int targetId)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                await _planDataProvider.DeleteTarget(targetId);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<PlanDefender> AddOrUpdateAttackPlan(AttackPlanData attackPlanData)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.AddOrUpdateAttackPlan(attackPlanData);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<IEnumerable<PlanDefender>> AddOrUpdateAllAttackPlan(AttackPlanData attackPlanData)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                return await _planDataProvider.AddOrUpdateAllAttackPlan(attackPlanData);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task DeletePlanDefender(int PlanDefenderID)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                await _planDataProvider.DeletePlanDefender(PlanDefenderID);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task<PlanSetting> GetPlanSettings(int planID)
        {
            _logger.LogInformation("Starting Login");
            try
            {
                return await _planDataProvider.GetPlanSettings(planID);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }

        public async Task UpdatePlanSettings(PlanSetting planSettings)
        {
            _logger.LogInformation("Starting Login");
            try
            {

                await _planDataProvider.UpdatePlanSettings(planSettings);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error Login");
                throw;
            }
        }
    }
}
