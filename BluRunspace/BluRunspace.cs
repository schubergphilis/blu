using System;
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
        private PowerShell _psRunspace;

        private void OpenRunspace()
        {
            try
            {
                _psRunspace = PowerShell.Create();
                _psRunspace.Streams.Debug.DataAdded += DebugOnDataAdded;
                _psRunspace.Streams.Error.DataAdded += ErrorOnDataAdded;
                _psRunspace.Streams.Progress.DataAdded += ProgressOnDataAdded;
                _psRunspace.Streams.Verbose.DataAdded += VerboseOnDataAdded;
                _psRunspace.Streams.Warning.DataAdded += WarningOnDataAdded;
                _psRunspace.Streams.Information.DataAdded += InformationOnDataAdded;
            }
            catch (Exception ex)
            {
                var error = "Error opening runspace: " + ex.Message;
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                throw new Exception(error);
            }
        }

        private void InformationOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<InformationRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine(record.Computer);
        }

        private void WarningOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<WarningRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine(record.Message);
        }

        private void VerboseOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<VerboseRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine(record.Message);
        }

        private void ProgressOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<ProgressRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine("Progress: " + record.PercentComplete);
        }

        private void ErrorOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<ErrorRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine(record.ErrorDetails.Message);
            Console.WriteLine(record.ErrorDetails.RecommendedAction ?? "");
        }

        private void DebugOnDataAdded(object sender, DataAddedEventArgs dataAddedEventArgs)
        {
            var record = ((PSDataCollection<DebugRecord>)sender)[dataAddedEventArgs.Index];
            Console.WriteLine(record.Message);
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
                var exit = 0;

                var pipeline = _psRunspace.Runspace.CreatePipeline();
                
                pipeline.Commands.AddScript(scriptBlock);

                pipeline.Output.DataReady += (sender, args) =>
                {
                    var psObjects = pipeline.Output.NonBlockingRead();

                    // if there were any, invoke the DataReady event
                    if (psObjects.Count > 0)
                    {
                        Console.WriteLine(ProcessResult(psObjects));
                    }

                    if (pipeline.Output.EndOfPipeline)
                    {
                        if (PsResultIsFalse(psObjects))
                        {
                            exit = 1;
                        }
                        // handle end
                    }
                };

                pipeline.InvokeAsync();

                while (pipeline.PipelineStateInfo.State != PipelineState.Completed &&
                       pipeline.PipelineStateInfo.State != PipelineState.Failed &&
                       pipeline.PipelineStateInfo.State != PipelineState.Stopped)
                {
                    Thread.Sleep(10);
                }
                if (pipeline.PipelineStateInfo.State == PipelineState.Failed)
                {
                    exit = 1;
                }
                return "Exit" +  exit + ":";
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
            return psObjects.Count == 1 && psObjects[0].BaseObject is bool && (bool)psObjects[0].BaseObject == false;
        }

        //private string ProcessErrors(Pipeline pipeline, string scriptBlock)
        //{
        //    var errorObject = pipeline.Error.Read();
        //    var errorRecords = errorObject as Collection<ErrorRecord>;
        //    if (errorRecords != null)
        //    {
        //        var errors = string.Empty;
        //        foreach (var er in errorRecords)
        //        {
        //            EventLogHelper.WriteToEventLog(EventLogEntryType.Warning,
        //                "Collecting error messages...");
        //            try
        //            {
        //                if (!string.IsNullOrEmpty(er.Exception.Message))
        //                    errors += "Message: " + er.Exception.Message + Environment.NewLine;
        //                if (!string.IsNullOrEmpty(er.Exception.Source))
        //                    errors += "Source: " + er.Exception.Source + Environment.NewLine;
        //                if (er.Exception.InnerException != null &&
        //                    !string.IsNullOrEmpty(er.Exception.InnerException.ToString()))
        //                    errors += "InnerException: " + er.Exception.InnerException + Environment.NewLine;
        //                if (!string.IsNullOrEmpty(er.Exception.StackTrace))
        //                    errors += "StackTrace: " + er.Exception.StackTrace + Environment.NewLine;
        //                if (!string.IsNullOrEmpty(er.Exception.HelpLink))
        //                    errors += "HelpLink: " + er.Exception.HelpLink + Environment.NewLine;
        //                if (!string.IsNullOrEmpty(er.Exception.TargetSite.ToString()))
        //                    errors += "TargetSite: " + er.Exception.TargetSite + Environment.NewLine;
        //                if (!string.IsNullOrEmpty(er.Exception.Data.ToString()))
        //                    errors += "Exception Data: " + er.Exception.Data + Environment.NewLine;
        //                errors += "--------------";
        //            }
        //            catch (Exception ex)
        //            {
        //                errors += "Error on collecting PowerShell exception messages: " + ex.Message;
        //            }

        //            var message = "Script Block: " + scriptBlock.FormatForEventLog() +
        //                          "Executed and returned the following errors:" +
        //                          Config.SeparatorLine + errors;

        //            EventLogHelper.WriteToEventLog(EventLogEntryType.Error, message);
        //            return "Exit1:" + message;
        //        }
        //    }
        //    return "Errors executing, errorObject: " + errorObject;
        //}

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
