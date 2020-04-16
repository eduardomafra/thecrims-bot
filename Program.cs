using System;
using System.Threading.Tasks;
using thecrims_bot.services;
using thecrims_bot.models;
using thecrims_bot.parser;

namespace thecrims_bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //TCParser p = new TCParser();
            //p.getChannel("a");
            User user = new User();
            TCServices service = new TCServices();

            Console.WriteLine("Bem vindo ao The Crims Bot!");
            Console.WriteLine("Primeiro nós precisamos efetuar o login à sua conta");
            Console.Write("Usuário -> ");
            string username = Console.ReadLine();
            Console.Write("Senha -> ");
            string password = Console.ReadLine();

            await service.LoginAsync(username, password);

            if (service.logged)
            {

                while (true)
                {
                     await service.getUser();
                     await service.Rob();                   
                }


            }

        }
    }
}
