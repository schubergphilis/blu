using System;
using System.Diagnostics;
using System.ServiceProcess;
using BluIpc.Server;
using BluIpc.Common;

namespace BluService
{
    public partial class BluService : ServiceBase
    {
        readonly BluIpcServer _bluIpcServer;
        
        public BluService()
        {
            InitializeComponent();
            _bluIpcServer = new BluIpcServer();
        }

        protected override void OnStart(string[] args)
        {
            InitializeEventLog();
            InitializeRunspace();
            _bluIpcServer.Start();
        }

        protected override void OnStop()
        {
            _bluIpcServer.Stop();
        }

        private void InitializeEventLog()
        {
            // Create eventlog source if not exists
            if (!EventLog.SourceExists(Config.ServiceName))
                EventLog.CreateEventSource(Config.ServiceName, "Application");
            if (!EventLog.SourceExists(Config.ShellName))
                EventLog.CreateEventSource(Config.ShellName, "Application");
        }

        private void InitializeRunspace()
        {
            try
            {
                PowerShellRunspace.PsRunspace.Open();
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteToEventLog(Config.ServiceName, 2, "Error initializing runspace: " + ex.Message);
            }
        }
    }
}
