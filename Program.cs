using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Watchdog
{
    internal static class Program
    {
        private const string ARG_PERIOD = "period";    // Период отправки запросов.
        private const string ARG_REJECTS = "rejects";  // Кол-во не полученных ответов для триггера.
        private const string ARG_HOST = "host";        // Хост для пинга.
        private const string ARG_SCRIPT = "script";    // Выполнить после тригера.

        private static StreamWriter streamWriter;
        public static void Main(string[] args)
        {
             InitStreamFile("log.txt");
            
            var argsDic = GetArgs(args, new[] {ARG_PERIOD, ARG_REJECTS, ARG_HOST, ARG_SCRIPT});
            Thread.Sleep(5000);
            Exit();
        }

        private static Dictionary<string, string> GetArgs(string[] args, string[] definedArgs)
        {
            var result = new Dictionary<string, string>(definedArgs.Length);
            for (int i = 0; i < definedArgs.Length; i++)
            {
                string definedArg = definedArgs[i];
                bool hasArg = false;
                for (int j = 0; j < args.Length; j++)
                {
                    var arg = args[j].Split('=');
                    if (arg.Length != 2)    // Если это не пара аргумент - значение.
                        break;
                    if (definedArg == arg[0].Replace("-",""))
                    {
                        result.Add(definedArg,arg[1]);
                        hasArg = true;
                        break;
                    }
                }
                if (!hasArg)
                    Log("Argument was not found or has wrong value: " + definedArg);
            }

            return result;
        }

        private static void InitStreamFile(string path)
        {
            path = $"{AppDomain.CurrentDomain.BaseDirectory}/{path}";
            streamWriter = new StreamWriter(path, true) {AutoFlush = true};
        }

        private static void Log(string data)
        {
            DateTime now = DateTime.Now;
            var logLine =
                $"[{now.Day:00}.{now.Month:00}.{now.Year}-{now.Hour:00}:{now.Minute:00}:{now.Second:00}]: {data}";
            Console.WriteLine(logLine);
            
            streamWriter.WriteLine(logLine);

            // Console.Write("Press  to exit... ");
            // while (Console.ReadKey().Key != ConsoleKey.Enter)
            // {
            //     //run loop until Enter is press
            // }
        }

        private static void Exit()
        {
            streamWriter.Close();
        }
        
    }
}