using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public struct SlimeMouldrendererParams
    {
        public int length, fps;
        public string outDir;

        public static SlimeMouldrendererParams GetDefault()
        {
            return new SlimeMouldrendererParams()
            {
                length = 20,
                fps = 20,
                outDir = "results"
            };
        }
    }
}
