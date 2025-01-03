using model.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class SlimeMould : ISlimeMould
    {
        private MonoGrid grid;
        private List<Agent> agents;

        public SlimeMould(int width, int height, int agents)
        {
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
            fade(1);
            updateAgents();
            drawAgents();
        }

        private void updateAgents()
        {
            foreach (Agent agent in agents)
            {
                agent.position.Item1 += 0.1f;
                agent.position.Item2 += 0.1f;

                if ((int)Math.Floor(agent.position.Item1) >= grid.width)
                {
                    agent.position.Item1 = 0;
                }
                if ((int)Math.Floor(agent.position.Item2) >= grid.height)
                {
                    agent.position.Item2 = 0;
                }

                //TODO: Expand this for the opposite
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
            for (int y = 0; y < this.grid.height; y++)
            {
                for (int x = 0; x < this.grid.width; x++)
                {
                    this.grid.setValue(x, y, Math.Max(this.grid.getValue(x, y) - factor, 0));
                }
            }
        }
    }
}
