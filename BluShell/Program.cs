using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using BluIpc.Client;
using BluIpc.Common;

namespace BluShell
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!IsAdministrator())
            {
                Console.WriteLine("BluShell needs to be executed with administrator privilege.");
                Environment.Exit(1);
            }


            InputArguments inputArguments = new InputArguments(args);
            string scriptBlock = String.Empty;
            string credentials = inputArguments["-Credentials"];

            if (!string.IsNullOrEmpty(inputArguments["-Command"]))
            {
                scriptBlock = inputArguments["-Command"];
            }
            else if (!string.IsNullOrEmpty(inputArguments["-Define"]))
            {
                scriptBlock = inputArguments["-Define"].TransformForBlu();
            }
            else if (!string.IsNullOrEmpty(inputArguments["-Execute"]))
            {
                // TODO
            }
            else
            {
                Console.WriteLine("BluShell needs to be executed with -Command or -Define or -Execute parameters.");
                Environment.Exit(1);
            }

            try
            {
                IpcClient ipcClient = new IpcClient(".", Config.PipeName);
                PipeStream pipe = ipcClient.Connect(1);
                Byte[] output;
                if (!string.IsNullOrWhiteSpace(credentials))
                {
                    output = Encoding.UTF8.GetBytes(credentials + Config.MagicSplitString + scriptBlock);
                }
                else
                {
                    output = Encoding.UTF8.GetBytes(scriptBlock);
                }
                pipe.Write(output, 0, output.Length);

                // Read the result
                Byte[] data = new Byte[IpcClient.ClientInBufferSize];
                Int32 bytesRead = pipe.Read(data, 0, data.Length);
                ExitHandler(Encoding.UTF8.GetString(data, 0, bytesRead));

                // Done with pipe
                pipe.Close();
            }
            catch (Exception ex)
            {
                string message = "Error sending command to BluService: " + ex.Message +  Environment.NewLine + "Please verify that BluService is running with at least local administrator privilege.";
                Console.WriteLine(message);
                EventLogHelper.WriteToEventLog(Config.ShellName, EventLogEntryType.Error, message);
                Environment.Exit(1);
            }
        }

        private static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void ExitHandler(string serverMessage)
        {
            Console.WriteLine(serverMessage.Substring(6));
            Environment.Exit(serverMessage.StartsWith("Exit0:") ? 0 : 1);
        }
    }
}




