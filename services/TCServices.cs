using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using thecrims_bot.models;
using thecrims_bot.parser;
using Console = Colorful.Console;

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

        public TCParser parser;

        public string msgRip;

        public TCServices()
        {
            url = new Uri("https://www.thecrims.com/");
            cookies = new CookieContainer();
            user = new User();
            rob = new Robberies();
            robberies = new List<Robberies>();
            nightclubs = new List<Nightclubs>();
            drugs = new List<Drug>();
            parser = new TCParser();
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
                await setXRequest();
                await getUser();
                Console.WriteLine("Logado com sucesso!");
                this.logged = true;                
            }
            
        }

        public async Task setXRequest()
        {

            var getNewspaper = await client.GetAsync("newspaper#/newspaper");
            getNewspaper.EnsureSuccessStatusCode();
            string newspaperHtml = getNewspaper.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            string xrequest = parser.getRequest(newspaperHtml);
            this.client.DefaultRequestHeaders.Add("x-request", xrequest);
        }

        public async Task getUser()
        {

            var getTasks = await client.GetAsync("api/v1/user/tasks");
            getTasks.EnsureSuccessStatusCode();
            string jsonTasks = getTasks.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.user = parser.parseUser(jsonTasks);
        }

        public async Task getRobberies()
        {

            string jsonRobberies = "";

            try
            {
                var getRobberies = await client.GetAsync("api/v1/robberies");
                getRobberies.EnsureSuccessStatusCode();
                jsonRobberies = getRobberies.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                this.robberies = parser.parseRobberies(jsonRobberies);
                this.rob = getBestRob();
            }
            catch
            {              
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await getRobberies();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await getRobberies();
                }
            }                      

        }

        public async Task getNightclubs()
        {

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

            Console.WriteLine("\nEntrando na " + nightclub.name, Color.BlueViolet);

            string jsonEnterNightclub = "{\"id\": \"" + nightclub.id.ToString() + "\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var enterNightClub = await client.PostAsync("api/v1/nightclub", new StringContent(jsonEnterNightclub, Encoding.UTF8, "application/json"));
            enterNightClub.EnsureSuccessStatusCode();
            var enterNightClubGet = await client.GetAsync("api/v1/nightclub");

            string jsonDrugs = enterNightClubGet.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            await buyDrugs(jsonDrugs);

            Console.WriteLine("Saindo da " + nightclub.name + "\n", Color.BlueViolet);

            string jsonExitNightClub = "{\"exit_key\": \"" + nightclub.id.ToString() + "\", \"e_at\":null, \"reason\":\"Manual exit\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var exitNightClub = await client.PostAsync("api/v1/nightclub/exit", new StringContent(jsonExitNightClub, Encoding.UTF8, "application/json"));
            exitNightClub.EnsureSuccessStatusCode();           

        }

        public async Task buyDrugs(string jsonDrugs)
        {

            this.drugs = parser.parseDrugs(jsonDrugs);

            Console.WriteLine("Comprando " + this.drugs[0].name, Color.Fuchsia);

            string jsonBuyDrugs = "{\"id\": " + this.drugs[0].id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var buyDrugs = await client.PostAsync("api/v1/nightclub/drug", new StringContent(jsonBuyDrugs, Encoding.UTF8, "application/json"));

        }

        public async Task Rob()
        {

            await getRobberies();

            if (this.rob.energy > this.user.stamina)
            {
                await enterNightclub();
            }

            Console.WriteLine("Roubando " + this.rob.translated_name, Color.Yellow);

            string jsonRob = "{\"id\": " + this.rob.id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";

            try
            {
                var rob = await client.PostAsync("api/v1/rob", new StringContent(jsonRob, Encoding.UTF8, "application/json"));
                string stringRob = rob.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                this.user = parser.parseUser(stringRob);
                Console.WriteLine("Sucesso! " + "Respeito: " + this.user.respect + " Inteligência: " + this.user.intelligence + " Força: " + this.user.strength + " Carisma: " + this.user.charisma + " Resistência: " + this.user.tolerance, Color.Green);
                Console.WriteLine("Estamina: " + this.user.stamina + "%" + " Vício: " + this.user.addiction + "%" + " Tickets: " + this.user.tickets, Color.Green);
                Console.WriteLine("Grana: " + this.user.cash, Color.Green);
            }
            catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...");
                    Thread.Sleep(5 * 60 * 1000);
                    await Rob();
                }
                else
                {

                    Console.Write("Tentando novamente...");
                    await Rob();
                }
            }
        }

        public Robberies getBestRob()
        {

            return this.robberies.OrderByDescending(id => id.id).First(x => x.successprobability >= 90);

        }

        public async Task<bool> getRip()
        {
            try
            {
                var getRip = await client.GetAsync("api/v1/rip");
                getRip.EnsureSuccessStatusCode();
                string jsonRip = getRip.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string mensagem = parser.parseRip(jsonRip);

                if (string.IsNullOrEmpty(mensagem))
                {
                    return await Task.FromResult(false);
                } else
                {
                    this.msgRip = mensagem;
                    return await Task.FromResult(true);
                }

            }
            catch
            {
                return await getRip();
            }
            
        }

    }
}
