using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core.Managers
{
    public interface IPlanManager
    {
        Task<bool> AddPlan(Plan plan);

        Task<IEnumerable<Plan>> GetPlans(int userID);

        Task<Plan> GetPlanData(int planID);

        Task<Target> AddOrUpdateTarget(Target target);

        Task<IEnumerable<Target>> GetTargets(int planId);

        Task DeleteTarget(int targetId);

        Task<PlanAttacker> AddOrUpdatePlanAttacker(PlanAttacker planAttacker);

        Task<IEnumerable<PlanAttacker>> GetPlanAttackers(int planId);

        Task<PlanDefender> AddOrUpdateAttackPlan(AttackPlanData attackPlanData);

        Task DeletePlanDefender(int PlanDefenderID);

        Task<PlanSetting> GetPlanSettings(int planID);

        Task UpdatePlanSettings(PlanSetting planSettings);

        Task DeletePlanAttacker(int planAttackerId);

        Task<IEnumerable<PlanDefender>> AddOrUpdateAllAttackPlan(AttackPlanData attackPlanData);
    }
}
