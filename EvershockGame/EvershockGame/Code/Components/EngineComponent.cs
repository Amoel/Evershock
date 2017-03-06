using EvershockGame.Code;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EvershockGame.Code.Components
{
    [RequireComponent(typeof(TransformComponent))]
    public class EngineComponent : Component, ITickableComponent
    {
        private Dictionary<int, EnginePath> m_Paths;
        public int Index { get; private set; }
        public EAnimationState State { get; private set; }

        //---------------------------------------------------------------------------

        public EngineComponent(Guid entity) : base(entity)
        {
            m_Paths = new Dictionary<int, EnginePath>();
            State = EAnimationState.Stopped;
        }

        //---------------------------------------------------------------------------

        public EnginePath AddPath(int index, bool loop)
        {
            EnginePath path = new EnginePath(loop);
            m_Paths.Add(index, path);
            return path;
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            switch (State)
            {
                case EAnimationState.Playing:
                    TransformComponent transform = GetComponent<TransformComponent>();
                    PhysicsComponent physics = GetComponentInAncestor<PhysicsComponent>();

                    if (transform != null)
                    {
                        if (physics != null)
                        {
                            Keyframe frame;
                            if (m_Paths[Index].GetNextKeyFrame(deltaTime, out frame))
                            {

                                physics.ApplyAbsoluteForce(frame.Location - transform.Location);
                                //physics.RotateTo(frame.Rotation);
                            }
                            else
                            {
                                Stop();
                            }
                        }
                        else
                        {
                            Keyframe frame;
                            if (m_Paths[Index].GetNextKeyFrame(deltaTime, out frame))
                            {
                                transform.MoveTo(frame.Location);
                                transform.RotateTo(frame.Rotation);
                            }
                            else
                            {
                                Stop();
                            }
                        }
                    }
                    break;
            }
        }

        //---------------------------------------------------------------------------

        public void SetCurrentPath(int index)
        {
            Index = index;
            m_Paths[Index].Reset();

            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                Keyframe frame;
                m_Paths[Index].GetNextKeyFrame(0.0f, out frame);

                transform.MoveTo(frame.Location);
                transform.RotateTo(frame.Rotation);
            }
        }

        //---------------------------------------------------------------------------

        public void Play()
        {
            State = EAnimationState.Playing;
        }

        //---------------------------------------------------------------------------

        public void Play(int index)
        {
            SetCurrentPath(index);
            State = EAnimationState.Playing;
        }

        //---------------------------------------------------------------------------

        public void Pause()
        {
            State = EAnimationState.Paused;
        }

        //---------------------------------------------------------------------------

        public void Stop()
        {
            m_Paths[Index].Reset();
            State = EAnimationState.Stopped;
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }

    //---------------------------------------------------------------------------

    public class EnginePath
    {
        public bool Loop { get; set; }

        private List<Keyframe> m_Keyframes;
        private int m_Index;
        private float m_Time;

        //---------------------------------------------------------------------------

        public EnginePath(bool loop)
        {
            Loop = loop;
            m_Keyframes = new List<Keyframe>();
        }

        //---------------------------------------------------------------------------

        public void Reset()
        {
            m_Time = 0.0f;
            m_Index = 0;
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

        public bool GetNextKeyFrame(float deltaTime, out Keyframe frame)
        {
            bool hasEnded = false;
            m_Time += deltaTime;
            if (m_Time >= m_Keyframes[(m_Index + 1) % m_Keyframes.Count].Time)
            {
                if (m_Index == m_Keyframes.Count - 1)
                {
                    m_Time -= m_Keyframes[m_Index].Time;
                    m_Index = 0;
                    hasEnded = true;
                }
                else
                {
                    m_Index++;
                }
            }
            frame = Interpolate(m_Keyframes[m_Index], m_Keyframes[(m_Index + 1) % m_Keyframes.Count]);
            return (Loop || !hasEnded);
        }

        //---------------------------------------------------------------------------

        private Keyframe Interpolate(Keyframe first, Keyframe second)
        {
            Keyframe frame = new Keyframe();

            float delta = (m_Time - first.Time) / Math.Abs(second.Time - first.Time);
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
    }

    //---------------------------------------------------------------------------

    public struct Keyframe
    {
        public Vector3 Location { get; set; }
        public Vector3 BezierIn { get; set; }
        public Vector3 BezierOut { get; set; }

        public float Rotation { get; set; }
        public float Time { get; set; }

        //---------------------------------------------------------------------------

        public Keyframe(Vector3 location, float rotation, float time) : this()
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
