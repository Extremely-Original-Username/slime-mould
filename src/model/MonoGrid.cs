using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class MonoGrid
    {
        public readonly int width, height, min, max;
        private int[,] grid;

        public MonoGrid(int width, int height)
        {
            this.min = 0;
            this.max = 100;
            this.width = width;
            this.height = height;

            this.grid = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    setValue(x, y, 0);
                }
            }
        }

        public MonoGrid(MonoGrid gridToCopy)
        {
            this.min = gridToCopy.min;
            this.max = gridToCopy.max;
            this.width = gridToCopy.width;
            this.height = gridToCopy.height;

            this.grid = gridToCopy.grid;
        }

        public void setValue(int x, int y, int val)
        {
            validateBounds(x, y);
            validateGridValue(val);

            this.grid[x, y] = val;
        }

        public int getValue(int x, int y)
        {
            validateBounds(x, y);

            return this.grid[x, y];
        }

        public int safeGetValue(int x, int y)
        {
            try
            {
                validateBounds(x, y);

                return this.grid[x, y];
            }
            catch (IndexOutOfRangeException e) { }

            return 0;
        }

        private void validateBounds(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return;
            }
            throw new IndexOutOfRangeException("The x or y values are outwith the monogrid's bounds.");
        }

        private void validateGridValue(int val)
        {
            if (val >= this.min && val <= this.max)
            {
                return;
            }
            throw new InvalidOperationException("The provided value cannot be set on the monogrid.");
        }
    }
}
