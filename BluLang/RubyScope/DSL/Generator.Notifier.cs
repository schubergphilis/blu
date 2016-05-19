using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {
        /// <summary>
        /// Adds notifier to the notifiers dictionary (_result["notifiers"])
        /// </summary>
        /// <param name="ResourceUniqueName">Resource unique name in format: resource->"name"</param>
        /// <param name="parameterValue">A list of parameters value, when paremeter is 'notifies'</param>
        private void AddNotifier(string ResourceUniqueName, List<string> parameterValue)
        {
            // Notifier list always contains 3 items
            List<string> notifier = new List<string>();

            // Item 1 (notifier[0]) = action (e.g. :run)
            if (parameterValue[1] != null) notifier.Add(parameterValue[1]);
            else
                throw new ArgumentException("Notifier action argument can not be null or empty");
            
            // Item 2 (notifier[1]) = ResourceUniqeName to be notified
            if (parameterValue[2] != null) notifier.Add(TransformNotifierUniqueName(parameterValue[2]));
            else
                throw new ArgumentException("Notifier resource argument can not be null or empty");
            
            // Item 3 (nogifier[2]) = :timer (:delayed or :immediately) default is :delayed
            if (parameterValue.Count > 3)
            {
                notifier.Add(parameterValue[4] ?? "delayed");
            }

            // Add notifier to result dictionary: _result["notifiers"]
            _result["notifiers"].Add(ResourceUniqueName, notifier);
        }
    }
}
