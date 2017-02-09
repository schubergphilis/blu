using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace BluShell
{
    [Verb("runspace", HelpText = "Instantiate a new runspace.")]
    class RunspaceOptions
    {
        [Option('d', "dispose", Required = false, HelpText = "Delete runspace.")]
        public bool Delete { get; set; }
        
        [Option("credentials", Required = false, HelpText = "Credentials to be used, format as: <user>,[<domain>,]<password>")]
        public string Credentials { get; set; }

        [Value(0, MetaName = "Runspace", Required = true, HelpText = "Runspace to create/delete.")]
        public string Runspace { get; set; }

        [Usage(ApplicationAlias = "BluShell")]
        public static IEnumerable<Example> Usage
        {
            get
            {
                yield return new Example("Create a runspace", new RunspaceOptions { Runspace = "admin", Credentials = "admin,example.local,Welcome123!"});
                yield return new Example("Delete a runspace", new RunspaceOptions { Runspace = "admin", Delete = true });
            }
        }
    }
}
