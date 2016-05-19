using System;
using System.IO.Pipes;

namespace BluIpc.Server
{
    // Interface for user code to receive notifications regarding pipe messages
    public interface IIpcCallback
    {
        void OnAsyncConnect(PipeStream pipe, out Object state);
        void OnAsyncDisconnect(PipeStream pipe, Object state);
        void OnAsyncMessage(PipeStream pipe, Byte[] data, Int32 bytes, Object state);
    }
}
