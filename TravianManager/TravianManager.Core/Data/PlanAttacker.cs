using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class PlanAttacker
    {
        public int PlanAttackerID { get; set; }
        public int AccountID { get; set; }
        public int PlanID { get; set; }
        public int TournamentSquare { get; set; }
        public int TroopSpeed { get; set; }
        public string SpeedArtifact { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }
}
