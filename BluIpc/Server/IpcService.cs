using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace BluIpc.Server
{
    public class IpcService
    {
        public const Int32 ServerInBufferSize = 32768;
        public const Int32 ServerOutBufferSize = 32768;

        private readonly String _pipeName;
        private readonly IIpcCallback _iipcCallback;
        private readonly PipeSecurity _pipeSecurity;
        private bool _running;

        private readonly Dictionary<PipeStream, IpcPipeData> _pipes = new Dictionary<PipeStream, IpcPipeData>();

        public IpcService(
            String pipename,
            IIpcCallback iipcCallback,
            int instances
        )
        {
            _running = true;

            // Save parameters for next new pipe
            _pipeName = pipename;
            _iipcCallback = iipcCallback;

            // Provide full access to the current user so more pipe instances can be created
            _pipeSecurity = new PipeSecurity();
            _pipeSecurity.AddAccessRule(
                new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow)
            );

            // Give access to builtin Administrators only 
            _pipeSecurity.AddAccessRule(
                new PipeAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow
                )
            );

            // Start accepting connections
            for (int i = 0; i < instances; ++i)
                IpcServerPipeCreate();
        }

        public void IpcServerStop()
        {
            // Close all pipes asynchronously
            lock (_pipes)
            {
                _running = false;
                foreach (var pipe in _pipes.Keys)
                    pipe.Close();
            }

            // Wait for all pipes to close
            for (; ; )
            {
                int count;
                lock (_pipes)
                {
                    count = _pipes.Count;
                }
                if (count == 0)
                    break;
                Thread.Sleep(5);
            }
        }

        private void IpcServerPipeCreate()
        {
            // Create message-mode pipe to simplify message transition
            // Assume all messages will be smaller than the pipe buffer sizes
            NamedPipeServerStream pipe = new NamedPipeServerStream(
                _pipeName,
                PipeDirection.InOut,
                -1,     // maximum instances
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                ServerInBufferSize,
                ServerOutBufferSize,
                _pipeSecurity
            );

            // Asynchronously accept a client connection
            pipe.BeginWaitForConnection(OnClientConnected, pipe);
        }

        private void OnClientConnected(IAsyncResult result)
        {
            // Complete the client connection
            NamedPipeServerStream pipe = (NamedPipeServerStream)result.AsyncState;
            pipe.EndWaitForConnection(result);

            // Create client pipe structure
            IpcPipeData pd = new IpcPipeData {Pipe = pipe, State = null, Data = new Byte[ServerInBufferSize]};

            // Add connection to connection list
            bool running;
            lock (_pipes)
            {
                running = _running;
                if (running)
                    _pipes.Add(pd.Pipe, pd);
            }

            // If server is still running
            if (running)
            {
                // Prepare for next connection
                IpcServerPipeCreate();

                // Accept messages
                BeginRead(pd);
            }
            else
            {
                pipe.Close();
            }
        }

        private void BeginRead(IpcPipeData pd)
        {
            // Asynchronously read a request from the client
            bool isConnected = pd.Pipe.IsConnected;
            if (isConnected)
            {
                try
                {
                    pd.Pipe.BeginRead(pd.Data, 0, pd.Data.Length, OnAsyncMessage, pd);
                }
                catch (Exception)
                {
                    isConnected = false;
                }
            }

            if (isConnected) return;
            pd.Pipe.Close();
            lock (_pipes)
            {
                bool removed = _pipes.Remove(pd.Pipe);
                Debug.Assert(removed);
            }
        }

        private void OnAsyncMessage(IAsyncResult result)
        {
            // Async read from client completed
            IpcPipeData pd = (IpcPipeData)result.AsyncState;
            Int32 bytesRead = pd.Pipe.EndRead(result);
            if (bytesRead != 0)
                _iipcCallback.OnAsyncMessage(pd.Pipe, pd.Data, bytesRead, pd.State);
            BeginRead(pd);
        }
    }
}