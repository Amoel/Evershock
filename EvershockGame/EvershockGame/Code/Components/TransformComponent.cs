using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code.Components
{
    public delegate void LocationChangedEventHandler(Vector3 oldLocation, Vector3 newLocation);
    public delegate void ScaleChangedEventHandler(Vector2 oldScale, Vector2 newScale);
    public delegate void OrientationChangedEventHandler(Vector2 oldOrientation, Vector2 newOrientation);
    public delegate void RotationChangedEventHandler(float oldRotation, float newRotation);

    //---------------------------------------------------------------------------

    public class TransformComponent : Component
    {
        public Vector3 AbsoluteLocation { get { return FindAbsoluteLocation(); } }

        private Vector3 m_Location;
        public Vector3 Location
        {
            get { return m_Location; }
            set
            {
                if (!m_Location.Equals(value))
                {
                    m_Location = value;
                    OnPropertyChanged(m_Location);
                }
            }
        }

        public Vector2 Scale { get; set; }
        public Vector2 Orientation { get; set; }
        public float Rotation { get; set; }

        public event LocationChangedEventHandler LocationChanged;
        public event ScaleChangedEventHandler ScaleChanged;
        public event OrientationChangedEventHandler OrientationChanged;
        public event RotationChangedEventHandler RotationChanged;

        //---------------------------------------------------------------------------

        public TransformComponent(Guid entity) : base(entity)
        {
            Init(Vector3.Zero, Vector2.One, Vector2.UnitY, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location)
        {
            Init(location, Vector2.One, Vector2.UnitY, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location, Vector2 scale)
        {
            Init(location, scale, Vector2.UnitY, 0.0f);
        }

        //---------------------------------------------------------------------------

        public void Init(Vector3 location, Vector2 scale, Vector2 orientation, float rotation)
        {
            Vector3 oldLocation = Location;
            Location = location;
            OnLocationChanged(oldLocation, Location);

            Vector2 oldScale = Scale;
            Scale = scale;
            OnScaleChanged(oldScale, Scale);

            Vector2 oldOrientation = Orientation;
            Orientation = orientation;
            OnOrientationChanged(oldOrientation, Orientation);

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

        public void OrientateTo(Vector2 orientation)
        {
            Vector2 oldOrientation = Orientation;
            Orientation = Vector2.Normalize(orientation);
            OnOrientationChanged(oldOrientation, Orientation);
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

        private Vector3 FindAbsoluteLocation()
        {
            TransformComponent parentTransform = GetComponentInParent<TransformComponent>();
            if (parentTransform != null)
            {
                return Location + parentTransform.AbsoluteLocation;
            }
            return Location;
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
        
        private void OnOrientationChanged(Vector2 oldOrientation, Vector2 newOrientation)
        {
            OrientationChanged?.Invoke(oldOrientation, newOrientation);
        }

        //---------------------------------------------------------------------------

        private void OnRotationChanged(float oldRotation, float newRotation)
        {
            RotationChanged?.Invoke(oldRotation, newRotation);
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
