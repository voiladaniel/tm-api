using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TravianManager.Core.Data
{
    public class Account
    {
        public int AccountID { get; set; }
        public int AccountType { get; set; }
        public string Name { get; set; }
        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public int? UserID { get; set; }
    }
}
