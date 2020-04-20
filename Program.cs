using System;
using System.Threading.Tasks;
using thecrims_bot.services;
using thecrims_bot.models;
using thecrims_bot.parser;
using thecrims_bot.console;

namespace thecrims_bot
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //TCComands command = new TCComands();
            //command.showInfo();

            //User user = new User();
            TCServices service = new TCServices();

            Console.WriteLine("  _______ _           _____      _                 ____        _   ");
            Console.WriteLine(" |__   __| |         / ____|    (_)               |  _ \\      | |  ");
            Console.WriteLine("    | |  | |__   ___| |     _ __ _ _ __ ___  ___  | |_) | ___ | |_ ");
            Console.WriteLine("    | |  | '_ \\ / _ \\ |    | '__| | '_ \\` _\\/ __| |  _ < / _ \\| __|");
            Console.WriteLine("    | |  | | | |  __/ |____| |  | | | | | | \\__ \\ | |_) | (_) | |_ ");
            Console.WriteLine("    |_|  |_| |_|\\___|\\_____|_|  |_|_| |_| |_|___/ |____/ \\___/ \\__|");
            Console.WriteLine("                                                                   ");
            Console.WriteLine("                                                                   ");

            Console.WriteLine("Bem vindo ao The Crims Bot!");
            Console.WriteLine("Realize o login");
            Console.Write("Usuário -> ");
            string username = Console.ReadLine();
            Console.Write("Senha -> ");
            string password = Console.ReadLine();

            await service.LoginAsync(username, password);

            if (service.logged)
            {

                while (true)
                {
                    //await service.getUser();
                    await service.Rob();
                }


            }

        }
    }
}
