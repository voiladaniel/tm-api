using System;
using System.Collections.Generic;
using System.Text;

namespace TravianManager.Core.Data
{
    public class PlanSetting
    {
        public int PlanSettingID { get; set; }
        public int TimeBuffer { get; set; }
        public int SafeTime { get; set; }
        public int PlanID { get; set; }
        public string Message { get; set; }

        public int IncludeTTA { get; set; }
        public int IncludeTTL { get; set; }
        public string FakeMessage { get; set; }
        public string RealMessage { get; set; }
        public string TTLMessage { get; set; }
        public string TTAMessage { get; set; }

        public int ServerSpeed { get; set; }

    }
}
