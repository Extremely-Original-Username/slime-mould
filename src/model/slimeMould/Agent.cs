using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    [AutoConstructor]
    public partial struct Agent
    {
        public float2 position;
        public float rotation;
        public int randomSeed;
    }
}
