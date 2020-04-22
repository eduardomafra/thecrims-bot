using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace thecrims_bot.models
{

    public class GangRobbery
    {
        public int id { get; set; }
        public object name { get; set; }
        public int difficulty { get; set; }
        public int energy { get; set; }
        public int spirit { get; set; }
        public string spiritname { get; set; }
        public int type { get; set; }
        public string translated_name { get; set; }
        public int max_reward { get; set; }
        public int min_reward { get; set; }
        public List<object> rewards { get; set; }
        public int user_power { get; set; }
        public int successprobability { get; set; }
        public string successprobability_image { get; set; }
        public string long_name { get; set; }
        public int required_members { get; set; }
        public int energy_per_participant { get; set; }
        public bool cover { get; set; }
    }

}
