using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using thecrims_bot.models;
using thecrims_bot.parser;
using thecrims_bot.services;
using Console = Colorful.Console;

namespace thecrims_bot.console
{
    public class TCComands
    {
        TCServices service;
        public TCComands()
        {
            service = new TCServices();
        }
        public void showInfo()
        {
            TCParser parser = new TCParser();
            User user = new User();
            user = parser.parseUser("a");

            var userTable = new ConsoleTable("Usuário", "Moral", "Respeito", "Tickets", "Stamina", "Vício", "Grana");
            userTable.AddRow(user.username, user.spirit_name, user.respect, user.tickets, user.stamina, user.addiction, user.cash);

            var userStatsTable = new ConsoleTable("Inteligência", "Força", "Carisma", "Resistência");
            userStatsTable.AddRow(user.intelligence, user.strength, user.charisma, user.tolerance);

            userTable.Options.EnableCount = false;
            userTable.Write();
            Console.WriteLine();
            userStatsTable.Options.EnableCount = false;
            userStatsTable.Write();
            Console.WriteLine();

            //var rows = Enumerable.Repeat(new Something(), 10);

            //ConsoleTable
            //    .From<Something>(rows)
            //    .Configure(o => o.NumberAlignment = Alignment.Right)
            //    .Write(Format.Alternative);

            //Console.ReadKey();
        }

        public async Task menu()
        {

            Console.WriteLine("\nSelecione uma opção:");
            Console.WriteLine("[1] - Roubo solo");
            Console.WriteLine("[2] - Roubo com gangue virtual");
            Console.Write("-> ");
            string opcao = Console.ReadLine();
            
            switch (opcao)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("Iniciando roubo solo...\n", Color.YellowGreen);
                    await soloRob();
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Iniciando roubo com gangue virtual...\n", Color.YellowGreen);
                    await virtualGangRob();
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        }

        public async Task soloRob()
        {
            while (true)
            {
                await this.service.Rob();
                //Thread.Sleep(5000);
            }
        }

        public async Task virtualGangRob()
        {
            await this.service.getVirtualGangs();
            await this.service.joinVirtualGang();

            while (true)
            {
                await this.service.getVirtualGangRobbery();
                showStats();
                Console.WriteLine();
                Thread.Sleep(3000);
            }
        }

        public async Task start()
        {
            showTheCrimsBot();
            Console.WriteLine("Bem vindo ao The Crims Bot!");
            Console.WriteLine("Realize o login");
            Console.Write("Usuário -> ");
            string username = Console.ReadLine();
            Console.Write("Senha -> ");
            string password = Console.ReadLine();

            await this.service.LoginAsync(username, password);

            if (this.service.logged)
            {
                Console.Clear();
                showTheCrimsBot();
                Console.WriteLine("Logado com sucesso!", Color.Green);
                await menu();
            }

        }

        public void showStats()
        {
            Console.WriteLine("Respeito: " + this.service.user.respect + " Inteligência: " + this.service.user.intelligence + " Força: " + this.service.user.strength + " Carisma: " + this.service.user.charisma + " Resistência: " + this.service.user.tolerance, Color.Green);
            Console.WriteLine("Estamina: " + this.service.user.stamina + "%" + " Vício: " + this.service.user.addiction + "%" + " Tickets: " + this.service.user.tickets, Color.Green);
            Console.WriteLine("Grana: " + Convert.ToDecimal(this.service.user.cash), Color.Green);
        }

        public void showTheCrimsBot()
        {
            Console.WriteLine("  _______ _           _____      _                 ____        _   ");
            Console.WriteLine(" |__   __| |         / ____|    (_)               |  _ \\      | |  ");
            Console.WriteLine("    | |  | |__   ___| |     _ __ _ _ __ ___  ___  | |_) | ___ | |_ ");
            Console.WriteLine("    | |  | '_ \\ / _ \\ |    | '__| | '_ \\` _\\/ __| |  _ < / _ \\| __|");
            Console.WriteLine("    | |  | | | |  __/ |____| |  | | | | | | \\__ \\ | |_) | (_) | |_ ");
            Console.WriteLine("    |_|  |_| |_|\\___|\\_____|_|  |_|_| |_| |_|___/ |____/ \\___/ \\__|");
            Console.WriteLine("                                                                   ");
            Console.WriteLine("                                                                   ");
        }

        public async Task Logout()
        {
            await this.service.Logout();
        }

    }  

}
