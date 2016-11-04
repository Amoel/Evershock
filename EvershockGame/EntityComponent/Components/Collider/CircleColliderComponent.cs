using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
    public class CircleColliderComponent : Component, ICollider, IDrawableComponent
    {
        public EColliderType Type { get; set; }
        public EColliderMobility Mobility { get; set; }

        public float Radius { get; set; }
        public Vector2 Offset { get; set; }

        //---------------------------------------------------------------------------

        public CircleColliderComponent(Guid entity) : base(entity)
        {
            Radius = 26;
            Offset = Vector2.Zero;
            CollisionManager.Get().Register(Entity);

            Init(EColliderType.Solid, EColliderMobility.Static);
        }

        //---------------------------------------------------------------------------

        public void Init(EColliderType type, EColliderMobility mobility)
        {
            Type = type;
            Mobility = mobility;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().CircleTexture;
                if (transform != null && tex != null)
                {
                    Vector2 center = transform.Location.ToLocal2D(data);
                    batch.Draw(tex, new Rectangle((int)(center.X - Offset.X - Radius), (int)(center.Y - Offset.Y - Radius), (int)(Radius * 2), (int)(Radius * 2)), Color.White);
                }
            }
        }
    }
}
