using System;
using System.Diagnostics;

namespace Blu.core.common
{
    public static class EventLogHelper
    {
        static object eloglock = new object();
        static EventLog elog = new EventLog { Source = Config.ServiceName };
        
        public static void WriteToEventLog(EventLogEntryType type, string message)
        {
            lock (eloglock)
            {
                // Truncate output if it is longer than 32756 otherwise we can't write it to EventLog
                string tMessage;
                if (message.Length > 8192)
                {
                    tMessage = message.Truncate(8192) + " ... (string is truncated)";
                }
                else
                {
                    tMessage = message;
                }

                try
                {
                    elog.WriteEntry(tMessage, type, 271);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}

