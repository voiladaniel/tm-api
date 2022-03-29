using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravianManager.Core.Data
{
    public class Target
    {
        public int TargetID { get; set; }
        public int AccountID { get; set; }
        public int? PlanID { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }

        [ForeignKey("TargetID")]
        public IEnumerable<PlanDefender> PlanDefender { get; set; }
    }
}
