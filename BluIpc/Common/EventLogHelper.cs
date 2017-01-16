using System;
using System.Diagnostics;

namespace BluIpc.Common
{
    public static class EventLogHelper
    {
        public static void WriteToEventLog(string serviceName, EventLogEntryType type, string message)
        {
            EventLog elog = new EventLog { Source = serviceName, EnableRaisingEvents = true };
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

