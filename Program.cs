using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Watchdog
{
    internal static class Program
    {
        private const string ARG_DELAY = "delay";      // Задержка/Период отправки запросов.
        private const string ARG_ATTEMPTS = "attempts";   // Кол-во попыток отправки подряд.
        private const string ARG_REJECTS = "rejects";  // Кол-во не полученных ответов для триггера.
        private const string ARG_HOST = "host";        // Хост для пинга.
        private const string ARG_TIMEOUT = "timeout";
        private const string ARG_SCRIPT = "script";    // Выполнить после тригера.

        private static Dictionary<string, string> argsDic;
        
        public static void Main(string[] args)
        {
            Logger.InitStreamFile();
            argsDic = GetArgs(args,
                new[] {ARG_DELAY, ARG_ATTEMPTS, ARG_REJECTS, ARG_HOST, ARG_TIMEOUT, ARG_SCRIPT});
            
            PingLoop();
            
            Exit();
        }

        private static void PingLoop()
        {
            if (!int.TryParse(argsDic[ARG_DELAY], out int delay))
            {
                Logger.Log(ARG_DELAY + " parse error.");
                Exit();
                return;
            }

            delay *= 60;
            string host = argsDic[ARG_HOST];
            int attempts = int.Parse(argsDic[ARG_ATTEMPTS]);
            int timeout = int.Parse(argsDic[ARG_TIMEOUT]);
            int rejects = int.Parse(argsDic[ARG_REJECTS]);
            string script = argsDic[ARG_SCRIPT];

            int reject = 0;
            do
            {
                if (!Ping(host, attempts, timeout))
                {
                    reject++;
                    Logger.Log($"Ping rejected. Total rejects {reject} of {rejects}");
                    if (reject > rejects)
                    {
                        Logger.Log($"Ping max rejects. Executing {script}...");
                        reject = 0;
                        Execute(script);
                    }
                }
                Logger.Log($"Waiting for next attempt {delay}s");
                Thread.Sleep(delay * 1000);
            } while (true);
            
        }

        private static bool Ping(string host, int attempts, int timeout)
        {
            Ping pingSender = new Ping ();
            PingOptions options = new PingOptions {DontFragment = true};

            var emptyBuffer = Encoding.ASCII.GetBytes("");
            for (int i = 0; i < attempts; i++)
            {
                Logger.Log($"Ping host {host}. Timeout: {timeout}ms. Attempt: {i}");
                PingReply reply = pingSender.Send (host, timeout, emptyBuffer, options);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    Logger.Log($"Reply from {host} recieved.");
                    return true;
                }
            }
            
            return false;
            // Console.WriteLine ("Address: {0}", reply.Address.ToString ());
            // Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
            // Console.WriteLine ("Time to live: {0}", reply.Options.Ttl);
            // Console.WriteLine ("Don't fragment: {0}", reply.Options.DontFragment);
            // Console.WriteLine ("Buffer size: {0}", reply.Buffer.Length);
        }

        private static Dictionary<string, string> GetArgs(string[] args, string[] definedArgs)
        {
            var result = new Dictionary<string, string>(definedArgs.Length);
            int missedArgs = 0;
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
                {
                    Logger.Log("Argument was not found or has wrong value: " + definedArg);
                    missedArgs++;
                }
                    
            }

            if (missedArgs > 0)
            {
                Logger.Log($"Missed {missedArgs} arguments. Exiting...");
                Exit();
            }
                

            return result;
        }

        private static void Exit()
        {
            Logger.CloseStreamFile();
            Environment.Exit(0);
        }

        private static void Execute(string path)
        {
            
        }
        
    }
}