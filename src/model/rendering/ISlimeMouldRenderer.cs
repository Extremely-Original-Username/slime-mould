using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.rendering
{
    public interface ISlimeMouldRenderer
    {
        public SlimeMouldrendererParams Parameters { get; }
        public void generateFrames(Action<int> beforeStep, Action whileAwaitingCompletion, Action onComplete);
        public void saveVideo(Action onSave);
    }
}
