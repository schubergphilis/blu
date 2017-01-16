using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using BluIpc.Common;

namespace BluService
{
    public class PowerShellRunspace : IDisposable
    {
        readonly RunspaceConfiguration _runspaceConfiguration;
        Runspace _psRunspace;

        public PowerShellRunspace()
        {
            _runspaceConfiguration = RunspaceConfiguration.Create();
            _psRunspace = RunspaceFactory.CreateRunspace(_runspaceConfiguration);
            Open();
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
            Pipeline pipeline;
            string result = string.Empty;
            string output;
            string errors = string.Empty;

            if (File.Exists(scriptBlock) && scriptBlock.EndsWith(".ps1"))
            {
                // Script block is actually a ps1 file, try to read it 
                string scriptFile = scriptBlock;
                try
                {
                    EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information, "Trying to read ps1 file: " + scriptFile);
                    scriptBlock = File.ReadAllText(scriptFile)
                        .TrimStart(' ')
                        .TrimStart(Environment.NewLine.ToCharArray())
                        .TrimStart('{')
                        .TrimEnd(' ')
                        .TrimEnd(Environment.NewLine.ToCharArray())
                        .TrimEnd('}');
                    EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information, "File content is: " + scriptBlock);
                }
                catch (Exception)
                {
                    return "Exit1:Cannot read: " + scriptFile;
                }
            }
            
            try
            {
                pipeline = _psRunspace.CreatePipeline();
            }
            catch (Exception ex)
            {
                output = "Error creating pipeline: " + ex.Message;
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }

            // Dispose Runspace
            if (scriptBlock == "DisposeRunspace")
            {
                RefreshRunspace();
                return "Exit0:";
            }

            try
            {
                pipeline.Commands.AddScript(scriptBlock);
                Collection<PSObject> psObjects = pipeline.Invoke();
                if (pipeline.Error.Count > 0)
                {
                    var error = pipeline.Error.Read() as Collection<ErrorRecord>;
                    if (error != null)
                    {
                        foreach (ErrorRecord er in error)
                        {
                            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Warning, "Collecting error messages...");
                            try
                            {
                                if (!string.IsNullOrEmpty(er.Exception.Message))
                                    errors += "Message: " + er.Exception.Message + Environment.NewLine;
                                if (!string.IsNullOrEmpty(er.Exception.Source))
                                    errors += "Source: " + er.Exception.Source + Environment.NewLine;
                                if (er.Exception.InnerException != null && !string.IsNullOrEmpty(er.Exception.InnerException.ToString()))
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

                            output = "Script Block: " + scriptBlock.FormatForEventLog() + "Executed and returned the following errors:" + 
                                Config.SeparatorLine + errors;

                            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, output);
                            return "Exit1:" + errors;
                        }
                    }
                }

                output = "Execution of Script Block: " + scriptBlock.FormatForEventLog();

                switch (psObjects.Count)
                {
                    case 0:
                        // When there is no error, accept null output as a valid result, so return null
                        output += "Is completed successfully and returned null." + Environment.NewLine;
                        result = string.Empty;
                        break;

                    case 1:
                        // Accept null as a valid osObject and BaseObject, so return null
                        if (psObjects[0] == null || psObjects[0].BaseObject == null)
                        {
                            output += "Is completed successfully and returned null." + Environment.NewLine;
                            result = string.Empty;
                        }
                        else
                        {
                            // Else exit 0 with result
                            output += "Is completed successfully and returned:" + Environment.NewLine;
                            output += psObjects[0].BaseObject.ToString();
                            result += psObjects[0].BaseObject;
                        }
                        break;

                    default:
                        output += "Is completed successfully and returned:" + Environment.NewLine + Environment.NewLine;
                        foreach (var pso in psObjects)
                        {
                            output += pso.BaseObject + Environment.NewLine;
                            result += pso.BaseObject + Environment.NewLine;
                        }
                        break;
                }
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Information, "Output: " + output.FormatForEventLog());
                return "Exit0:" + result.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('\r', '\n');
            }
            catch (Exception ex)
            {
                output = "Exception Invoking script block: " +
                         scriptBlock.FormatForEventLog() +
                         "Reason:" + Environment.NewLine +
                         ex.Message;
                EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }
        }

        public void RefreshRunspace()
        {
            _psRunspace.Dispose();
            _psRunspace = null;
            _psRunspace = RunspaceFactory.CreateRunspace(_runspaceConfiguration);
            Open();
            EventLogHelper.WriteToEventLog(Config.ServiceName, EventLogEntryType.Warning, "PowerShell Runspace is disposed." + Environment.NewLine + "All previously definied PS objects are garbage collected.");
        }

        public void Dispose()
        {
            if (_psRunspace != null)
            {
                _psRunspace.Dispose();
            }
        }
    }
}
