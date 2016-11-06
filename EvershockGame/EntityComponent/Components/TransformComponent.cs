using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EntityComponent.Components
{
    public delegate void LocationChangedEventHandler(Vector3 oldLocation, Vector3 newLocation);
    public delegate void ScaleChangedEventHandler(Vector2 oldScale, Vector2 newScale);
    public delegate void RotationChangedEventHandler(float oldRotation, float newRotation);

    //---------------------------------------------------------------------------

    public class TransformComponent : Component
    {
        public Vector3 Location { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }

        public event LocationChangedEventHandler LocationChanged;
        public event ScaleChangedEventHandler ScaleChanged;
        public event RotationChangedEventHandler RotationChanged;

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
            Vector3 oldLocation = Location;
            Location = location;
            OnLocationChanged(oldLocation, Location);

            Vector2 oldScale = Scale;
            Scale = scale;
            OnScaleChanged(oldScale, Scale);

            float oldRotation = Rotation;
            Rotation = rotation;
            OnRotationChanged(oldRotation, Rotation);
        }

        //---------------------------------------------------------------------------

        public void MoveTo(Vector3 location)
        {
            Vector3 oldLocation = Location;
            Location = location;
            OnLocationChanged(oldLocation, Location);
        }

        //---------------------------------------------------------------------------

        public void MoveBy(Vector3 delta)
        {
            Vector3 oldLocation = Location;
            Location += delta;
            OnLocationChanged(oldLocation, Location);
        }

        //---------------------------------------------------------------------------

        public void RotateTo(float rotation)
        {
            float oldRotation = Rotation;
            Rotation = rotation;
            OnRotationChanged(oldRotation, Rotation);
        }

        //---------------------------------------------------------------------------

        public void RotateBy(float delta)
        {
            float oldRotation = Rotation;
            Rotation += delta;
            OnRotationChanged(oldRotation, Rotation);
        }

        //---------------------------------------------------------------------------

        private void OnLocationChanged(Vector3 oldLocation, Vector3 newLocation)
        {
            LocationChanged?.Invoke(oldLocation, newLocation);
        }

        //---------------------------------------------------------------------------

        private void OnScaleChanged(Vector2 oldScale, Vector2 newScale)
        {
            ScaleChanged?.Invoke(oldScale, newScale);
        }

        //---------------------------------------------------------------------------

        private void OnRotationChanged(float oldRotation, float newRotation)
        {
            RotationChanged?.Invoke(oldRotation, newRotation);
        }
    }
}
