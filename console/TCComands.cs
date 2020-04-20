using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thecrims_bot.models;
using thecrims_bot.parser;

namespace thecrims_bot.console
{
    public class TCComands
    {

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

    }  

}
