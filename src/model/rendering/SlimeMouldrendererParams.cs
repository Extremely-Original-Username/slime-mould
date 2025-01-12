using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public struct SlimeMouldrendererParams
    {
        public int length, fps;
        public string outDir;
        public Color background, foreground;

        public static SlimeMouldrendererParams GetDefault()
        {
            return new SlimeMouldrendererParams()
            {
                length = 20,
                fps = 20,
                outDir = "results",
                background = Color.FromArgb(255, 0, 0, 0),
                foreground = Color.FromArgb(255, 255, 255, 255)
            };
        }
    }
}
