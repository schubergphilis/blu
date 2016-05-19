using System;
using System.IO.Pipes;

namespace BluIpc.Client
{
    internal class IpcClient
    {
        private readonly NamedPipeClientStream _pipe;
        public const Int32 ClientInBufferSize = 32768;

        public IpcClient(String serverName, String pipename)
        {
            _pipe = new NamedPipeClientStream(
                serverName,
                pipename,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
                );
        }

        public PipeStream Connect(Int32 timeout)
        {
            // NOTE: will throw on failure
            _pipe.Connect(timeout);

            // Must Connect before setting ReadMode
            _pipe.ReadMode = PipeTransmissionMode.Message;

            return _pipe;
        }
    }
}