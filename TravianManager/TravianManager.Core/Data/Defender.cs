using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class Defender
    {
        public int DefenderID { get; set; }
        public int AttackerID { get; set; }
        public int AccountID { get; set; }
        public int TemplateID { get; set; }
        public string ArrivingTime { get; set; }
        public string AttackingTime { get; set; }
        public int AttackType { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }
}
