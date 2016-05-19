using System;
using System.IO.Pipes;

namespace BluIpc.Server
{
    // Internal data associated with pipes
    struct IpcPipeData
    {
        public PipeStream Pipe;
        public Object State;
        public Byte[] Data;
    };
}
