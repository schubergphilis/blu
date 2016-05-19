using System;
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
        private Int32 _count;

        public void Start()
        {
            _srv = new IpcService(Config.PipeName, this, 3);
        }

        public void Stop()
        {
            _srv.IpcServerStop();
        }

        public void OnAsyncConnect(PipeStream pipe, out Object state)
        {
            Int32 count = Interlocked.Increment(ref _count);
            // Utils.EventLogUtil.WriteToEventLog(Utils.Config.ServiceName, 0, "Connected session number: " + count);
            state = count;
        }

        public void OnAsyncDisconnect(PipeStream pipe, Object state)
        {
            // Utils.EventLogUtil.WriteToEventLog(Utils.Config.ServiceName, 0, "Disconnected session number: " + (Int32)state);
        }

        public void OnAsyncMessage(PipeStream pipe, Byte[] data, Int32 bytes, Object state)
        {
            string scriptBlock = String.Empty;
            try
            {
                scriptBlock = Encoding.UTF8.GetString(data, 0, bytes);
                // EventLogHelper.WriteToEventLog(Config.ServiceName, 0, "IPC Script Block received: " + Environment.NewLine +
                // "------------------------------------------" + Environment.NewLine +
                // scriptBlock + Environment.NewLine +
                // "------------------------------------------" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, 2,
                    "BluService: There was an error in reading script block as UTF8 string: " + Environment.NewLine + ex.Message);
            }
            
            // Execute and Write back results
            try
            {
                string psResult = PowerShellRunspace.ExecuteScriptBlock(scriptBlock);
                byte[] psResultBytes = Encoding.Default.GetBytes(psResult);
                string result = Encoding.UTF8.GetString(psResultBytes, 0, Encoding.UTF8.GetByteCount(psResult));
                data = Encoding.UTF8.GetBytes(result.ToCharArray());
                pipe.BeginWrite(data, 0, Encoding.UTF8.GetByteCount(result), OnAsyncWriteComplete, pipe);
                pipe.Close();

            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, 2,
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
