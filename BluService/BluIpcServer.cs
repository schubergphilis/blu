using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using BluIpc.Common;
using BluIpc.Server;

namespace BluService
{
    public class BluIpcServer : IIpcCallback
    {
        private IpcService _srv;
        private int _count;
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

        public void OnAsyncConnect(PipeStream pipe, out object state)
        {
            int count = Interlocked.Increment(ref _count);
            state = count;
        }

        public void OnAsyncDisconnect(PipeStream pipe, object state)
        {
        }

        public void OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            string scriptBlock = string.Empty;
            try
            {
                scriptBlock = Encoding.UTF8.GetString(data, 0, bytes);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, 
                    "BluService: There was an error in reading script block as UTF8 string: " + Environment.NewLine + ex.Message);
            }
            
            try
            {
                string psResult = _runspaceManager.ExecuteScriptBlock(scriptBlock);
                data = Encoding.UTF8.GetBytes(psResult);
                pipe.BeginWrite(data, 0, data.Length, OnAsyncWriteComplete, pipe);
                pipe.Close();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, 
                    "There is an error in executing script block UTF8 string: " + Environment.NewLine + ex.Message);
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
