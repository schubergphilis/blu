using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BluIpc.Common;
using BluIpc.Server;

namespace BluService
{
    public class BluIpcServer : IIpcCallback, IDisposable
    {
        private IpcService _srv;
        private PowerShellRunspaceManager _runspaceManager;
        private PipeStream _pipe;

        public void Start()
        {
            _runspaceManager = new PowerShellRunspaceManager();
            _runspaceManager.ScriptOutputReceived += (sender, args) =>
            {
                try
                {
                    var dataBytes = Encoding.UTF8.GetBytes(args.Data);
                    _pipe?.Write(dataBytes, 0, dataBytes.Length);
                }
                catch (Exception)
                {
                    // Ignore errors sending intermediate output
                }
            };
            _srv = new IpcService(Config.PipeName, this, 3);
        }

        public void Stop()
        {
            _srv.IpcServerStop();
            _runspaceManager.Dispose();
        }

        public void OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            _pipe = pipe;
            string message;
            
            try
            {
                message = Encoding.UTF8.GetString(data, 0, bytes);
                if (string.IsNullOrWhiteSpace(message))
                {
                    var error = "Empty command received.";
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                    var errorBytes = Encoding.UTF8.GetBytes(error);
                    _pipe.Write(errorBytes, 0, errorBytes.Length);
                    _pipe.Close();
                    _pipe = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                var error = "BluService: There was an error in reading script block as UTF8 string: " +
                            Environment.NewLine + ex;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                var errorBytes = Encoding.UTF8.GetBytes(error);
                try
                {
                    _pipe?.Write(errorBytes, 0, errorBytes.Length);
                    _pipe?.Close();
                    _pipe = null;
                }
                catch
                {
                    // Eat broken pipe issues
                }
                return;
            }

            try
            {
                var psResult = _runspaceManager.ProcessMessage(message);
                psResult.Wait();
                var resultBytes = Encoding.UTF8.GetBytes(psResult.Result);
                _pipe.Write(resultBytes, 0, resultBytes.Length);
                _pipe.Close();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, 
                    "There is an error in executing: " + message + Environment.NewLine + ex);
                var errorBytes = Encoding.UTF8.GetBytes("Exit1:ERROR executing:" + message + Environment.NewLine + "Error: " + ex);
                try
                {
                    _pipe.Write(errorBytes, 0, errorBytes.Length);
                    _pipe.Close();
                    _pipe = null;
                }
                catch
                {
                    // Eat broken pipe issues in exception handler
                }
            }
            _pipe = null;
        }

        public void Dispose()
        {
            _runspaceManager?.Dispose();
        }
    }
}
