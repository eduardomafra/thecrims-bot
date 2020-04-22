using System;
using System.Threading.Tasks;
using thecrims_bot.services;
using thecrims_bot.models;
using thecrims_bot.parser;
using thecrims_bot.console;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace thecrims_bot
{
    class Program
    {       
        static async Task Main(string[] args)
        {

            TCComands commands = new TCComands();
            await commands.start();

        }

    }
}
