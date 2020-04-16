using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using thecrims_bot.models;
using thecrims_bot.parser;

namespace thecrims_bot.services
{

    public class TCServices
    {

        private HttpClient client;
        private HttpClientHandler handler;
        private CookieContainer cookies;
        private Uri url;

        public bool logged;
        public User user { get; set; }
        public Robberies rob { get; set; }
        public List<Robberies> robberies { get; set; }
        public List<Nightclubs> nightclubs { get; set; }
        public List<Drug> drugs { get; set; }

        public TCServices()
        {
            url = new Uri("https://www.thecrims.com/");
            cookies = new CookieContainer();
            user = new User();
            rob = new Robberies();
            robberies = new List<Robberies>();
            nightclubs = new List<Nightclubs>();
            drugs = new List<Drug>();
            handler = new HttpClientHandler() { CookieContainer = cookies };
            client = new HttpClient(handler) { BaseAddress = url };
            client.DefaultRequestHeaders.Clear();
            this.logged = false;
        }

        public async Task LoginAsync(string user, string password)
        {

            var dict = new Dictionary<string, string>();
            dict.Add("username", user);
            dict.Add("password", password);
            var req = new HttpRequestMessage(HttpMethod.Post, "https://www.thecrims.com/login") { Content = new FormUrlEncodedContent(dict) };
            var res = await client.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {

                //await getUser();
                //await getRobberies();
                //await Rob();
                //await getNightclubs();
                //await enterNightclub();
                await setXRequest();
                Console.WriteLine("Logado com sucesso!");
                this.logged = true;                
            }
            
            //await Roubar();
        }

        public async Task setXRequest()
        {
            TCParser parser = new TCParser();

            var getNewspaper = await client.GetAsync("newspaper#/newspaper");
            getNewspaper.EnsureSuccessStatusCode();
            string newspaperHtml = getNewspaper.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            string xrequest = parser.getRequest(newspaperHtml);
            this.client.DefaultRequestHeaders.Add("x-request", xrequest);
        }

        public async Task getUser()
        {
            TCParser parser = new TCParser();

            var getTasks = await client.GetAsync("api/v1/user/tasks");
            getTasks.EnsureSuccessStatusCode();
            string jsonTasks = getTasks.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.user = parser.parseUser(jsonTasks);
        }

        public async Task getRobberies()
        {
            TCParser parser = new TCParser();

            var getRobberies = await client.GetAsync("api/v1/robberies");
            getRobberies.EnsureSuccessStatusCode();
            string jsonRobberies = getRobberies.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.robberies = parser.parseRobberies(jsonRobberies);
            this.rob = getBestRob();

        }

        public async Task getNightclubs()
        {
            TCParser parser = new TCParser();

            var getNightclubs = await client.GetAsync("api/v1/nightclubs");
            getNightclubs.EnsureSuccessStatusCode();
            string jsonNightclubs = getNightclubs.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.nightclubs = parser.parseNightclubs(jsonNightclubs);

        }

        public async Task enterNightclub()
        {
            await getNightclubs();

            Nightclubs nightclub = new Nightclubs();

            nightclub = this.nightclubs.Where(w => w.business_id == 1).First();

            Console.WriteLine("Entrando na " + nightclub.name);

            string jsonEnterNightclub = "{\"id\": \"" + nightclub.id.ToString() + "\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var enterNightClub = await client.PostAsync("api/v1/nightclub", new StringContent(jsonEnterNightclub, Encoding.UTF8, "application/json"));
            enterNightClub.EnsureSuccessStatusCode();
            
            string jsonDrugs = enterNightClub.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            await buyDrugs(jsonDrugs);

            string jsonExitNightClub = "{\"exit_key\": \"" + nightclub.id.ToString() + "\", \"e_at\":null, \"reason\":\"Manual exit\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var exitNightClub = await client.PostAsync("api/v1/nightclub/exit", new StringContent(jsonExitNightClub, Encoding.UTF8, "application/json"));
            exitNightClub.EnsureSuccessStatusCode();           

        }

        public async Task buyDrugs(string jsonDrugs)
        {

            TCParser parser = new TCParser();

            this.drugs = parser.parseDrugs(jsonDrugs);

            Console.WriteLine("Comprando droga");

            string jsonBuyDrugs = "{\"id\": " + this.drugs[0].id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var buyDrugs = await client.PostAsync("api/v1/nightclub/drug", new StringContent(jsonBuyDrugs, Encoding.UTF8, "application/json"));
            //buyDrugs.EnsureSuccessStatusCode();

        }

        public async Task Rob()
        {

            await getRobberies();

            if (this.rob.energy > this.user.stamina)
            {
                await enterNightclub();
            }

            Console.WriteLine("Roubando " + this.rob.translated_name);

            string jsonRob = "{\"id\": " + this.rob.id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var rob = await client.PostAsync("api/v1/rob", new StringContent(jsonRob, Encoding.UTF8, "application/json"));

        }

        public Robberies getBestRob()
        {

            return this.robberies.OrderByDescending(id => id.id).First(x => x.successprobability >= 90);

        }

    }
}
