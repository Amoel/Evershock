using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Misc
{
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private Queue<float> m_SampleBuffer;

        //---------------------------------------------------------------------------

        public FrameCounter()
        {
            m_SampleBuffer = new Queue<float>();
        }

        //---------------------------------------------------------------------------

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            m_SampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (m_SampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                m_SampleBuffer.Dequeue();
                AverageFramesPerSecond = m_SampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }
    }
}
