using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Management.Automation;
using BluApi.Common;
using BluLang.RubyScope.Engine;

namespace BluStation.CmdLets
{
    // BluStation Enter-RubyScope method   
    [Cmdlet(VerbsCommon.Enter, "RubyScope")]
    public class EnterRubyScope : PSCmdlet
    {
        /// <summary>
        /// -Script parameter: script to add to the scope (as string)
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Script { get; set; }

        /// <summary>
        /// -RubyFile parameter: script to add to the scope (as file)
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string RubyFile { get; set; }

        /// <summary>
        /// -AbstractScript parameter: abstract script to be added to the scope (as string)
        /// </summary>
        [Parameter(Mandatory = false)]
        public string AbstractScript { get; set; }

        /// <summary>
        /// -Extract parameter: output values to extract from the scope (know words) as string array
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Verbs { get; set; }

        // postScript is the string constructed by either rubyFile or ruby script
        private string _postScript = String.Empty;

        // scopeScript is the final constracted script (as string) that will be send to the scope
        // scopeScropt = abstract + postScript
        private string _scopeScript;

        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));

            if (RubyFile != null && File.Exists(RubyFile))
            {
                _postScript = File.ReadAllText(RubyFile);
            }
            else if (Script != null)
            {
                _postScript = Script;
            }

            if (AbstractScript != null)
            {
                // Combine abstractScript 
                _scopeScript = AbstractScript + Environment.NewLine + _postScript;
            }
            else
            {
                // No abstract script found, just pass the postScript to the scope
                _scopeScript = _postScript;
            }

            SortedDictionary<string, object> inputVariables = new SortedDictionary<string, object>();
            if (Verbs != null)
            {


                string[] verbsArray = Verbs.Replace(" ", String.Empty).Split(',');

                if (verbsArray.Length > 0)
                {
                    foreach (string verb in verbsArray)
                    {
                        inputVariables.Add(verb, new object());
                    }
                }
            }

            RubyEngine rubyEngine = new RubyEngine {Script = _scopeScript, ScriptVariables = inputVariables};
            SortedDictionary<string, object> outputVariables = rubyEngine.Execute();

            WriteObject(outputVariables);
        }

        public void Terminate(string badNews)
        {
            var errorRecord = new ErrorRecord(new Exception(badNews), badNews, ErrorCategory.CloseError, null);
            ThrowTerminatingError(errorRecord);
        }

    }
}