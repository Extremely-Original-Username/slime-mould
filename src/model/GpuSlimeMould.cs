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
                    new float2(random.Next(0, width), random.Next(0, height)),
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

            var agentsArray = agents.ToArray(); // Convert List<Agent> to array
            var agentsBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<Agent>(agentsArray.Length);
            agentsBuffer.CopyFrom(agentsArray); // Ensure data is copied correctly

            GraphicsDevice.GetDefault().For(agents.Count, new
                UpdateAgentsShader(agentsBuffer, texture, Width, Height));

            agents = new List<Agent>(agentsBuffer.ToArray());
            texture.Dispose();
        }

        private void drawAgents()
        {
            var texture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(grid.getData());
            var positions = GraphicsDevice.GetDefault().AllocateReadOnlyTexture1D<int2>(agents.Select(x => new int2((int)Math.Floor(x.position.X), (int)Math.Floor(x.position.Y))).ToArray());

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
            public readonly ReadWriteBuffer<Agent> agents;
            public readonly ReadOnlyTexture2D<int> buffer;
            public readonly int Width;
            public readonly int Height;

            private int countLookAhead(Agent agent, int angle, int lookCount, float lookaheadGrowth, float lookaheadStart)
            {
                int result = 0;

                float current = lookaheadStart;
                for (int i = 0; i < lookCount; i++)
                {
                    float angleInRadians = (agent.rotation + angle) * (MathF.PI / 180);

                    float2 position = new float2(
                        (MathF.Floor(agent.position.X + (MathF.Cos(angleInRadians) * current)) + Width) % Width,
                        (MathF.Floor(agent.position.Y + (MathF.Sin(angleInRadians) * current)) + Height) % Height
                        );

                    int value = buffer[new int2((int)MathF.Floor(position.X), (int)MathF.Floor(position.Y))];
                    result += value;
                    current += lookaheadGrowth;
                }

                return result;
            }

            public void Execute()
            {
                const int looks = 50;
                const float lookaheadGrowth = 1.2f;
                const float lookaheadStart = 1f;
                const float speed = 1;

                var agent = agents[ThreadIds.X];
                float newRotation = agent.rotation;
                float2 newPosition = agent.position;

                //Update rotation
                int left = countLookAhead(agent, -45, looks, lookaheadGrowth, lookaheadStart);
                int ahead = countLookAhead(agent, 0, looks, lookaheadGrowth, lookaheadStart);
                int right = countLookAhead(agent, 45, looks, lookaheadGrowth, lookaheadStart);

                if (left > right)
                {
                    newRotation -= 20;
                }
                if (right > left)
                {
                    newRotation += 20;
                }

                newRotation = newRotation % 360;

                //Update position
                float angleInRadians = agent.rotation * (MathF.PI / 180);
                newPosition.X += MathF.Cos(angleInRadians) * speed;
                newPosition.Y += MathF.Sin(angleInRadians) * speed;

                newPosition.X = (newPosition.X + Width) % Width;
                newPosition.Y = (newPosition.Y + Height) % Height;

                agents[ThreadIds.X].position = newPosition;
                agents[ThreadIds.X].rotation = newRotation;
            }
        }
    }
}
