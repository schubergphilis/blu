using System;
using System.IO;
using System.Reflection;
using BluApi.Chef.ChefAPI;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Common
{
    /// <summary>
    /// Logger class to log a line to console and write the log line in ./BluStation.log 
    /// </summary>
    public static class Logger
    {
        public static void log(string caption, string content)
        {
            if (caption == "api" && !ChefConfig.ApiLog) return;
            
            string logpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\BluStation.log";
            string log = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            switch (caption)
            {
                case "start":
                    log += " | START | ======= " + content + " =======";
                    break;
                case "ok":      // vr.Result/Exit 0
                    log += " | OK    | " + content;
                    break;
                case "info":    // vr.Result/Exit 1
                    log += " | INFO  | " + content;
                    break;
                case "data":    // vr.Result/Exit 1
                    log += " | DATA  | " + content;
                    break;
                case "warn":    // vr.Result/Exit 2
                    log += " | WARN  | " + content;
                    break;
                case "error":   // vr.Result/Exit 3
                    log += " | ERROR | " + content;
                    break;
                case "fatal":   // vr.Result/Exit 4
                    log += " | FATAL | " + content;
                    break;
                case "api":
                    log += " | API   | " + content;
                    break;
                default:
                    log += " | ???   | Log format is unknown.";
                    break;
            }

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(log);
            }
            Console.WriteLine(log);
        }

        public static void log(ReturnType rt)
        {
            string logpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\BluStation.log";
            string log = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            switch (rt.Result)
            {
                case 0:     // vr.Result/Exit 0
                    log += " | OK    | " + rt.Message;
                    break;
                case 1:     // vr.Result/Exit 1
                    log += " | INFO  | " + rt.Message;
                    break;
                case 2:     // vr.Result/Exit 2
                    log += " | WARN  | " + rt.Message;
                    break;
                case 3:     // vr.Result/Exit 3
                    log += " | ERROR | " + rt.Message;
                    break;
                case 4:     // vr.Result/Exit 4
                    log += " | FATAL | " + rt.Message;
                    break;
                default:
                    log += " | ???   | Log format is unknown.";
                    break;
            }

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(log);
            }
            Console.WriteLine(log);
        }
    }
}
