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
        private AutoResetEvent _executeEvent = new AutoResetEvent(false);
        private AutoResetEvent _resultAvailableEvent = new AutoResetEvent(false);
        private ManualResetEvent _runningEvent = new ManualResetEvent(false);
        private string _result;
        private string _scriptBlock;
        private volatile bool _error = false;

        public class UserData : EventArgs, IDisposable
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
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, "PowerShellRunspace.UserData: unable to parse: " + userDataHeader);
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
                IsBackground = true,
                Name = data == null ? "_default" : data.User + data.Domain
            };
            _runspaceThread.Start(data);
            if (!_runningEvent.WaitOne(3000))
            {
                throw new PowerShellRunspaceException("Thread not started on time.");
            }
        }

        private void Run(object data)
        {
            var uData = data as UserData;
            if (uData != null)
            {
                try
                {
                    Impersonate(uData);
                }
                catch (Exception err)
                {
                    _result = "Exit1:Error impersonating: " + err.Message;
                    _resultAvailableEvent.Set();
                    _error = true;
                }
            }
            OpenRunspace();
            _runningEvent.Set();
            try
            {
                while (true)
                {
                    _executeEvent.WaitOne();
                    _result = ExecuteScriptBlock();
                    _resultAvailableEvent.Set();
                }
            }
            catch (ThreadInterruptedException)
            {
                // ok, ending the loop.
            }
            _runningEvent.Reset();
        }

        private void Impersonate(UserData data)
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

        private void Stop()
        {
            if (_runspaceThread != null)
            {
                _runspaceThread.Interrupt();
            }
        }


        private void OpenRunspace()
        {
            try
            {
                _psRunspace = RunspaceFactory.CreateRunspace();
                _psRunspace.Open();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, "Error initializing runspace: " + ex.Message);
                _result = "Exit1:Error opening runspace: " + ex.Message;
                _resultAvailableEvent.Set();
                _error = true;
            }
        }

        public string ExecuteScriptBlock(string scriptBlock)
        {
            CheckError();
            this._scriptBlock = scriptBlock;
            if (!_executeEvent.Set())
            {
                return "Exit1:Unable to execute script";
            }
            _resultAvailableEvent.WaitOne();
            CheckError();
            return _result;
        }

        private void CheckError()
        {
            if (_error)
            {
                var message = "Runspace thread closed.";
                if (_resultAvailableEvent.WaitOne(500))
                {
                    message = _result + " " + message;
                }
                throw new PowerShellRunspaceException(message);
            }
        }

        private string ExecuteScriptBlock()
        {
            if (_psRunspace == null)
            {
                OpenRunspace();
            }

            if (File.Exists(_scriptBlock) && _scriptBlock.EndsWith(".ps1"))
            {
                string scriptFile = _scriptBlock;
                try
                {
                    _scriptBlock = LoadScriptFileIntoScriptBlock(scriptFile);
                }
                catch (Exception)
                {
                    return "Exit1:Cannot read: " + scriptFile;
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
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }

            try
            {
                pipeline.Commands.AddScript(_scriptBlock);
                var psObjects = pipeline.Invoke();
                if (pipeline.Error.Count > 0)
                {
                    return ProcessErrors(pipeline);
                }

                var result = ProcessResult(psObjects);
                return "Exit0:" + result;
            }
            catch (Exception ex)
            {
                var output = "Exception Invoking script block: " +
                         _scriptBlock.FormatForEventLog() +
                         "Reason:" + Environment.NewLine +
                         ex.Message;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }
        }

        private string LoadScriptFileIntoScriptBlock(string scriptFile)
        {
            // Script block is actually a ps1 file, try to read it 

            EventLogHelper.WriteToEventLog(EventLogEntryType.Information,
                "Trying to read ps1 file: " + scriptFile);
            var content = File.ReadAllText(scriptFile)
                .TrimStart(' ')
                .TrimStart(Environment.NewLine.ToCharArray())
                .TrimStart('{')
                .TrimEnd(' ')
                .TrimEnd(Environment.NewLine.ToCharArray())
                .TrimEnd('}');
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information,
                "File content is: " + content);
            return content;
        }

        private string ProcessErrors(Pipeline pipeline)
        {
            var error = pipeline.Error.Read() as Collection<ErrorRecord>;
            if (error != null)
            {
                var errors = string.Empty;
                foreach (var er in error)
                {
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Warning,
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

                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
                    return "Exit1:" + errors;
                }
            }
            return null;
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
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information,
                "Output: " + output.FormatForEventLog());
            return result.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('\r', '\n');
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

    public class PowerShellRunspaceException : Exception
    {
        public PowerShellRunspaceException(string message) : base(message) { }
        public PowerShellRunspaceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
