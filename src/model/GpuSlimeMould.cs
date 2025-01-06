using model.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using TerraFX.Interop.Windows;

namespace model
{
    public partial class GpuSlimeMould : ISlimeMould
    {
        private MonoGrid grid;
        private List<Agent> agents;

        public int Width { get; }
        public int Height { get; }

        public GpuSlimeMould(int width, int height, int agents)
        {
            this.Width = width;
            this.Height = height;
            this.grid = new MonoGrid(width, height);

            Random random = new Random();
            this.agents = new List<Agent>();
            for (int i = 0; i < agents; i++)
            {
                this.agents.Add(new Agent(
                    (random.Next(0, width), random.Next(0, height)),
                    random.Next(0, 360))
                    );
            }
        }

        public MonoGrid getState()
        {
            return new MonoGrid(grid);
        }

        public void step()
        {
            drawAgents();
            updateAgents();
            fade(1);
        }

        private int countLookAhead(Agent agent, int angle, int lookCount, float lookaheadGrowth, float lookaheadStart)
        {
            int result = 0;

            float current = lookaheadStart;
            for (int i = 0; i < lookCount; i++)
            {
                double angleInRadians = agent.rotation * (Math.PI / 180);

                (int x, int y) position = (
                    (int)Math.Floor(agent.position.x + (Math.Cos(angleInRadians) + current)),
                    (int)Math.Floor(agent.position.y + (Math.Sin(angleInRadians) + current))
                    );

                result += grid.safeGetValue(position.x, position.y);
                current += lookaheadGrowth;
            }

            return result;
        }

        private void updateAgents()
        {
            foreach (Agent agent in agents)
            {
                //Update rotation
                int left = countLookAhead(agent, -45, 8, 1.5f, 1.2f);
                int ahead = countLookAhead(agent, 0, 8, 1.5f, 1.2f);
                int right = countLookAhead(agent, 45, 8, 1.5f, 1.2f);

                if (left > right && left > ahead)
                {
                    agent.rotation -= 10;
                }
                if (right > left && right > ahead)
                {
                    agent.rotation += 10;
                }

                agent.rotation %= 360;

                //Update position
                agent.position.x += (float)Math.Cos(agent.rotation);
                agent.position.y += (float)Math.Sin(agent.rotation);

                if ((int)Math.Floor(agent.position.x) >= Width)
                {
                    agent.position.x = 0;
                }
                if ((int)Math.Floor(agent.position.y) >= Height)
                {
                    agent.position.y = 0;
                }
                if ((int)Math.Floor(agent.position.x) < 0)
                {
                    agent.position.x = Width - 1;
                }
                if ((int)Math.Floor(agent.position.y) < 0)
                {
                    agent.position.y = Height - 1;
                }
            }
        }

        private void drawAgents()
        {
            foreach (Agent agent in agents)
            {
                this.grid.setValue((int)Math.Floor(agent.position.Item1), (int)Math.Floor(agent.position.Item2), 100);
            }
        }

        private void fade(int factor)
        {
            var texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(grid.getData());

            GraphicsDevice.GetDefault().For(Width, Height, new
                FadeShader(texture, factor));

            grid.setData(texture.ToArray());
            texture.Dispose();
        }

        [AutoConstructor]
        public readonly partial struct FadeShader : IComputeShader
        {
            public readonly ReadWriteTexture2D<int> buffer;
            public readonly int fadeFactor;

            public void Execute()
            {
                if (buffer[ThreadIds.XY] > 0)
                {
                    buffer[ThreadIds.XY] -= fadeFactor;
                }
            }
        }
    }
}
