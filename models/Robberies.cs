using System;
using System.Collections.Generic;
using System.Text;

namespace thecrims_bot.models
{
    public class Robberies
    {
        public int id { get; set; }
        public string name { get; set; }
        public int difficulty { get; set; }
        public int energy { get; set; }
        public string spirit { get; set; }
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

    }
}
