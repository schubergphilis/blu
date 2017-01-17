using System;
using System.ComponentModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace BluService
{
    

    /// <summary>
    /// Allows code to be executed under the security context of a specified user account.
    /// </summary>
    /// <remarks> 
    ///
    /// Implements IDispose, so can be used via a using-directive or method calls;
    ///  ...
    ///
    ///  var imp = new Impersonator( "myUsername", "myDomainname", "myPassword" );
    ///  imp.UndoImpersonation();
    ///
    ///  ...
    ///
    ///   var imp = new Impersonator();
    ///  imp.Impersonate("myUsername", "myDomainname", "myPassword");
    ///  imp.UndoImpersonation();
    ///
    ///  ...
    ///
    ///  using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
    ///  {
    ///   ...
    ///   
    ///   ...
    ///  }
    ///
    ///  ...
    /// </remarks>
    public class Impersonator : IDisposable
    {

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        private WindowsImpersonationContext _wic;

        /// <summary>
        /// Begins impersonation with the given credentials, Logon type and Logon provider.
        /// </summary>
        /// <param name = "userName" > Name of the user.</param>
        /// <param name = "domainName" > Name of the domain.</param>
        /// <param name = "password" > The password. <see cref = "System.String" /></ param >
        /// <param name="logonType">Type of the logon.</param>
        /// <param name = "logonProvider" > The logon provider.</ param >
        public Impersonator(string userName, string domainName, string password, LogonProvider logonProvider)
        {
            Impersonate(userName, domainName, password, logonProvider);
        }

        /// <summary>
        /// Begins impersonation with the given credentials.
        /// </summary>
        /// <param name = "userName" > Name of the user.</param>
        /// <param name = "domainName" > Name of the domain.</param>
        /// <param name = "password" > The password. <see cref = "System.String" /></ param >
        public Impersonator(string userName, string domainName, string password)
        {
            Impersonate(userName, domainName, password, LogonProvider.Logon32ProviderDefault);
        }

        /// <summary>
        /// Begins impersonation with the given credentials for a local account.
        /// </summary>
        /// <param name = "userName" > Name of the user.</param>
        /// <param name = "password" > The password. <see cref = "System.String" /></ param >
        public Impersonator(string userName, string password)
        {
            Impersonate(userName, Environment.MachineName, password, LogonProvider.Logon32ProviderDefault);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Impersonator"/> class.
        /// </summary>
        public Impersonator()
        { }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            UndoImpersonation();
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name = "userName" > Name of the user.</param>
        /// <param name = "domainName" > Name of the domain.</param>
        /// <param name = "password" > The password. <see cref = "System.String" /></param >
        public void Impersonate(string userName, string domainName, string password)
        {
            Impersonate(userName, domainName, password, LogonProvider.Logon32ProviderDefault);
        }

        /// <summary>
        /// Impersonates the specified user account.
        /// </summary>
        /// <param name = "userName" > Name of the user.</param>
        /// <param name = "domainName" > Name of the domain.</param>
        /// <param name = "password" > The password. <see cref = "System.String" /></param >
        /// < param name="logonType">Type of the logon.</param>
        /// <param name = "logonProvider" > The logon provider.</param >
        public void Impersonate(string userName, string domainName, string password, LogonProvider logonProvider)
        {
            UndoImpersonation();

            var logonToken = IntPtr.Zero;
            try
            {
                if (LogonUser(userName,
                        domainName,
                        password,
                        (int) LogonType.Logon32LogonInteractive,
                        (int) logonProvider,
                        ref logonToken) != 0)
                {
                    _wic = WindowsIdentity.Impersonate(logonToken);
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                {
                    CloseHandle(logonToken);
                }
            }
        }

        /// <summary>
        /// Stops impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            // restore saved requestor identity
            if (_wic != null)
                _wic.Undo();
            _wic = null;
        }
    }

    public enum LogonType
    {
        Logon32LogonInteractive = 2,
        Logon32LogonNetwork = 3,
        Logon32LogonBatch = 4,
        Logon32LogonService = 5,
        Logon32LogonUnlock = 7,
        Logon32LogonNetworkCleartext = 8, // Win2K or higher
        Logon32LogonNewCredentials = 9 // Win2K or higher
    };

    public enum LogonProvider
    {
        Logon32ProviderDefault = 0,
        Logon32ProviderWinnt35 = 1,
        Logon32ProviderWinnt40 = 2,
        Logon32ProviderWinnt50 = 3
    };
}
