using System;
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
            try
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
                    pipeCommand += CreateScriptFile(opts.ScriptBlock);
                }
                else
                {
                    Console.WriteLine(
                        "Specify either --script-block or --script-file, for more info use BluShell help exec.");
                    return 1;
                }
                return SendPipeCommand(pipeCommand);
            }
            finally
            {
                if (_tempScript != null)
                {
                    File.Delete(_tempScript);
                    _tempScript = null;
                }
            }
        }

        private static string CreateScriptFile(string optsScriptBlock)
        {
            var path = Path.GetTempPath();
            var scriptFile = Guid.NewGuid().ToString("N").Substring(5, 10);
            scriptFile += "-temp-script.ps1";
            var fullName = Path.Combine(path, scriptFile);
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
                pipe = ipcClient.Connect(1);

                var bytes = Encoding.UTF8.GetBytes(pipeCommand);
                pipe.Write(bytes, 0, bytes.Length);

                // Read the result
                var data = new Byte[IpcClient.ClientInBufferSize];
                var bytesRead = pipe.Read(data, 0, data.Length);
                return ExitHandler(Encoding.UTF8.GetString(data, 0, bytesRead));
            }
            catch (Exception ex)
            {
                var message = "Error sending command to BluService: " + ex.Message + Environment.NewLine +
                              "Please verify that BluService is running with at least local administrator privilege.";
                Console.WriteLine(message);
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
                return 1;
            }
            finally
            {
                if (pipe != null)
                {
                    pipe.Close();
                }
            }
        }

        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static int ExitHandler(string serverMessage)
        {
            Console.WriteLine(serverMessage.Substring(6));
            var exitCode = serverMessage.StartsWith("Exit0:") ? 0 : 1;
            return exitCode;
        }
    }
}




