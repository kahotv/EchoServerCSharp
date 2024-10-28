using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServerCSharp
{
    internal interface IEchoServer
    {
        abstract Task Start(ushort port);
    }
}
