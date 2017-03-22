using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using BluIpc.Common;

namespace BluRunspace
{
    class BluRunspace: IDisposable
    {
        private Runspace _psRunspace;

        private void OpenRunspace()
        {
            try
            {
                _psRunspace = RunspaceFactory.CreateRunspace();
                _psRunspace.Open();
            }
            catch (Exception ex)
            {
                var error = "Error opening runspace: " + ex.Message;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                throw new Exception(error);
            }
        }

        public string RunScript(string file)
        {
            var scriptBlock = LoadScriptFileIntoScriptBlock(file);
            return ExecuteScriptBlock(scriptBlock);
        }

        private static string LoadScriptFileIntoScriptBlock(string scriptFile)
        {
            try
            {
                var content = File.ReadAllText(scriptFile)
                    .TrimStart(' ')
                    .TrimStart(Environment.NewLine.ToCharArray())
                    .TrimEnd(' ')
                    .TrimEnd(Environment.NewLine.ToCharArray());
                EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "File content is: " + content);
                return content;
            }
            catch (Exception err)
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, "Error loading file " + scriptFile + " exception: " + err);
                throw;
            }
        }

        private string ExecuteScriptBlock(string scriptBlock)
        {
            if (_psRunspace == null)
            {
                OpenRunspace();
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
                pipeline.Commands.AddScript(scriptBlock);
                var psObjects = pipeline.Invoke();
                var exit = 0;
                if (PsResultIsFalse(psObjects))
                {
                    exit = 1;
                }
                if (pipeline.Error.Count > 0)
                {
                    return ProcessErrors(pipeline, scriptBlock);
                }

                var result = ProcessResult(psObjects, scriptBlock);
                return "Exit" + exit + ":" + result;
            }
            catch (Exception ex)
            {
                var output = "Exception Invoking script block: " +
                         scriptBlock.FormatForEventLog() +
                         "Reason:" + Environment.NewLine +
                         ex;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }
        }

        private static bool PsResultIsFalse(Collection<PSObject> psObjects)
        {
            return psObjects.Count == 1 && psObjects[0].BaseObject is bool && (bool)psObjects[0].BaseObject == false;
        }

        private string ProcessErrors(Pipeline pipeline, string scriptBlock)
        {
            var errorObject = pipeline.Error.Read();
            var errorRecords = errorObject as Collection<ErrorRecord>;
            if (errorRecords != null)
            {
                var errors = string.Empty;
                foreach (var er in errorRecords)
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

                    var message = "Script Block: " + scriptBlock.FormatForEventLog() +
                                  "Executed and returned the following errors:" +
                                  Config.SeparatorLine + errors;

                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
                    return "Exit1:" + message;
                }
            }
            return "Exit1:Errors executing, errorObject: " + errorObject;
        }

        private string ProcessResult(Collection<PSObject> psObjects, string scriptBlock)
        {
            var output = "Execution of Script Block: " + scriptBlock.FormatForEventLog();
            var result = string.Empty;
            try
            {
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
                            if (pso == null || pso.BaseObject == null)
                            {
                                continue;
                            }
                            output += pso.BaseObject + Environment.NewLine;
                            result += pso.BaseObject + Environment.NewLine;
                        }
                        break;
                }
                EventLogHelper.WriteToEventLog(EventLogEntryType.Information,
                    "Output: " + output.FormatForEventLog());
                return result.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd('\r', '\n');
            }
            catch (Exception err)
            {
                return "Command successful, but error while processing result, please report to blu guru: " + err;
            }
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
