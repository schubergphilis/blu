using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using BluIpc.Common;

namespace BluService
{
    public class PowerShellRunspace : IDisposable
    {
        private Thread _runspaceThread;
        private Runspace _psRunspace;
        private Impersonator _impersonator = null;
        private volatile string _scriptBlock = null;
        private volatile string _lastResult = null;
        private readonly object _scriptBlockLock = new object();
        private readonly object _lastResultLock = new object();

        public class UserData : IDisposable
        {
            public UserData(string userDataHeader)
            {
                var parts = userDataHeader.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    User = parts[0];
                    Domain = Environment.MachineName;
                    Password = parts[1];
                }
                else if (parts.Length == 3)
                {
                    User = parts[0];
                    Domain = parts[1];
                    Password = parts[2];
                }
                else
                {
                    EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, "PowerShellRunspace.UserData: unable to parse: " + userDataHeader);
                    throw new RuntimeException("Unable to parse userdata from: " + userDataHeader);
                }
            }
            public string User { get; private set; }
            public string Domain { get; private set; }
            public string Password { get; private set; }
            public void Dispose()
            {
                User = null;
                Domain = null;
                Password = null;
                GC.Collect();
            }
        }

        public PowerShellRunspace()
        {
            Start();
        }

        public PowerShellRunspace(UserData userData)
        {
            Start(userData);
        }

        private void Start(UserData data = null)
        {
            _runspaceThread = new Thread(Run)
            {
                IsBackground = true
            };
            _runspaceThread.Start(data);
        }

        private void Run(object userData)
        {
            var data = userData as UserData;
            if (data != null)
            {
                try
                {
                    _impersonator = new Impersonator(data.User, data.Domain, data.Password);
                }
                finally
                {
                    data.Dispose();
                }
            }
            _psRunspace = RunspaceFactory.CreateRunspace();
            Open();
            try
            {
                while (true)
                {
                    ExecuteScriptBlock();
                    Thread.Sleep(50);
                }
            }
            catch (ThreadInterruptedException)
            {
                // ok, ending the loop.
            }
        }

        private void Stop()
        {
            if (_runspaceThread != null)
            {
                _runspaceThread.Interrupt();
            }
        }


        private void Open()
        {
            try
            {
                _psRunspace.Open();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, "Error initializing runspace: " + ex.Message);
            }
        }

        public string ExecuteScriptBlock(string scriptBlock)
        {
            lock (_scriptBlockLock)
            {
                _scriptBlock = scriptBlock;
            }
            while (_scriptBlock != null)
            {
                Thread.Sleep(0);
            }
            lock (_lastResultLock)
            {
                return _lastResult;
            }
        }

        private void ExecuteScriptBlock()
        {
            if (_scriptBlock == null)
            {
                return;
            }
            lock (_scriptBlockLock)
            {
                if (_scriptBlock == null)
                {
                    return;
                }
                lock (_lastResultLock)
                {
                    ExecuteScriptBlockUnsafe();
                }
            }
        }

        private void ExecuteScriptBlockUnsafe()
        {
            if (File.Exists(_scriptBlock) && _scriptBlock.EndsWith(".ps1"))
            {
                string scriptFile = _scriptBlock;
                try
                {
                    _scriptBlock = LoadScriptFileIntoScriptBlock(scriptFile);
                }
                catch (Exception)
                {
                    ScriptResultUnsafe("Exit1:Cannot read: " + scriptFile);
                    return;
                }
            }

            Pipeline pipeline;
            try
            {
                pipeline = _psRunspace.CreatePipeline();
            }
            catch (Exception ex)
            {
                var output = "Error creating pipeline: " + ex.Message;
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, output);
                ScriptResultUnsafe("Exit1:" + output);
                return;
            }

            // Dispose Runspace
            if (_scriptBlock == "DisposeRunspace")
            {
                RefreshRunspace();
                ScriptResultUnsafe("Exit0:");
                return;
            }

            try
            {
                pipeline.Commands.AddScript(_scriptBlock);
                var psObjects = pipeline.Invoke();
                if (pipeline.Error.Count > 0)
                {
                    ProcessErrors(pipeline);
                    return;
                }

                var result = ProcessResult(psObjects);
                ScriptResultUnsafe("Exit0:" + result);
            }
            catch (Exception ex)
            {
                var output = "Exception Invoking script block: " +
                         _scriptBlock.FormatForEventLog() +
                         "Reason:" + Environment.NewLine +
                         ex.Message;
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, output);
                ScriptResultUnsafe("Exit1:" + output);
            }
        }

        private string LoadScriptFileIntoScriptBlock(string scriptFile)
        {
            // Script block is actually a ps1 file, try to read it 

            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information,
                "Trying to read ps1 file: " + scriptFile);
            var content = File.ReadAllText(scriptFile)
                .TrimStart(' ')
                .TrimStart(Environment.NewLine.ToCharArray())
                .TrimStart('{')
                .TrimEnd(' ')
                .TrimEnd(Environment.NewLine.ToCharArray())
                .TrimEnd('}');
            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information,
                "File content is: " + content);
            return content;
        }

        private void ProcessErrors(Pipeline pipeline)
        {
            var error = pipeline.Error.Read() as Collection<ErrorRecord>;
            if (error != null)
            {
                var errors = string.Empty;
                foreach (var er in error)
                {
                    EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Warning,
                        "Collecting error messages...");
                    try
                    {
                        if (!string.IsNullOrEmpty(er.Exception.Message))
                            errors += "Message: " + er.Exception.Message + Environment.NewLine;
                        if (!string.IsNullOrEmpty(er.Exception.Source))
                            errors += "Source: " + er.Exception.Source + Environment.NewLine;
                        if (er.Exception.InnerException != null &&
                            !string.IsNullOrEmpty(er.Exception.InnerException.ToString()))
                            errors += "InnerException: " + er.Exception.InnerException + Environment.NewLine;
                        if (!string.IsNullOrEmpty(er.Exception.StackTrace))
                            errors += "StackTrace: " + er.Exception.StackTrace + Environment.NewLine;
                        if (!string.IsNullOrEmpty(er.Exception.HelpLink))
                            errors += "HelpLink: " + er.Exception.HelpLink + Environment.NewLine;
                        if (!string.IsNullOrEmpty(er.Exception.TargetSite.ToString()))
                            errors += "TargetSite: " + er.Exception.TargetSite + Environment.NewLine;
                        if (!string.IsNullOrEmpty(er.Exception.Data.ToString()))
                            errors += "Exception Data: " + er.Exception.Data + Environment.NewLine;
                        errors += "--------------";
                    }
                    catch (Exception ex)
                    {
                        errors += "Error on collecting PowerShell exception messages: " + ex.Message;
                    }

                    var message = "Script Block: " + _scriptBlock.FormatForEventLog() +
                                  "Executed and returned the following errors:" +
                                  Config.SeparatorLine + errors;

                    EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, message);
                    ScriptResultUnsafe("Exit1:" + errors);
                }
            }
        }

        private string ProcessResult(Collection<PSObject> psObjects)
        {
            var output = "Execution of Script Block: " + _scriptBlock.FormatForEventLog();
            var result = string.Empty;
            switch (psObjects.Count)
            {
                case 0:
                    output += "Is completed successfully and returned null." + Environment.NewLine;
                    break;

                case 1:
                    // Accept null as a valid osObject and BaseObject, so return null
                    if (psObjects[0] == null || psObjects[0].BaseObject == null)
                    {
                        output += "Is completed successfully and returned null." + Environment.NewLine;
                    }
                    else
                    {
                        output += "Is completed successfully and returned:" + Environment.NewLine;
                        output += psObjects[0].BaseObject.ToString();
                        result += psObjects[0].BaseObject;
                    }
                    break;

                default:
                    output += "Is completed successfully and returned:" + Environment.NewLine +
                              Environment.NewLine;
                    foreach (var pso in psObjects)
                    {
                        output += pso.BaseObject + Environment.NewLine;
                        result += pso.BaseObject + Environment.NewLine;
                    }
                    break;
            }
            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information,
                "Output: " + output.FormatForEventLog());
            return result.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('\r', '\n');
        }
        
        private void ScriptResultUnsafe(string message)
        {
            _lastResult = message;
            _scriptBlock = null;
        }

        private void RefreshRunspace()
        {
            _psRunspace.Dispose();
            _psRunspace = null;
            _psRunspace = RunspaceFactory.CreateRunspace();
            Open();
            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Warning, "PowerShell Runspace is disposed." + Environment.NewLine + "All previously definied PS objects are garbage collected.");
        }

        public void Dispose()
        {
            Stop();
            if (_psRunspace != null)
            {
                _psRunspace.Dispose();
            }
            if (_impersonator != null)
            {
                _impersonator.Dispose();
            }
        }
    }
}
