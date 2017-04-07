using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BluIpc.Common;

namespace BluRunspace
{
    class Program
    {
        private static BluRunspace _runspace;
        static void Main(string[] args)
        {
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Starting up blurunspace. Args: " + string.Join(", ", args));
            if (args.Length != 1)
            {
                Console.WriteLine("Please specify a waitHandle name as argument.");
                Environment.Exit(0);
            }
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Registering wait handle: " + args[0]);
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, args[0]);

            EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Starting runspace.");
            _runspace = new BluRunspace();
            try
            {
                string input;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Started runspace.");
                waitHandle.Set();
                EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Waithandle set.");
                while ((input = Console.ReadLine()) != null)
                {
                    if (input.ToLower() == "quit" || input.ToLower() == "exit")
                    {
                        waitHandle.Set();
                        break;
                    }
                    try
                    {
                        Console.WriteLine(_runspace.RunScript(input));
                    }
                    catch (Exception err)
                    {
                        var error = "ERROR during script execution, exception: " + err;
                        EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                        Console.WriteLine("Exit1:" + error);
                    }
                    Console.WriteLine(Config.RunspaceExecutionDone);
                    Console.Out.Flush();
                    waitHandle.Set();
                }
            }
            finally
            {
                _runspace.Dispose();
            }
            Console.WriteLine("Exiting runspace.");
            Environment.ExitCode = 0;
        }
    }
}
