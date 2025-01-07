using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputeSharp;
using TerraFX.Interop.Windows;
using System.Numerics;
using model.slimeMould;

namespace model
{
    public partial class GpuSlimeMould : ISlimeMould
    {
        private ReadWriteTexture2D<int> grid;
        private ReadWriteBuffer<Agent> agents;

        public SlimeMouldParams Parameters { get; }

        public GpuSlimeMould(SlimeMouldParams parameters)
        {
            this.Parameters = parameters;

            Random random = new Random();
            var startingAgents = new List<Agent>();
            for (int i = 0; i < Parameters.agents; i++)
            {
                startingAgents.Add(new Agent(
                    new float2(random.Next(0, Parameters.width), random.Next(0, Parameters.height)),
                    random.Next(0, 360))
                    );
            }

            var size = new int[Parameters.width, Parameters.height];
            this.grid = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<int>(size);
            this.agents = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<Agent>(startingAgents.ToArray());
        }

        public MonoGrid getState()
        {
            MonoGrid result = new MonoGrid(Parameters.width, Parameters.height);
            result.setData(grid.ToArray());

            return result;
        }

        public void step()
        {
            drawAgents();
            updateAgents();
            fade();
        }

        private void drawAgents()
        {
            GraphicsDevice.GetDefault().For(agents.Length, new
                DrawAgentsShader(agents, grid));
        }

        private void fade()
        {
            GraphicsDevice.GetDefault().For(grid.Width, grid.Height, new
                FadeShader(grid, Parameters.fadeFactor));
        }

        private void updateAgents()
        {
            GraphicsDevice.GetDefault().For(agents.Length, new
                UpdateAgentsShader(agents, grid, Parameters.width, Parameters.height, Parameters.speed, Parameters.lookAngle, Parameters.lookCount, Parameters.lookGrowth, Parameters.turnStrength));
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
            public readonly ReadWriteBuffer<Agent> agents;
            public readonly ReadWriteTexture2D<int> buffer;

            public void Execute()
            {
                buffer[(int)MathF.Floor(agents[ThreadIds.X].position.Y), (int)MathF.Floor(agents[ThreadIds.X].position.X)] = 100;
            }
        }

        [AutoConstructor]
        private readonly partial struct UpdateAgentsShader : IComputeShader
        {
            public readonly ReadWriteBuffer<Agent> agents;
            public readonly ReadWriteTexture2D<int> buffer;
            public readonly int Width;
            public readonly int Height;
            public readonly float Speed;
            public readonly int LookAngle, LookCount;
            public readonly float LookGrowth, TurnStrength;

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

                    int value = buffer[new int2((int)MathF.Floor(position.Y), (int)MathF.Floor(position.X))];
                    result += value;
                    current += lookaheadGrowth;
                }

                return result;
            }

            public void Execute()
            {
                const float lookaheadStart = 1f;

                var agent = agents[ThreadIds.X];
                float newRotation = agent.rotation;
                float2 newPosition = agent.position;

                //Update rotation
                int left = countLookAhead(agent, -LookAngle, LookCount, LookGrowth, lookaheadStart);
                int ahead = countLookAhead(agent, 0, LookCount, LookGrowth, lookaheadStart);
                int right = countLookAhead(agent, LookAngle, LookCount, LookGrowth, lookaheadStart);

                if (left > right)
                {
                    newRotation -= TurnStrength;
                }
                if (right > left)
                {
                    newRotation += TurnStrength;
                }

                newRotation = newRotation % 360;

                //Update position
                float angleInRadians = agent.rotation * (MathF.PI / 180);
                newPosition.X += MathF.Cos(angleInRadians) * Speed;
                newPosition.Y += MathF.Sin(angleInRadians) * Speed;

                newPosition.X = (newPosition.X + Width) % Width;
                newPosition.Y = (newPosition.Y + Height) % Height;

                agents[ThreadIds.X].position = newPosition;
                agents[ThreadIds.X].rotation = newRotation;
            }
        }
    }
}
