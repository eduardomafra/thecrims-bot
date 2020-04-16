using System;
using System.Collections.Generic;
using System.Text;

namespace thecrims_bot.models
{
    public class Nightclubs
    {
        public string id { get; set; }
        public string name { get; set; }
        public int entrance_fee { get; set; }
        public object min_respect { get; set; }
        public object level { get; set; }
        public object description { get; set; }
        public int type { get; set; }
        public int business_id { get; set; }
        public string image { get; set; }
        public string owner_id { get; set; }
        public int user_id { get; set; }
        public bool anonymous { get; set; }
        public object num_items_for_sale { get; set; }
    }
}
