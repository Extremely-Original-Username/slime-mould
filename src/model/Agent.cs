using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    internal class Agent
    {
        public (float, float) position;
        public float rotation;
        public Agent((float, float) position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
