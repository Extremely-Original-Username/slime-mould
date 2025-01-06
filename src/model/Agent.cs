using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    [AutoConstructor]
    public readonly struct Agent
    {
        public readonly float2 position;
        public readonly float rotation;
    }
}
