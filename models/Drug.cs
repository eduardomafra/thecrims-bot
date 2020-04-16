using System;
using System.Collections.Generic;
using System.Text;

namespace thecrims_bot.models
{
    public class Drug
    {
        public int id { get; set; }
        public int drug_id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public int stamina { get; set; }
        public string quantity { get; set; }
    }
}
