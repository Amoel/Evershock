using Microsoft.Xna.Framework;
using System;

namespace EntityComponent.Components
{
    public class TransformComponent : Component
    {
        public Vector3 Location { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }

        //---------------------------------------------------------------------------

        public TransformComponent(Guid entity) : base(entity)
        {
            Init(Vector3.Zero, Vector2.One, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location)
        {
            Init(location, Vector2.One, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location, Vector2 scale)
        {
            Init(location, scale, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location, Vector2 scale, float rotation)
        {
            Location = location;
            Scale = scale;
            Rotation = rotation;
        }

        //---------------------------------------------------------------------------

        public void MoveTo(Vector3 location)
        {
            Location = location;
        }

        //---------------------------------------------------------------------------

        public void MoveBy(Vector3 delta)
        {
            Location += delta;
        }

        //---------------------------------------------------------------------------

        public void RotateTo(float rotation)
        {
            Rotation = rotation;
        }

        //---------------------------------------------------------------------------

        public void RotateBy(float delta)
        {
            Rotation += delta;
        }
    }
}
