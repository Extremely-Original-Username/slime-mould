using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.slimeMould
{
    public interface ISlimeMould
    {
        public SlimeMouldParams Parameters { get; }

        public void step();
        public MonoGrid getState();
    }
}
