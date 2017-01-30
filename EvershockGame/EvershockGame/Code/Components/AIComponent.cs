using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using EntityComponent.Pathfinding;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    [RequireComponent(typeof(PhysicsComponent))]
    public class AIComponent : Component, ITickableComponent, IDrawableComponent
    {
        private Pathfinder m_Pathfinder;

        private float m_Timer;
        private Guid m_Target;
        private EBehaviour m_Behaviour;

        private List<Vector3> m_Path;

        //---------------------------------------------------------------------------

        public AIComponent(Guid entity) : base(entity)
        {
            Area area = AreaManager.Get().FindAreaFromEntity(Entity);
            m_Pathfinder = new Pathfinder();
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            if (m_Target == Guid.Empty)
            {
                List<Player> players = EntityManager.Get().Find<Player>();
                if (players.Count > 0)
                {
                    AddTarget(players[0], EBehaviour.Follow);
                }
            }

            m_Timer += deltaTime;
            if (m_Timer >= 0.2f)
            {
                m_Timer -= 0.2f;
                TickPathfinding();
            }
                
            if (m_Path != null && m_Path.Count > 0)
            {

                TransformComponent transform = GetComponent<TransformComponent>();
                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (transform != null && physics != null)
                {
                    Vector3 step = Vector3.Normalize(m_Path[0] - transform.Location) * 400;
                    physics.ApplyForce(step);
                }
            }
        }

        //---------------------------------------------------------------------------

        private void TickPathfinding()
        {
            IEntity target = EntityManager.Get().Find(m_Target);
            if (target != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                TransformComponent targetTransform = target.GetComponent<TransformComponent>();

                if (transform != null && targetTransform != null)
                {
                    m_Path = m_Pathfinder.ExecuteSearch(transform.Location, targetTransform.Location, m_Behaviour);

                    hasLineOfSight = true;
                    while (m_Path.Count > 1 && hasLineOfSight)
                    {
                        PhysicsManager.Get().World.RayCast(RaycastCallback, transform.Location.To2D() / ColliderComponent.Unit, m_Path[1].To2D() / ColliderComponent.Unit);
                        if (hasLineOfSight)
                        {
                            m_Path.RemoveAt(0);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        bool hasLineOfSight = true;
        private float RaycastCallback(Fixture fix, Vector2 hit, Vector2 normal, float fraction)
        {
            hasLineOfSight = false;
            return fraction;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                Texture2D tex = CollisionManager.Get().PointTexture;
                TransformComponent transform = GetComponent<TransformComponent>();
                if (tex != null && transform != null && m_Path != null)
                {
                    for (int i = 0; i < m_Path.Count; i++)
                    {
                        Vector2 start = (i == 0 ? transform.Location.ToLocal2D(data) : m_Path[i - 1].ToLocal2D(data));
                        Vector2 end = m_Path[i].ToLocal2D(data);

                        float length = Vector2.Distance(start, end);
                        float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
                        batch.Draw(tex, new Rectangle((int)(start.X), (int)(start.Y), (int)length, 2), tex.Bounds, Color.Yellow * (1.0f - (float)i / m_Path.Count), angle, Vector2.Zero, SpriteEffects.None, 1.0f);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void AddTarget(IEntity target, EBehaviour behaviour)
        {
            m_Target = target.GUID;
            m_Behaviour = behaviour;
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
