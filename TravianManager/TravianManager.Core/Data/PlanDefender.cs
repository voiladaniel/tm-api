using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class PlanDefender
    {
        public int PlanDefenderID { get; set; }
        public int PlanAttackerID { get; set; }
        public int AccountID { get; set; }
        public int TargetID { get; set; }
        public int PlanID { get; set; }
        public string ArrivingTime { get; set; }
        public string AttackingTime { get; set; }
        public int AttackType { get; set; }
        public int AttackerConflict { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }

        [ForeignKey("PlanAttackerID")]
        public PlanAttacker PlanAttacker { get; set; }

    }
}
