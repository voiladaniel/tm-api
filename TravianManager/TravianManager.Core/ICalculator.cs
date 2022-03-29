using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TravianManager.Core.Data;

namespace TravianManager.Core
{
    public interface ICalculator
    {
        TimeSpan CalculateDistance(Coordinate coordOrigin, Coordinate coordTarget, double speed, int arena);

        long FindMinDifference(List<long> list);

        void RefreshDataPerAttacker(int AttackerID);

        Task RefreshDataPerTemplate(int templateID);

        Attacker GetSpies(int AttackerID, int SpyID, int TroopSpeed, int TournamentSquare);

        Task RefreshDataPerPlanDefender(int PlanDefenderID);

        Task RefreshDataPerTarget(int TargetID);

        Task<IEnumerable<Target>> RefreshDataPerPlan(int PlanID);

        Task<IEnumerable<Target>> RefreshDataPerPlanAndPlanAttacker(int PlanID, int PlanAttackerID);
    }
}
