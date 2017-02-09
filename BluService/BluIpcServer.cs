using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using BluIpc.Common;
using BluIpc.Server;

namespace BluService
{
    public class BluIpcServer : IIpcCallback
    {
        private IpcService _srv;
        private PowerShellRunspaceManager _runspaceManager;

        public void Start()
        {
            _runspaceManager = new PowerShellRunspaceManager();
            _srv = new IpcService(Config.PipeName, this, 3);
        }

        public void Stop()
        {
            _srv.IpcServerStop();
            _runspaceManager.Dispose();
        }

        public void OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            var scriptBlock = string.Empty;
            PowerShellRunspace.UserData userData = null;
            try
            {
                var message = Encoding.UTF8.GetString(data, 0, bytes);
                if (string.IsNullOrWhiteSpace(message))
                {
                    var error = "Empty command received.";
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                    var errorBytes = Encoding.UTF8.GetBytes(error);
                    pipe.Write(errorBytes, 0, errorBytes.Length);
                    pipe.Close();
                    return;
                }
                var messageParts = message.Split(new[] {Config.MagicSplitString}, StringSplitOptions.RemoveEmptyEntries);
                switch (messageParts.Length)
                {
                    case 1:
                        scriptBlock = messageParts[0];
                        break;
                    case 2:
                        userData = new PowerShellRunspace.UserData(messageParts[0]);
                        scriptBlock = messageParts[1];
                        break;
                    default:
                        var error = "Unexpected number of message parts. Received " + messageParts.Length + " parts, expecting 1 or 2.";
                        EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                        var errorBytes = Encoding.UTF8.GetBytes(error);
                        pipe.Write(errorBytes, 0, errorBytes.Length);
                        pipe.Close();
                        return;
                }
                
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, 
                    "BluService: There was an error in reading script block as UTF8 string: " + Environment.NewLine + ex);
            }
            
            try
            {
                string psResult;
                if (userData != null)
                {
                    psResult = _runspaceManager.ExecuteScriptBlock(scriptBlock, userData);
                }
                else
                {
                    psResult = _runspaceManager.ExecuteScriptBlock(scriptBlock);
                }
                var resultBytes = Encoding.UTF8.GetBytes(psResult);
                pipe.BeginWrite(resultBytes, 0, resultBytes.Length, OnAsyncWriteComplete, pipe);
                pipe.Close();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, 
                    "There is an error in executing script block: " + scriptBlock + Environment.NewLine + ex);
                var errorBytes = Encoding.UTF8.GetBytes("Exit1:ERROR executing Script:" + scriptBlock + Environment.NewLine + "Error: " + ex);
                pipe.BeginWrite(errorBytes, 0, errorBytes.Length, OnAsyncWriteComplete, pipe);
                pipe.Close();
            }
        }

        private void OnAsyncWriteComplete(IAsyncResult result)
        {
            PipeStream pipe = (PipeStream)result.AsyncState;
            pipe.EndWrite(result);
        }
    }
}
