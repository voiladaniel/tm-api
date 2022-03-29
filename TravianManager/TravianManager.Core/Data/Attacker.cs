using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TravianManager.Core.Data
{
    public class Attacker
    {
        public int AttackerID { get; set; }
        public int AccountID { get; set; }
        public int TemplateID { get; set; }
        public int TournamentSquare { get; set; }
        public int TroopSpeed { get; set; }
        public string NotBeforeTime { get; set; }
        public string SpeedArtifact { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
        
        public IEnumerable<Defender> Defender { get; set; }
    }
}
