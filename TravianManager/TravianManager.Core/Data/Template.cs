using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class Template
    {
        public int TemplateID { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
    }
}
