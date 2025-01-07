using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace model.slimeMould
{
    public class MonoGrid
    {
        public readonly int width, height, min, max;
        private int[,] grid;

        public MonoGrid(int width, int height)
        {
            min = 0;
            max = 100;
            this.width = width;
            this.height = height;

            grid = new int[width, height];

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
            min = gridToCopy.min;
            max = gridToCopy.max;
            width = gridToCopy.width;
            height = gridToCopy.height;

            grid = gridToCopy.grid;
        }

        public void setValue(int x, int y, int val)
        {
            validateBounds(x, y);
            validateGridValue(val);

            grid[x, y] = val;
        }

        public int getValue(int x, int y)
        {
            validateBounds(x, y);

            return grid[x, y];
        }

        public int safeGetValue(int x, int y)
        {
            try
            {
                validateBounds(x, y);

                return grid[x, y];
            }
            catch (IndexOutOfRangeException e) { }

            return 0;
        }

        public int[,] getData()
        {
            return grid;
        }

        public void setData(int[,] data)
        {
            if (data.GetLength(0) == grid.GetLength(0) && data.GetLength(1) == grid.GetLength(1))
            {
                grid = data;
                return;
            }
            throw new IndexOutOfRangeException("Data was an invalid shape");
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
            if (val >= min && val <= max)
            {
                return;
            }
            throw new InvalidOperationException("The provided value cannot be set on the monogrid.");
        }
    }
}
