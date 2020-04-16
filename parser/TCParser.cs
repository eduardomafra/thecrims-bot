using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using thecrims_bot.models;

namespace thecrims_bot.parser
{
    public class TCParser
    {
        //string html = "{\"menus\":{\"left\":[{\"title\":\"Porradas de gangue\",\"text\":\"Espanque algum infeliz\",\"sprite_id\":\"menu-sprite-assault\",\"id\":\"menu-assault\",\"vue_route\":\"assault\"},{\"title\":\"Roubar\",\"text\":\"Assaltos solo e gangue \\u00e9 aqui\",\"sprite_id\":\"menu-sprite-robbery\",\"id\":\"menu-robbery\",\"vue_route\":\"robberies\"},{\"title\":\"Vida Noturna\",\"text\":\"Passe a noite na balada\",\"sprite_id\":\"menu-sprite-nightlife\",\"id\":\"menu-nightlife\",\"vue_route\":\"nightlife\"},{\"title\":\"O Beco\",\"text\":\"Fa\\u00e7a algumas tarefas extras.\",\"sprite_id\":\"menu-sprite-alley\",\"id\":\"menu-alley\",\"url\":\"\\/alley\"},{\"title\":\"Putas\",\"text\":\"Cafetinagem \\u00e9 aqui mesmo.\",\"sprite_id\":\"menu-sprite-hookers\",\"id\":\"menu-hookers\",\"vue_route\":\"hookers\"},{\"title\":\"Mercado Negro\",\"text\":\"Armas, acess\\u00f3rios e seguran\\u00e7a.\",\"sprite_id\":\"menu-sprite-armsdealer\",\"id\":\"menu-armsdealer\",\"vue_route\":\"market-drug-components\"},{\"title\":\"Tr\\u00e1fico de Drogas\",\"text\":\"Compre e venda uns bagulhos\",\"sprite_id\":\"menu-sprite-drugdealer\",\"id\":\"menu-drugdealer\",\"vue_route\":\"drugdealer\"},{\"title\":\"Mercado de a\\u00e7\\u00f5es\",\"text\":\"Invista e fique rico!\",\"sprite_id\":\"menu-sprite-stockmarket\",\"id\":\"menu-stockmarket\",\"vue_route\":\"stockmarket\"},{\"title\":\"F\\u00e1bricas\",\"text\":\"Produza aqui suas pr\\u00f3prias drogas\",\"sprite_id\":\"menu-sprite-buildings\",\"id\":\"menu-buildings\",\"vue_route\":\"buildings\"},{\"title\":\"Distritos\",\"text\":\"Expanda o seu imp\\u00e9rio\",\"sprite_id\":\"menu-sprite-districts\",\"id\":\"menu-districts\",\"vue_route\":\"districts\"},{\"title\":\"Hospital\",\"text\":\"Vai pro hospital, seu doente!\",\"sprite_id\":\"menu-sprite-hospital\",\"id\":\"menu-hospital\",\"vue_route\":\"hospital\"},{\"title\":\"Banco\",\"text\":\"Deposite a sua grana num lugar seguro\",\"sprite_id\":\"menu-sprite-bank\",\"id\":\"menu-bank\",\"vue_route\":\"bank\"},{\"title\":\"Cassino\",\"text\":\"Tente a sorte! O azar \\u00e9 certo\",\"sprite_id\":\"menu-sprite-casino\",\"id\":\"menu-casino\",\"vue_route\":\"casino\"},{\"title\":\"Pris\\u00e3o\",\"text\":\"Foi parar no xadrez? \\u00c9, a vida \\u00e9 dura!\",\"sprite_id\":\"menu-sprite-prison\",\"id\":\"menu-prison\",\"vue_route\":\"prison\"}],\"right\":[{\"title\":\"Ajuda\",\"text\":\"Aqui voc\\u00ea pode obter ajuda\",\"sprite_id\":\"menu-sprite-help\",\"id\":\"menu-help\",\"url\":\"\\/help\"},{\"title\":\"As Docas\",\"text\":\"O que est\\u00e1 rolando no porto?\",\"sprite_id\":\"menu-sprite-docks\",\"vue_route\":\"docks\",\"id\":\"menu-docks\"},{\"title\":\"Centro de treinamento\",\"text\":\"Flexione os m\\u00fasculos!\",\"sprite_id\":\"menu-sprite-training\",\"vue_route\":\"training\",\"id\":\"menu-training\"},{\"title\":\"A Pra\\u00e7a\",\"text\":\"Aqui \\u00e9 onde a bandidagem se reuni...\",\"sprite_id\":\"menu-sprite-square\",\"vue_route\":\"square\",\"id\":\"menu-square\"}]},\"messages\":[],\"messages_prefix\":\"\",\"redirect_to\":false,\"reload\":false,\"route\":false,\"route_props\":[],\"user\":{\"id\":17736189,\"eid\":\"YnBulZeSnZptl5dolJHEnG7Hap5nxJZoZ2tnaWpqlJtjaZjEw5SVw2Y\",\"username\":\"botfodase\",\"respect\":213,\"credits\":0,\"country\":\"br\",\"language\":\"br\",\"tolerance\":430,\"strength\":423,\"charisma\":420,\"intelligence\":421,\"gang_points\":0,\"total_gang_points\":21,\"cash\":43336,\"bank\":0,\"stamina\":7,\"spirit_name\":\"Super drogado\",\"level\":1,\"level_text_name\":\"LEVEL_PROSPECT\",\"assault_points\":1000,\"character_text_name\":\"CHARACTER_GANGSTER\",\"addiction\":1,\"avatar\":\"\\/static\\/images\\/avatars\\/avatar_39.jpg\",\"under_protection\":false,\"tickets\":300,\"alive\":true,\"in_prison\":false,\"prison_end_time\":null,\"prison_end_time_formatted\":null,\"rip_end_time\":null,\"rip_end_time_formatted\":null,\"show_ads\":false,\"vip\":false,\"nightclub_id\":115,\"new_message\":false,\"new_relation\":false,\"new_temp_relation\":false,\"new_gang\":false,\"new_task\":false,\"robbery_power\":254,\"single_robbery_power\":254,\"gang_robbery_power\":255,\"assault_power\":212,\"can_favourite_nightclubs\":false,\"equipment\":[\"Nenhum item de equipamento\"],\"push_id\":\"f8e295730e4ba09bd6e5f1b69aaf8e92\",\"skill_points\":null,\"gang_push_id\":false,\"new_relation_sound\":false,\"is_crew\":false,\"is_test\":false,\"gang\":null,\"is_gang_leader\":false,\"is_co_leader\":false,\"mui\":false,\"show_welcome_message\":false,\"fingerprint\":\"bc243eae376cf1a37e18f0270df769c6\"},\"tutorials\":[{\"section\":\"bank\",\"type\":1,\"title\":\"Bank Tutorial \",\"youtube_id\":\"kAd6vUHDdtw\",\"guide\":\"\"},{\"section\":\"blackmarket\",\"type\":1,\"title\":\"Mercado Negro \",\"youtube_id\":\"ItohIXtKqU0\",\"guide\":\"\"},{\"section\":\"robbery\",\"type\":1,\"title\":\"ROUBO SOLO\\/GANGUE\",\"youtube_id\":\"F2dWwVox_qQ\",\"guide\":\"\"},{\"section\":\"docks\",\"type\":1,\"title\":\"Docas\",\"youtube_id\":\"NFRErNR7fYA\",\"guide\":\"\"},{\"section\":\"robbery\",\"type\":1,\"title\":\"COMO FAZER DOA\\u00c7\\u00c3O PARA GANGUE\",\"youtube_id\":\"WcQ8ZwLzfgs\",\"guide\":\"\"}],\"is_trusted_account\":false,\"server_timestamp\":1586990481,\"nightclub\":{\"business_id\":1,\"name\":\"Festas Rave\",\"entrance_fee\":0,\"min_respect\":null,\"level\":null,\"description\":null,\"earnings\":0,\"safe\":false,\"beginner_club\":false,\"buy_price\":25000,\"products\":{\"drugs\":[{\"id\":46857,\"drug_id\":8,\"name\":\"\\u00d3pio\",\"price\":\"$704\",\"stamina\":96,\"quantity\":\"\"}],\"hookers\":[]},\"bots\":[],\"channel\":\"presence-ce4f588559758f8f90184ac253958228\",\"bot_channel\":null,\"image\":\"\\/static\\/images\\/sections\\/businesses\\/rave.jpg\",\"timeout\":15,\"num_items_for_sale\":null,\"type\":\"nightclub\",\"editable\":true,\"sell_price\":12500,\"exit_key\":\"YmpsnZOSxpVkmMlvaJSclmjFa3BsmJ5lZG-XaJKcx55nbpua\",\"prices_with_drugs\":[{\"id\":46857,\"business_id\":115,\"drug_id\":8,\"price\":44,\"user_id\":17176859,\"quantity\":65252648,\"deletable\":true,\"drug\":{\"id\":8,\"name\":\"\\u00d3pio\",\"price\":89,\"satisfaction\":0.6,\"stamina\":6}}],\"business\":{\"id\":1,\"name\":\"Rave party\",\"type\":1,\"picture\":\"rave.jpg\",\"price\":25000,\"max_visitors\":100,\"day_price\":100,\"max_items_for_sale\":0,\"deleted_at\":null,\"is_nightclub\":true,\"is_whorehouse\":false,\"credit_buy_price\":10,\"buyable\":false,\"buy_price\":25000},\"prices\":[{\"id\":46857,\"business_id\":115,\"drug_id\":8,\"price\":44,\"deletable\":true}],\"anonymous\":true,\"timeout_at\":1586990496}}";
        public string getRequest(string html)
        {
            //html = this.html;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var xrequest = htmlDoc.DocumentNode.SelectSingleNode("//settings");

            return xrequest.Attributes[0].Value;
        }

