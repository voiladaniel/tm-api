using System;
using System.Collections.Generic;
using System.Text;

namespace TravianManager.Core.Data
{
    public class AttackPlanData
    {
        public int PlanID { get; set; }
        public int PlanAttackerID { get; set; }
        public int PlanDefenderID { get; set; }
        public int TargetID { get; set; }
        public bool AttackType { get; set; }
        public string ArrivingTime { get; set; }
    }
}
