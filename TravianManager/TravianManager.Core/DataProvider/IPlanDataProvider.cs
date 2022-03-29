using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core.DataProvider
{
    public interface IPlanDataProvider
    {
        bool AddPlan(Plan plan);

        Task<IEnumerable<Plan>> GetPlans(int userID);

        Task<Plan> GetPlanData(int planID);

        Task<Target> AddOrUpdateTarget(Target target);

        Task<IEnumerable<Target>> GetTargets(int planId);

        Task DeleteTarget(int TargetID);

        Task<PlanAttacker> AddOrUpdatePlanAttacker(PlanAttacker planAttacker);

        Task<IEnumerable<PlanAttacker>> GetPlanAttackers(int planId);

        Task<PlanDefender> AddOrUpdateAttackPlan(AttackPlanData attackPlanData);

        Task DeletePlanDefender(int PlanDefenderID);

        PlanDefender GetPlanDefender(int planDefenderID);

        void UpdatePlanDefender(PlanDefender planDefender);

        PlanSetting GetPlanSetting(int planID);

        Task<PlanSetting> GetPlanSettings(int planID);

        Task UpdatePlanSettings(PlanSetting planSetting);

        PlanSetting GetNotAsyncPlanSetting(int planID);

        Task DeletePlanAttacker(int PlantAttackerID);

        Target GetTarget(int targetID);

        void UpdatePlanDefenders(List<PlanDefender> planDefender);

        Task<IEnumerable<PlanDefender>> AddOrUpdateAllAttackPlan(AttackPlanData attackPlanData);
    }
}
