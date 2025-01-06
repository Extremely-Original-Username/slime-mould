using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    internal struct Agent
    {
        public Position position;
        public float rotation;
        public Agent(Position position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    internal struct Position
    {
        public float x;
        public float y;

        public Position(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
