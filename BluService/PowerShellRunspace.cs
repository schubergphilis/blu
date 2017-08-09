﻿using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BluIpc.Common;

namespace BluService
{
    public class PowerShellRunspace : IDisposable
    {
        private bool _runningScript;
        private Process _runspaceConsole;
        private readonly EventWaitHandle _eventWaitHandle;
        private readonly string _waitHandleName;

        public PowerShellRunspace(string runspaceName) : this(runspaceName, null)
        {
        }

        public event EventHandler<DataReceivedEventArgs> ScriptOutputReceived;


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
            _runspaceConsole.OutputDataReceived += (sender, args) =>
            {
                if (args.Data?.EndsWith(Config.RunspaceExecutionDone) ?? false)
                {
                    _runningScript = false;
                }
                ScriptOutputReceived?.Invoke(sender, args);
            };
            _runspaceConsole.ErrorDataReceived += (sender, args) =>
            {
                ScriptOutputReceived?.Invoke(sender, args);
            };
            _runspaceConsole.Start();
            _runspaceConsole.BeginErrorReadLine();
            _runspaceConsole.BeginOutputReadLine();
            Thread.Sleep(500); // Making sure that data reading events are running
            if (!_eventWaitHandle.WaitOne(60000))
            {
                throw new Exception("Error starting runspace - timeout");
            }
        }

        public async Task<string> ExecuteScript(string scriptFile)
        {
            _runningScript = true;
            if (_runspaceConsole.HasExited)
            {
                return "Exit1:runspace has exited, dispose and recreate";
            }
            _runspaceConsole.StandardInput.WriteLine(scriptFile);
            if (await Task.Run(() => !_eventWaitHandle.WaitOne(TimeSpan.FromHours(4))))
            {
                return "Exit1:runspace timeout";
            }

            if (_runspaceConsole.HasExited && _runspaceConsole.ExitCode != 0)
            {
                return "Exit1:Error during script execution";
            }
            while (_runningScript)
            {
                Thread.Sleep(50);
            }

            return "Exit0:";
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
