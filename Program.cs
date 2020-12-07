using System;
using System.Threading;

namespace Watchdog
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Log("21312312321312");
            Thread.Sleep(5000);
            Log("2131231232131222222");
        }

        private static void Log(string data)
        {
            DateTime dateTime = DateTime.Now;
            Console.WriteLine(data);
            
            //print current date and time and moves cursor to next line
            // Console.WriteLine("Current Date and time is : "+dat);
            //
            // //prints text but keeps cursor in same line
            // Console.Write("Press  to exit... ");
            // while (Console.ReadKey().Key != ConsoleKey.Enter)
            // {
            //     //run loop until Enter is press
            // }
        }
    }
}