using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using BluIpc.Common;

namespace BluService
{
    public class PowerShellRunspace : IDisposable
    {
        private Process _runspaceConsole;
        private EventWaitHandle _eventWaitHandle;
        private string _waitHandleName;
        private string _dataReceived = string.Empty;

        public PowerShellRunspace(string runspaceName) : this(runspaceName, null)
        {
        }

        public PowerShellRunspace(string runspaceName, UserData userData)
        {
            _waitHandleName = "Global\\" + runspaceName + "-wait-handle";
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, _waitHandleName);
            Start(userData);
        }

        private void Start(UserData data = null)
        {

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                FileName = "BluRunspace.exe",
                UseShellExecute = false,
                Arguments = _waitHandleName
            };
            if (data != null)
            {
                WindowStationAccess.GrantAccessToWindowStationAndDesktop(data.Domain + "\\" + data.User);
                startInfo.UserName = data.User;
                startInfo.Password = data.Password;
                startInfo.Domain = data.Domain;
                startInfo.LoadUserProfile = true;
            }
            _runspaceConsole = new Process { StartInfo = startInfo };
            _runspaceConsole.OutputDataReceived += (sender, args) => { _dataReceived += args.Data + Environment.NewLine; };
            _runspaceConsole.ErrorDataReceived += (sender, args) => { _dataReceived += args.Data + Environment.NewLine; };
            _runspaceConsole.Start();
            _runspaceConsole.BeginErrorReadLine();
            _runspaceConsole.BeginOutputReadLine();
            Thread.Sleep(500); // Making sure that data reading events are running
            _dataReceived = string.Empty;
            if (!_eventWaitHandle.WaitOne(60000))
            {
                throw new Exception("Error starting runspace - timeout");
            }
        }

        public string ExecuteScript(string scriptFile)
        {
            if (_runspaceConsole.HasExited)
            {
                return "Exit1:runspace has exited, dispose and recreate";
            }
            _runspaceConsole.StandardInput.WriteLine(scriptFile);
            if (!_eventWaitHandle.WaitOne(TimeSpan.FromHours(4)))
            {
                return "Exit1:runspace timeout";
            }

            var output = new StringBuilder();
            if (_runspaceConsole.HasExited && _runspaceConsole.ExitCode != 0)
            {
                output.Append("Exit1:");
            }
            while (!_dataReceived.Trim().EndsWith(Config.RunspaceExecutionDone))
            {
                Thread.Sleep(10);
            }
            _dataReceived = _dataReceived.Replace(Config.RunspaceExecutionDone, "").Trim();
            output.Append(_dataReceived);
            _dataReceived = string.Empty;
            return output.ToString();
        }

        public void Dispose()
        {
            if (_runspaceConsole != null)
            {
                try
                {
                    _runspaceConsole.StandardInput.WriteLine("quit");
                    _eventWaitHandle.WaitOne(1000);
                }
                catch
                {
                    // ignore errors
                }
                _runspaceConsole.Dispose();
            }
        }
    }

    public class PowerShellRunspaceException : Exception
    {
        public PowerShellRunspaceException(string message) : base(message) { }
        public PowerShellRunspaceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
