using System;

namespace consoleWather
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Погода в Сыктывкаре.";
            ConsoleWeb consoleWeb = new ConsoleWeb();
            consoleWeb.TimerWeb();
        }
    }
}
