using System;
using System.Collections.Generic;
using System.Text;

namespace TravianManager.Core.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
