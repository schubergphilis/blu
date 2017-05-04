using System;
using System.Diagnostics;
using System.Security;

using Blu.core.common;

namespace BluService
{
    public class UserData : EventArgs, IDisposable
    {
        public UserData(string userDataHeader)
        {
            var parts = userDataHeader.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                User = parts[0];
                Domain = Environment.MachineName;
                Password = GetSecureString(parts[1]);
            }
            else if (parts.Length == 3)
            {
                User = parts[0];
                Domain = parts[1];
                Password = GetSecureString(parts[2]);
            }
            else
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, "PowerShellRunspace.UserData: unable to parse: " + userDataHeader);
                throw new Exception("Unable to parse userdata from: " + userDataHeader);
            }
        }

        private SecureString GetSecureString(string text)
        {
            var chars = text.ToCharArray();
            var sec = new SecureString();
            foreach (var c in chars)
            {
                sec.AppendChar(c);
            }
            return sec;
        }

        public string User { get; private set; }
        public string Domain { get; private set; }
        public SecureString Password { get; private set; }
        public void Dispose()
        {
            User = null;
            Domain = null;
            if (Password != null)
            {
                Password.Dispose();
            }
            Password = null;
            GC.Collect();
        }
    }
}
