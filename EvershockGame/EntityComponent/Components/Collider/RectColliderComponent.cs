using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
    public class RectColliderComponent : Component, ICollider
    {
        public EColliderType Type { get; set; }
        public EColliderMobility Mobility { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Offset { get; set; }

        //---------------------------------------------------------------------------

        public RectColliderComponent(Guid entity) : base(entity)
        {
            CollisionManager.Get().Register(Entity);

            Init(EColliderType.Solid, EColliderMobility.Static);
        }

        //---------------------------------------------------------------------------

        public void Init(EColliderType type, EColliderMobility mobility)
        {
            Type = type;
            Mobility = mobility;
        }
    }
}
