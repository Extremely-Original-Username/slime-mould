using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    internal class Agent
    {
        public (float x, float y) position;
        public float rotation;
        public Agent((float x, float y) position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
