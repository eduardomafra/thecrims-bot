using System;
using System.Collections.Generic;
using System.Text;

namespace thecrims_bot.models
{
    public class invitation
    {
        public int status { get; set; }
        public int id { get; set; }
        public int respect { get; set; }
        public string username { get; set; }
        public string under_protection { get; set; }
        public string raw_avatar { get; set; }
        public string avatar { get; set; }
        public object level { get; set; }
        public string country { get; set; }
        public string level_text_name { get; set; }
        public string character_text_name { get; set; }
        public string username_link { get; set; }
        public string flag { get; set; }
        public string under_protection_text { get; set; }
    }

    public class PlannedRobbery
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
        public string user_power { get; set; }
        public string long_name { get; set; }
        public int required_members { get; set; }
        public int energy_per_participant { get; set; }
        public bool cover { get; set; }
        public bool @virtual { get; set; }
        public List<invitation> invitations { get; set; }
        public string push_id { get; set; }
    }
}
