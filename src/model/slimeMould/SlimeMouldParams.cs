using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.slimeMould
{
    public struct SlimeMouldParams
    {
        //Main options
        public int width, height, agents;
        public Func<int, int, Agent> startingAgentSetup;
        //Agent options
        public int fadeFactor;
        public float speed;
        //Agent behaviour
        public int lookAngle, lookCount;
        public float lookGrowth, turnStrength, followWeight;

        public static SlimeMouldParams GetDefault()
        {
            return new SlimeMouldParams()
            {
                width = 1920,
                height = 1080,
                agents = 100000,
                startingAgentSetup = (width, height) =>
                {
                    Random random = new Random();
                    return new Agent(
                        new float2(random.Next(0, width), random.Next(0, height)),
                        random.Next(0, 360),
                        random.Next());
                },
                speed = 1,
                fadeFactor = 2,
                lookAngle = 45,
                turnStrength = 20,
                lookCount = 50,
                lookGrowth = 1.2f,
                followWeight = 2
            };
        }
    }
}
