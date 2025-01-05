using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.interfaces
{
    public interface ISlimeMould
    {
        public int Width { get; }
        public int Height { get; }

        public void step();
        public MonoGrid getState();
    }
}
