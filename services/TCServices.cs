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
        public PlannedRobbery plannedRobbery { get; set; }
        public List<GangRobbery> gangRobbery { get; set; }
        public List<String> gangsUrl { get; set; }

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
            plannedRobbery = new PlannedRobbery();
            gangRobbery = new List<GangRobbery>();
            gangsUrl = new List<String>();
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

        public async Task Logout()
        {
            try
            {
                Console.WriteLine("Encerrando sessão...", Color.Green);
                var logout = await client.GetAsync("logout");
                logout.EnsureSuccessStatusCode();
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
            catch
            {
                return;
            }
            
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
                this.gangRobbery = parser.parseGangRobbery(jsonRobberies);
            }
            catch
            {              
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
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

            if (haveTickets())
            {

                await getNightclubs();

                Nightclubs nightclub = new Nightclubs();

                nightclub = this.nightclubs.Where(w => w.business_id == 1).First();


                try
                {
                    Console.Write("Entrando na " + nightclub.name + "...", Color.BlueViolet);

                    string jsonEnterNightclub = "{\"id\": \"" + nightclub.id.ToString() + "\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";

                    var enterNightClub = await client.PostAsync("api/v1/nightclub", new StringContent(jsonEnterNightclub, Encoding.UTF8, "application/json"));
                    enterNightClub.EnsureSuccessStatusCode();
                    var enterNightClubGet = await client.GetAsync("api/v1/nightclub");

                    string jsonDrugs = enterNightClubGet.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    await buyDrugs(jsonDrugs);
                }
                catch
                {
                    Console.WriteLine("Erro!", Color.Red);

                    if (await getRip() || await getPrison())
                    {
                        Console.WriteLine(this.msgRip, Color.DarkRed);
                        Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                        Thread.Sleep(5 * 60 * 1000);
                        await enterNightclub();
                    }
                    else
                    {

                        Console.Write("Tentando novamente...", Color.Yellow);
                        await enterNightclub();
                    }
                }


                try
                {
                    Console.Write(" Saindo da " + nightclub.name + "...\n", Color.BlueViolet);

                    string jsonExitNightClub = "{\"exit_key\": \"" + nightclub.id.ToString() + "\", \"e_at\":null, \"reason\":\"Manual exit\", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";

                    var exitNightClub = await client.PostAsync("api/v1/nightclub/exit", new StringContent(jsonExitNightClub, Encoding.UTF8, "application/json"));
                    exitNightClub.EnsureSuccessStatusCode();
                }
                catch
                {

                }
            }
            else
            {
                Console.WriteLine("Você está sem tickets!", Color.DarkRed);
                await Logout();
            }

        }

        public async Task buyDrugs(string jsonDrugs)
        {

            this.drugs = parser.parseDrugs(jsonDrugs);

            Console.Write(" Comprando " + this.drugs[0].name + "...", Color.Fuchsia);

            string jsonBuyDrugs = "{\"id\": " + this.drugs[0].id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var buyDrugs = await client.PostAsync("api/v1/nightclub/drug", new StringContent(jsonBuyDrugs, Encoding.UTF8, "application/json"));

        }

        public async Task Rob()
        {

            await getRobberies();

            if (this.rob.energy > this.user.stamina)
            {

                if(this.user.addiction >= 70)
                {
                    await Detox();
                }

                await enterNightclub();
                
            }

            Console.WriteLine("Roubando " + this.rob.translated_name, Color.Yellow);

            //string jsonRob = "{\"id\":19, \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            string jsonRob = "{\"id\": " + this.rob.id + ", \"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";

            try
            {
                var rob = await client.PostAsync("api/v1/rob", new StringContent(jsonRob, Encoding.UTF8, "application/json"));
                string stringRob = rob.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                this.user = parser.parseUser(stringRob);
                Console.Write("Sucess! ", Color.Green);
                //Console.WriteLine(this.user.ToString(), Color.Green);

            }
            catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await Rob();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await Rob();
                }
            }
        }

        public async Task gangRob()
        {
            await getGangRobbery();

            if (this.plannedRobbery != null)
            {

                bool executar = false;

                while (!executar)
                {

                    executar = await verifyGangRobbery();

                    if (executar)
                    {
                        Console.WriteLine("Executando roubo", Color.Yellow);
                        string jsonExecute = "{\"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
                        var robberyExecute = await client.PostAsync("api/v1/gangrobbery/execute", new StringContent(jsonExecute, Encoding.UTF8, "application/json"));
                        Console.WriteLine("Roubo em gangue efetuado com sucesso!", Color.Green);
                    }                   

                }

            }
            else
            {
                await createGangRobbery();
            }

        }

        public async Task<bool> verifyGangRobbery()
        {

            Console.WriteLine("Verificando se há usuários suficientes...", Color.Yellow);

            var plannedRobbery = await client.GetAsync("api/v1/robberies");
            string jsonPlannedRobbery = plannedRobbery.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.plannedRobbery = parser.parsePlannedRobbery(jsonPlannedRobbery);

            int invitations = this.plannedRobbery.invitations.Where(s => s.status == 1).Count();
            
            if(invitations == this.plannedRobbery.required_members)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }

        }

        public async Task getGangRobbery()
        {
            this.plannedRobbery = null;

            Console.WriteLine("Verificando se há roubo em gangue disponível...", Color.Yellow);

            var plannedRobbery = await client.GetAsync("api/v1/robberies");
            string jsonPlannedRobbery = plannedRobbery.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            this.plannedRobbery = parser.parsePlannedRobbery(jsonPlannedRobbery);
            this.user = parser.parseUser(jsonPlannedRobbery);

            if (this.plannedRobbery != null)
            {

                List<invitation> invitation = new List<invitation>();
                invitation = this.plannedRobbery.invitations;

                int verifyAccept = invitation.Where(s => s.username == "*" + this.user.username && s.status == 1).Count();

                if (verifyAccept != 1)
                {
                    if (this.plannedRobbery.energy_per_participant > this.user.stamina)
                    {

                        if (this.user.addiction >= 70)
                        {
                            await Detox();
                        }

                        await enterNightclub();
                    }

                    await acceptGangRobbery();
                }

            }

        }

        public async Task acceptGangRobbery()
        {

            Console.WriteLine("Aceitando roubo em gangue...", Color.Yellow);

            string jsonAccept = "{\"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";

            try
            {

                var robberyAccept = await client.PostAsync("api/v1/gangrobbery/accept", new StringContent(jsonAccept, Encoding.UTF8, "application/json"));
                string stringAccept = robberyAccept.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //this.plannedRobbery = parser.parsePlannedRobbery(stringAccept);
                this.user = parser.parseUser(stringAccept);

            } catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await getVirtualGangRobbery();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await getVirtualGangRobbery();
                }
            }
        }

        public async Task createGangRobbery()
        {
            await getRobberies();

            if (getBestGangRobbery().energy_per_participant > this.user.stamina)
            {

                if (this.user.addiction >= 70)
                {
                    await Detox();
                }

                await enterNightclub();
            }

            Console.WriteLine("Criando roubo em gangue...", Color.Yellow);

            
            int id = getBestGangRobbery().id;

            string jsonCreateGangRobbery = "{\"id\":" + "2," + "\"input_counters\":{}, \"action_timestamp\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + "}";
            var gangRob = await client.PostAsync("api/v1/gangrobbery/plan", new StringContent(jsonCreateGangRobbery, Encoding.UTF8, "application/json"));

            //string jsonGangRobbery = gangRob.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        }

        public async Task Detox()
        {

            Console.WriteLine("\nAplicando Detox...\n", Color.BlueViolet);

            try
            {
                var detox = await client.PostAsync("api/v1/hospital/detox", null);
            }
            catch
            {
                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await Detox();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await Detox();
                }
            }
        }

        public Robberies getBestRob()
        {

            return this.robberies.OrderByDescending(id => id.id).First(x => x.successprobability >= 100);

        }

        public GangRobbery getBestGangRobbery()
        {

            return this.gangRobbery.OrderByDescending(id => id.id).First(x => x.successprobability >= 100);

        }

        public async Task<bool> getRip()
        {
            try
            {
                Console.WriteLine("Verificando se foi para o hospital...\n", Color.OrangeRed);
                var getRip = await client.GetAsync("api/v1/rip");
                getRip.EnsureSuccessStatusCode();
                string jsonRip = getRip.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string mensagem = parser.parseRip(jsonRip);

                if (string.IsNullOrEmpty(mensagem))
                {
                    Console.WriteLine("Ok!", Color.Green);
                    return await Task.FromResult(false);
                } else
                {
                    this.msgRip = mensagem;
                    Console.WriteLine("Você foi para o hospital!", Color.DarkRed);
                    return await Task.FromResult(true);
                }

            }
            catch
            {
                //return await getRip();
                return false;
            }
            
        }

        public async Task<bool> getPrison()
        {
            try
            {
                Console.WriteLine("Verificando se foi para a prisão...\n", Color.OrangeRed);
                var getPrison = await client.GetAsync("api/v1/prison");
                getPrison.EnsureSuccessStatusCode();
                string jsonPrison = getPrison.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                string mensagem = parser.parseRip(jsonPrison);

                if (string.IsNullOrEmpty(mensagem))
                {
                    Console.WriteLine("Ok!", Color.Green);
                    return await Task.FromResult(false);
                }
                else
                {
                    this.msgRip = mensagem;
                    Console.WriteLine("Você foi preso!", Color.DarkRed);
                    return await Task.FromResult(true);
                }

            }
            catch
            {
                //return await getPrison();
                return false;
            }

        }

        public async Task getVirtualGangs()
        {
            try
            {

                Console.WriteLine("Procurando gangue virtual...", Color.Yellow);
                var getGangs = await client.GetAsync("gang");
                getGangs.EnsureSuccessStatusCode();
                string htmlGangs = getGangs.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                this.gangsUrl = parser.getGangs(htmlGangs);
            }
            catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await getVirtualGangs();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await getVirtualGangs();
                }
            }

        }

        public async Task joinVirtualGang()
        {
            try
            {
                Console.WriteLine("Entrando na gangue virtual...", Color.YellowGreen);
                var dict = new Dictionary<string, string>();
                dict.Add("message", "I+wanna+join");
                var req = new HttpRequestMessage(HttpMethod.Post, "https://www.thecrims.com" + gangsUrl[3]) { Content = new FormUrlEncodedContent(dict) };
                var res = await client.SendAsync(req);

            }
            catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await joinVirtualGang();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await getVirtualGangs();
                    await joinVirtualGang();
                }
            }
        }

        public async Task getVirtualGangRobbery()
        {

            Console.WriteLine("Verificando roubo em gangue...", Color.Yellow);

            try
            {

                var plannedRobbery = await client.GetAsync("api/v1/gangrobbery");
                string jsonPlannedRobbery = plannedRobbery.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                this.plannedRobbery = parser.parseVirtualPlannedRobbery(jsonPlannedRobbery);
            }
            catch
            {
                Console.WriteLine("Erro!", Color.Red);

                if (await getRip() || await getPrison())
                {
                    Console.WriteLine(this.msgRip, Color.DarkRed);
                    Console.WriteLine("Tentando novamente em 5 minutos...", Color.Yellow);
                    Thread.Sleep(5 * 60 * 1000);
                    await getVirtualGangRobbery();
                }
                else
                {

                    Console.Write("Tentando novamente...", Color.Yellow);
                    await getVirtualGangRobbery();
                }
            }
            //this.user = parser.parseUser(jsonPlannedRobbery);

            if (this.plannedRobbery != null)
            {

                List<invitation> invitation = new List<invitation>();
                invitation = this.plannedRobbery.invitations;

                int verifyAccept = invitation.Where(s => s.username == "*" + this.user.username && s.status == 1).Count();

                if (verifyAccept != 1)
                {
                    if (this.plannedRobbery.energy_per_participant > this.user.stamina)
                    {

                        if (this.user.addiction >= 70)
                        {
                            await Detox();
                        }

                        await enterNightclub();
                    }

                    await acceptGangRobbery();
                } else
                {
                    Console.WriteLine("Já aceitou", Color.GreenYellow);
                }

            }

        }

        public bool haveTickets()
        {
            if (this.user.tickets < 1)
            {
                return false;
            } else
            {
                return true;
            }
        }

    }
}
