using System;
using System.Diagnostics;

namespace BluIpc.Common
{
    public static class EventLogHelper
    {
        public static void WriteToEventLog(string serviceName, int type, string message)
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
                switch (type)
                {
                    // Info
                    case 0:
                        elog.WriteEntry(tMessage, EventLogEntryType.Information, 271);
                        break;
                    // Warning
                    case 1:
                        elog.WriteEntry(tMessage, EventLogEntryType.Warning, 271);
                        break;
                    // Error
                    case 2:
                        elog.WriteEntry(tMessage, EventLogEntryType.Error, 271);
                        break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}

