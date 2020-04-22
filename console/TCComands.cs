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

            var userTable = new ConsoleTable("Username", "Spirit Name", "Respect", "Tickets", "Stamina", "Addiction", "Cash");
            userTable.AddRow(this.service.user.username, this.service.user.spirit_name, this.service.user.respect, this.service.user.tickets, this.service.user.stamina, this.service.user.addiction, this.service.user.cash.ToString("$ #,###"));

            var userStatsTable = new ConsoleTable("Intelligence", "Strength", "Charisma", "Tolerance");
            userStatsTable.AddRow(this.service.user.intelligence, this.service.user.strength, this.service.user.charisma, this.service.user.tolerance);

            var userPowerTable = new ConsoleTable("Single Robbery Power", "Gang Robbery Power", "Assault Power");
            userPowerTable.AddRow(this.service.user.single_robbery_power, this.service.user.gang_robbery_power, this.service.user.assault_power);

            userTable.Options.EnableCount = false;
            userStatsTable.Options.EnableCount = false;
            userPowerTable.Options.EnableCount = false;
            userTable.Write();            
            userStatsTable.Write();
            userPowerTable.Write();
        }

        public async Task menu()
        {
            showInfo();
            Console.WriteLine("Selecione uma opção:");
            Console.WriteLine("[1] - Roubo solo");
            Console.WriteLine("[2] - Roubo com gangue virtual");
            Console.WriteLine("[3] - Sair");
            Console.Write("-> ");
            string opcao = Console.ReadLine();
            
            switch (opcao)
            {
                case "1":
                    Console.Clear();
                    showTheCrimsBot();
                    Console.WriteLine("Iniciando roubo solo...\n", Color.YellowGreen);
                    await soloRob();
                    break;
                case "2":
                    Console.Clear();
                    showTheCrimsBot();
                    Console.WriteLine("Iniciando roubo com gangue virtual...\n", Color.YellowGreen);
                    await virtualGangRob();
                    break;
                case "3":
                    Console.Clear();
                    showTheCrimsBot();                   
                    await Logout();                    
                    break;
                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }

        }

        public async Task soloRob()
        {

            while (!Console.KeyAvailable)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break;
                await this.service.Rob();
            }

            Console.Clear();
            showTheCrimsBot();
            Console.WriteLine("Operação de roubos cancelada", Color.GreenYellow);
            await menu();
        }

        public async Task virtualGangRob()
        {
            await this.service.getVirtualGangs();
            await this.service.joinVirtualGang();

            while (true)
            {

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break;
                try
                {
                    await this.service.getVirtualGangRobbery();
                    Console.WriteLine(this.service.user.ToString(), Color.Green);
                    Console.WriteLine();
                    Thread.Sleep(1000);
                }
                catch
                {
                    return;
                }
            }

            Console.Clear();
            showTheCrimsBot();
            Console.WriteLine("Operação de roubo de gangue cancelada", Color.GreenYellow);
            await menu();
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
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

    }  

}
