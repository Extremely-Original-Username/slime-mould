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

        public static SlimeMouldParams GetDefault()
        {
            return new SlimeMouldParams()
            {
                width = 1920,
                height = 1080,
                agents = 100000,
                speed = 1,
                fadeFactor = 2,
                lookAngle = 45,
                turnStrength = 20,
                lookCount = 50,
                lookGrowth = 1.2f
            };
        }
    }
}
