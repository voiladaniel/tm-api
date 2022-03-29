using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class Plan
    {
        public int PlanID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int UserID { get; set; }
    }
}
