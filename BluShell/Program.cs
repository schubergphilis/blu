using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using BluIpc.Client;
using BluIpc.Common;

namespace BluShell
{
    class Program
    {
        static void Main(string[] args)
        {
            InputArguments inputArguments = new InputArguments(args);
            string scriptBlock = String.Empty;

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
                Console.WriteLine("BluSheel needs to be executed with -Command or -Define or -Execute parameters.");
                Environment.Exit(1);
            }
            
            try
            {
                IpcClient ipcClient = new IpcClient(".", "BluPowerShell");
                PipeStream pipe = ipcClient.Connect(1);
                Byte[] output = Encoding.UTF8.GetBytes(scriptBlock);
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
                EventLogHelper.WriteToEventLog(Config.ShellName, EventLogEntryType.Error, "Exception in send/recieve script block: " + Environment.NewLine + ex.Message);
                Environment.Exit(1);
            }
        }

        private static void ExitHandler(string serverMessage)
        {
            Console.WriteLine(serverMessage.Substring(6));
            Environment.Exit(serverMessage.StartsWith("Exit0:") ? 0 : 1);
        }
    }
}
        
        
        

