using model.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using TerraFX.Interop.Windows;
using System.Numerics;

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
                    new Position(random.Next(0, width), random.Next(0, height)),
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

        private void updateAgents()
        {
            var texture = GraphicsDevice.GetDefault().AllocateReadOnlyTexture2D<int>(grid.getData());
            var agentsBuffer = GraphicsDevice.GetDefault().AllocateReadWriteTexture1D<Agent>(agents.ToArray());

            GraphicsDevice.GetDefault().For(agents.Count, new
                UpdateAgentsShader(agentsBuffer, texture, Width, Height));

            agents = new List<Agent>(agentsBuffer.ToArray());
            texture.Dispose();
        }

        private void drawAgents()
        {
            var texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(grid.getData());
            var positions = GraphicsDevice.GetDefault().AllocateReadOnlyTexture1D<int2>(agents.Select(x => new int2((int)Math.Floor(x.position.x), (int)Math.Floor(x.position.y))).ToArray());

            GraphicsDevice.GetDefault().For(agents.Count, new
                DrawAgentsShader(positions, texture));

            grid.setData(texture.ToArray());
            texture.Dispose();
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

        [AutoConstructor]
        private readonly partial struct DrawAgentsShader : IComputeShader
        {
            public readonly ReadOnlyTexture1D<int2> positions;
            public readonly ReadWriteTexture2D<int> buffer;

            public void Execute()
            {
                buffer[positions[ThreadIds.X]] = 100;
            }
        }

        [AutoConstructor]
        private readonly partial struct UpdateAgentsShader : IComputeShader
        {
            public readonly ReadWriteTexture1D<Agent> agents;
            public readonly ReadOnlyTexture2D<int> buffer;
            public readonly int Width;
            public readonly int Height;

            private int countLookAhead(Agent agent, int angle, int lookCount, float lookaheadGrowth, float lookaheadStart)
            {
                int result = 0;

                float current = lookaheadStart;
                for (int i = 0; i < lookCount; i++)
                {
                    double angleInRadians = agent.rotation * (Math.PI / 180);

                    Position position = new Position(
                        (int)Math.Floor(agent.position.x + (Math.Cos(angleInRadians) + current)),
                        (int)Math.Floor(agent.position.y + (Math.Sin(angleInRadians) + current))
                        );

                    int value = 0;
                    if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
                    {
                        value = buffer[(int)agent.position.x, (int)agent.position.y];
                    }
                    result += value;
                    current += lookaheadGrowth;
                }

                return result;
            }

            public void Execute()
            {
                var agent = agents[ThreadIds.X];

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
    }
}
