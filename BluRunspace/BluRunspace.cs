using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
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
                var host = new BluPsHost();
                host.DataReady += HostOnDataReady;
                _psRunspace = RunspaceFactory.CreateRunspace(host);
                _psRunspace.Open();
            }
            catch (Exception ex)
            {
                var error = "Error opening runspace: " + ex.Message;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                throw new Exception(error);
            }
        }

        private void HostOnDataReady(object sender, string message)
        {
            Console.Write(message);
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

            try
            {
                var exit = new List<int>();

                var pipeline = _psRunspace.CreatePipeline();
                
                pipeline.Commands.AddScript(scriptBlock);

                pipeline.Error.DataReady += (sender, args) =>
                {
                    var data = pipeline.Error.Read(1);
                    if (data.Count > 0)
                    {
                        var errors = ProcessErrors(data);
                        Console.WriteLine(errors);
                    }
                };

                pipeline.Output.DataReady += (sender, args) =>
                {
                    var psObjects = pipeline.Output.Read(1);

                    if (psObjects.Count > 0)
                    {
                        Console.WriteLine(ProcessResult(psObjects));
                    }
                    if (PsResultIsFalse(psObjects))
                    {
                        exit.Add(1);
                    }
                    else if (psObjects.Count > 0)
                    {
                        exit.Add(0);
                    }
                    // handle end
                };

                pipeline.Input.Close();
                try
                {
                    pipeline.Invoke();
                }
                catch (CmdletInvocationException err)
                {
                    exit.Clear();
                    exit.Add(1);
                    if (err.ErrorRecord != null)
                    {
                        Console.WriteLine("ERROR: " + err.ErrorRecord);
                        Console.WriteLine(err.ErrorRecord.ScriptStackTrace);
                    }
                    else
                    {
                        Console.WriteLine(err);
                    }
                }
                catch (RuntimeException err)
                {
                    exit.Clear();
                    exit.Add(1);
                    if (err.ErrorRecord != null)
                    {
                        Console.WriteLine("ERROR: " + err.ErrorRecord);
                        Console.WriteLine(err.ErrorRecord.ScriptStackTrace);
                    }
                    else
                    {
                        Console.WriteLine(err);
                    }
                }

                if (pipeline.PipelineStateInfo.State == PipelineState.Failed)
                {
                    return "Exit1:" + pipeline.PipelineStateInfo.Reason.Message;
                }
                return "Exit" + (exit.Count == 1 ? exit[0] : 0) + ":";
            }
            catch (Exception ex)
            {
                var output = "Exception Invoking script block: " +
                         "Reason:" + Environment.NewLine +
                         ex;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, output);
                return "Exit1:" + output;
            }
        }

        private static bool PsResultIsFalse(Collection<PSObject> psObjects)
        {
            return psObjects.Count == 1 && psObjects[0] != null && psObjects[0].BaseObject is bool && (bool)psObjects[0].BaseObject == false;
        }

        private string ProcessErrors(Collection<object> errorCollection)
        {
            if (errorCollection != null)
            {
                var message = "";
                foreach (var er in errorCollection)
                {
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Warning,
                        "Collecting error messages...");

                    var psObject = er as PSObject;
                    if (psObject != null)
                    {
                        message = ProcessError(psObject);
                    }
                    else
                    {
                        var psError = er as ErrorRecord;
                        message = ProcessError(psError);
                    }

                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
                }
                return "Exit1:" + message;
            }
            return "Exit1:No errors found, but still error.";
        }

        private static string ProcessError(PSObject er)
        {
            if (er.BaseObject is string)
            {
                return (string)er.BaseObject;
            }
            return er.ToString();
        }

        private static string ProcessError(ErrorRecord er)
        {
            var errors = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(er.Exception.Message))
                {
                    errors += "Message: " + er.Exception.Message + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.Exception.Source))
                {
                    errors += "Source: " + er.Exception.Source + Environment.NewLine;
                }
                if (er.Exception.InnerException != null &&
                    !string.IsNullOrEmpty(er.Exception.InnerException.ToString()))
                {
                    errors += "InnerException: " + er.Exception.InnerException + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.Exception.StackTrace))
                {
                    errors += "StackTrace: " + er.Exception.StackTrace + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.Exception.HelpLink))
                {
                    errors += "HelpLink: " + er.Exception.HelpLink + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.Exception.TargetSite.ToString()))
                {
                    errors += "TargetSite: " + er.Exception.TargetSite + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.Exception.Data.ToString()))
                {
                    errors += "Exception Data: " + er.Exception.Data + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(er.ScriptStackTrace))
                {
                    errors += "ScriptStackTrace: " + er.ScriptStackTrace + Environment.NewLine;
                }
                errors += "--------------";
            }
            catch (Exception ex)
            {
                errors += "Error on collecting PowerShell exception messages: " + ex.Message;
            }

            var message = "Executed and returned the following errors:" +
                          Config.SeparatorLine + errors;
            return message;
        }

        private string ProcessResult(Collection<PSObject> psObjects)
        {
            var output = "Execution of Script Block: ";
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
                        if (psObjects[0]?.BaseObject == null)
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
                            if (pso?.BaseObject == null)
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
            _psRunspace?.Dispose();
        }
    }
}
