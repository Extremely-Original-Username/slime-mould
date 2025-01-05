using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.rendering
{
    public interface ISlimeMouldRenderer
    {
        public void generateFrames(Action<int> onStep, Action onComplete);
        public void saveVideo(Action onSave);
    }
}
