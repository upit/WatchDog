using System;
using System.IO;

namespace Watchdog
{
    internal static class Logger
    {
        private static StreamWriter streamWriter;
        
        public static void InitStreamFile()
        {
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}/log{GetStringFromDate(DateTime.Now, true)}.txt";
            streamWriter = new StreamWriter(path, true) {AutoFlush = true};
        }

        public static void CloseStreamFile()
        {
            streamWriter.Close();
        }
        
        public static void Log(string data)
        {
            string logLine = $"[{GetStringFromDate(DateTime.Now)}]: {data}";
            Console.WriteLine(logLine);
            streamWriter.WriteLine(logLine);

            // Console.Write("Press  to exit... ");
            // while (Console.ReadKey().Key != ConsoleKey.Enter)
            // {
            //     //run loop until Enter is press
            // }
        }

        private static string GetStringFromDate(DateTime time, bool simple = false)
        {
            return simple
                ? $"{time.Day:00}{time.Month:00}{time.Year}_{time.Hour:00}{time.Minute:00}{time.Second:00}"
                : $"{time.Day:00}.{time.Month:00}.{time.Year}-{time.Hour:00}:{time.Minute:00}:{time.Second:00}";
        }

    }
}