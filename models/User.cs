using System;
using System.Collections.Generic;
using System.Text;

/*
json model


    
    "id": 17740644,
    "eid": "ZWxraGVtaJqelZaUlZVmyZlnlGhqlpprxm1onZabxG2Vl2htbJ1tmpY",
    "username": "lukintest",
    "respect": 1474,
    "credits": 0,
    "country": "br",
    "language": "br",
    "tolerance": 2420,
    "strength": 2043,
    "charisma": 912,
    "intelligence": 4312,
    "gang_points": 0,
    "total_gang_points": 121,
    "cash": 5276321,
    "bank": 0,
    "stamina": 3,
    "spirit_name": "Super drogado",
    "level": 2,
    "level_text_name": "LEVEL_ROBBER_NEWBIE",
    "assault_points": 1000,
    "character_text_name": "CHARACTER_ROBBER",
    "addiction": 21,
    "avatar": "\\/static\\/images\\/avatars\\/avatar_23.jpg",
    "under_protection": true,
    "tickets": 174,
    "alive": true,
    "in_prison": false,
    "prison_end_time": null,
    "prison_end_time_formatted": null,
    "rip_end_time": null,
    "rip_end_time_formatted": null,
    "show_ads": true,
    "vip": false,
    "nightclub_id": 0,
    "new_message": false,
    "new_relation": false,
    "new_temp_relation": false,
    "new_gang": false,
    "new_task": false,
    "robbery_power": 2214,
    "single_robbery_power": 2214,
    "gang_robbery_power": 1700,
    "assault_power": 1198,
    "can_favourite_nightclubs": false,
    "equipment": [
      "Serra-Eletrica",
      "Capacete de Combate"
    ],
    "push_id": "8b0faa78f3cfe94f0c85fbdb86fe557e",
    "skill_points": null,
    "gang_push_id": "916c4781e959dd64d0d8e9f4a97e87bd",
    "new_relation_sound": false,
    "is_crew": false,
    "is_test": false,
    "gang": {
      "id": 11252,
      "name": "GangTest",
      "country": "BR",
      "description": ""
    },
    "is_gang_leader": true,
    "is_co_leader": false,
    "mui": false,
    "show_welcome_message": false,
    "fingerprint": "6cffa67fd9b565e70d8875b6658b064c"

    */

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