        public User parseUser(string jsonTasks)
        {
            
            JObject jsonUser = JObject.Parse(jsonTasks);
            User user = JsonConvert.DeserializeObject<User>(jsonUser["user"].ToString());

            return user;
        }

        public List<Robberies> parseRobberies(string stringRobberies)
        {

            JObject jsonRobberies = JObject.Parse(stringRobberies);
            List<Robberies> robberies = JsonConvert.DeserializeObject<List<Robberies>>(jsonRobberies["single_robberies"].ToString());

            return robberies;
        }

        public List<Nightclubs> parseNightclubs(string stringNightclubs)
        {

            JObject jsonNightclubs = JObject.Parse(stringNightclubs);
            List<Nightclubs> nightclubs = JsonConvert.DeserializeObject<List<Nightclubs>>(jsonNightclubs["nightclubs"].ToString());

            return nightclubs;
        }

        public List<Drug> parseDrugs(string stringDrugs)
        {

            JObject jsonDrugs = JObject.Parse(stringDrugs);
            List<Drug> drugs = JsonConvert.DeserializeObject<List<Drug>>(jsonDrugs["nightclub"]["products"]["drugs"].ToString());

            return drugs;
        }

        //public string getChannel(string stringDrugs)
        //{
        //    stringDrugs = this.html;
        //    JObject jsonDrugs = JObject.Parse(stringDrugs);
        //    string channel = Convert.ToString(jsonDrugs["nightclub"]["channel"]);
        //    return channel;
        //}

    }
}
