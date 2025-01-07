using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public struct SlimeMouldParams
    {
        //Main options
        public int width, height, agents;
        //Agent options
        public int fadeFactor;
        public float speed;
        //Agent behaviour
        public int lookAngle, lookCount;
        public float lookGrowth, turnStrength;
    }
}
