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
    public class BluIpcServer : IIpcCallback, IDisposable
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
            string message = null;
            try
            {
                message = Encoding.UTF8.GetString(data, 0, bytes);
                if (string.IsNullOrWhiteSpace(message))
                {
                    var error = "Empty command received.";
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                    var errorBytes = Encoding.UTF8.GetBytes(error);
                    pipe.Write(errorBytes, 0, errorBytes.Length);
                    pipe.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                var error = "BluService: There was an error in reading script block as UTF8 string: " +
                            Environment.NewLine + ex;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                var errorBytes = Encoding.UTF8.GetBytes(error);
                pipe.Write(errorBytes, 0, errorBytes.Length);
                pipe.Close();
                return;
            }

            try
            {
                var psResult = _runspaceManager.ProcessMessage(message); ;
                var resultBytes = Encoding.UTF8.GetBytes(psResult);
                pipe.BeginWrite(resultBytes, 0, resultBytes.Length, OnAsyncWriteComplete, pipe);
                pipe.Close();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, 
                    "There is an error in executing: " + message + Environment.NewLine + ex);
                var errorBytes = Encoding.UTF8.GetBytes("Exit1:ERROR executing:" + message + Environment.NewLine + "Error: " + ex);
                pipe.BeginWrite(errorBytes, 0, errorBytes.Length, OnAsyncWriteComplete, pipe);
                pipe.Close();
            }
        }

        private void OnAsyncWriteComplete(IAsyncResult result)
        {
            PipeStream pipe = (PipeStream)result.AsyncState;
            pipe.EndWrite(result);
        }

        public void Dispose()
        {
            if (_runspaceManager != null)
            {
                _runspaceManager.Dispose();
            }
        }
    }
}
