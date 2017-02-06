using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EvershockGame.Components
{
    [RequireComponent(typeof(TransformComponent))]
    public class EngineComponent : Component, ITickableComponent
    {
        private List<Keyframe> m_Keyframes;
        private int m_Index;
        private float m_Time;

        public bool Loop { get; set; }

        //---------------------------------------------------------------------------

        public EngineComponent(Guid entity) : base(entity)
        {
            m_Keyframes = new List<Keyframe>();
        }

        //---------------------------------------------------------------------------

        public void Init(bool loop)
        {
            Loop = loop;
        }

        //---------------------------------------------------------------------------

        public void AddKeyframe(Keyframe frame)
        {
            m_Keyframes.Add(frame);
        }

        //---------------------------------------------------------------------------

        public void AddKeyframe(Vector3 location, float rotation, float time)
        {
            m_Keyframes.Add(new Keyframe(location, rotation, time));
        }

        //---------------------------------------------------------------------------

        public void AddKeyframe(Vector3 location, Vector3 bezierIn, Vector3 bezierOut, float rotation, float time)
        {
            m_Keyframes.Add(new Keyframe(location, bezierIn, bezierOut, rotation, time));
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            m_Time += deltaTime;

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Keyframe frame = Interpolate(m_Keyframes[m_Index], m_Keyframes[(m_Index + 1) % m_Keyframes.Count]);

                transform.MoveTo(frame.Location);
                transform.RotateTo(frame.Rotation);
            }        
            
            if (m_Time >= m_Keyframes[(m_Index + 1) % m_Keyframes.Count].Time)
            {
                if (m_Index == m_Keyframes.Count - 1)
                {
                    m_Time -= m_Keyframes[m_Index].Time;
                    m_Index = 0;
                }
                else
                {
                    m_Index++;
                }
            }           
        }

        //---------------------------------------------------------------------------

        private Keyframe Interpolate(Keyframe first, Keyframe second)
        {
            Keyframe frame = new Keyframe();

            float delta = (m_Time - first.Time) / Math.Abs(second.Time - first.Time);

            //float x = MathHelper.Lerp(first.Location.X, second.Location.X, delta);
            //float y = MathHelper.Lerp(first.Location.Y, second.Location.Y, delta);
            //float z = MathHelper.Lerp(first.Location.Z, second.Location.Z, delta);
            //frame.Location = new Vector3(x, y, z);

            frame.Location = CalculateBezierPoint(first.Location, first.Location + first.BezierOut, second.Location, second.Location + second.BezierIn, delta);

            frame.Rotation = MathHelper.Lerp(first.Rotation, second.Rotation, delta);

            return frame;
        }

        //---------------------------------------------------------------------------

        private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float delta)
        {
            float u = 1 - delta;
            float tt = delta * delta;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * delta;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * delta * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }

    //---------------------------------------------------------------------------

    public class Keyframe
    {
        public Vector3 Location { get; set; }
        public Vector3 BezierIn { get; set; }
        public Vector3 BezierOut { get; set; }

        public float Rotation { get; set; }
        public float Time { get; set; }

        //---------------------------------------------------------------------------

        public Keyframe() { }

        //---------------------------------------------------------------------------

        public Keyframe(Vector3 location, float rotation, float time)
        {
            Location = location;
            Rotation = rotation;
            Time = time;
        }

        //---------------------------------------------------------------------------

        public Keyframe(Vector3 location, Vector3 bezierIn, Vector3 bezierOut, float rotation, float time)
        {
            Location = location;
            BezierIn = bezierIn;
            BezierOut = bezierOut;
            Rotation = rotation;
            Time = time;
        }
    }
}
