using System;
using System.Collections.Generic;
using System.Text;

namespace thecrims_bot.models
{
    public class User
    {
        public int id { get; set; }
        public string eid { get; set; }
        public string username { get; set; }
        public int respect { get; set; }
        public int tolerance { get; set; }
        public int strength { get; set; }
        public int charisma { get; set; }
        public int intelligence { get; set; }
        public int cash { get; set; }
        public int stamina { get; set; }
        public string spirit_name { get; set; }
        public int level { get; set; }
        public int addiction { get; set; }
        public int tickets { get; set; }
        public bool in_prision { get; set; }
    }
}
