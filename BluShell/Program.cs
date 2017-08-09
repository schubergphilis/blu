using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using BluIpc.Client;
using BluIpc.Common;
using CommandLine;
using CommandLine.Text;

namespace BluShell
{
    class Program
    {
        private static string _tempScript = null;
        private static void Main(string[] args)
        {
            if (!IsAdministrator())
            {
                Console.WriteLine("BluShell needs to be executed with administrator privilege.");
                Environment.Exit(1);
            }

            var optionsResult = Parser.Default.ParseArguments<ExecuteOptions, RunspaceOptions>(args);

            Environment.Exit(
                optionsResult.MapResult(
                    (ExecuteOptions opts) => RunExecute(opts),
                    (RunspaceOptions opts) => RunRunspace(opts),
                    errors => 1));
        }

        private static int RunExecute(ExecuteOptions opts)
        {
            var pipeCommand = Config.ExecuteCommand + Config.MagicSplitString;
            if (!string.IsNullOrWhiteSpace(opts.Runspace))
            {
                pipeCommand += opts.Runspace + Config.MagicSplitString;
            }
            if (!string.IsNullOrWhiteSpace(opts.ScriptFile))
            {
                if (opts.ScriptFile.EndsWith(".ps1") && File.Exists(opts.ScriptFile))
                {
                    pipeCommand += opts.ScriptFile;
                }
                else
                {
                    Console.WriteLine("Script file specified not found or not .ps1 file.");
                    return 1;
                }
            }
            else if (!string.IsNullOrWhiteSpace(opts.ScriptBlock))
            {
                pipeCommand += CreateScriptFile(opts.ScriptBlock, opts.Runspace);
            }
            else
            {
                Console.WriteLine(
                    "Specify either --script-block or --script-file, for more info use BluShell help exec.");
                return 1;
            }
            return SendPipeCommand(pipeCommand);
        }

        private static string CreateScriptFile(string optsScriptBlock, string runspace)
        {
            var scriptFile = string.Empty;
            if (!string.IsNullOrWhiteSpace(runspace))
            {
                scriptFile += "-" + runspace;
            }
            scriptFile += DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            scriptFile += "-" + Guid.NewGuid().ToString("N").Substring(5, 4);
            scriptFile += ".ps1";
            var fullName = Path.Combine(Config.TempPath, scriptFile);
            File.WriteAllText(fullName, optsScriptBlock);
            _tempScript = fullName;
            return fullName;
        }

        private static int RunRunspace(RunspaceOptions opts)
        {
            string pipeCommand;
            if (opts.Delete)
            {
                pipeCommand = Config.DisposeRunspaceCommand + Config.MagicSplitString;
            }
            else
            {
                pipeCommand = Config.CreateRunspaceCommand + Config.MagicSplitString;
            }
            if (!string.IsNullOrWhiteSpace(opts.Credentials) && !opts.Delete)
            {
                pipeCommand += opts.Credentials + Config.MagicSplitString;
            }
            pipeCommand += opts.Runspace;
            return SendPipeCommand(pipeCommand);
        }

        private static int SendPipeCommand(string pipeCommand)
        {
            PipeStream pipe = null;
            try
            {
                IpcClient ipcClient = new IpcClient(".", Config.PipeName);
                pipe = ipcClient.Connect(100);

                var bytes = Encoding.UTF8.GetBytes(pipeCommand);
                pipe.Write(bytes, 0, bytes.Length);

                //TODO: If Exit0: or Exit1: never comes this spins out of controlllllllllllllllll..........
                while (true)
                {
                    // Read the result
                    var data = new Byte[IpcClient.ClientInBufferSize];
                    var bytesRead = pipe.Read(data, 0, data.Length);
                    var message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    if (message.StartsWith("Exit0:") || message.StartsWith("Exit1:"))
                    {
                        return ExitHandler(message);
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = "Error sending command to BluService: " + ex + Environment.NewLine +
                              "Please verify that BluService is running with at least local administrator privilege.";
                Console.WriteLine(message);
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
                return 1;
            }
            finally
            {
                pipe?.Close();
            }
        }

        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static int ExitHandler(string serverMessage)
        {
            if (serverMessage?.Length > 6)
            {
                Console.WriteLine(serverMessage.Substring(6));
                var exitCode = serverMessage.StartsWith("Exit0:") ? 0 : 1;
                return exitCode;
            }
            return 1;
        }
    }
}




