using EvershockGame.Code.Components;
using EvershockGame.Components;
using EvershockGame.Code.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using EvershockGame.Code.Stages;

namespace EvershockGame.Code
{
    public static class Extensions
    {
        public static Vector2 ToLocal(this Vector2 source, CameraData data)
        {
            return source - data.Center + new Vector2(data.Width / 2.0f, data.Height / 2.0f);
        }

        //---------------------------------------------------------------------------

        public static Vector3 ToLocal(this Vector3 source, CameraData data)
        {
            return new Vector3(source.X - data.Center.X + data.Width / 2.0f, source.Y - data.Center.Y + data.Height / 2.0f, source.Z);
        }

        //---------------------------------------------------------------------------

        public static Vector2 ToLocal2D(this Vector3 source, CameraData data)
        {
            //return new Vector2(source.X, source.Y - source.Z / 2.0f) - data.Center + new Vector2(data.Width / 2.0f, data.Height / 2.0f);
            return new Vector2((int)(source.X + data.Width / 2 - data.Center.X), (int)(source.Y + data.Height / 2 - data.Center.Y - source.Z / 2.0f));
        }

        //---------------------------------------------------------------------------

        public static Vector2 ToLocal2DShadow(this Vector3 source, CameraData data)
        {
            //return new Vector2(source.X, source.Y + source.Z / 7.0f + 5.0f) - data.Center + new Vector2(data.Width / 2.0f, data.Height / 2.0f);
            return new Vector2((int)(source.X + data.Width / 2 - data.Center.X), (int)(source.Y + data.Height / 2 - data.Center.Y + source.Z / 7.0f + 5.0f));
        }

        //---------------------------------------------------------------------------

        public static Vector3 ToLocal3D(this Vector2 source, CameraData data)
        {
            return new Vector3(source.X - data.Center.X + data.Width / 2.0f, source.Y - data.Center.Y + data.Height / 2.0f, 0);
        }

        //---------------------------------------------------------------------------

        public static Vector2 To2D(this Vector3 source)
        {
            return new Vector2(source.X, source.Y);
        }

        //---------------------------------------------------------------------------

        public static Vector3 To3D(this Vector2 source)
        {
            return new Vector3(source.X, source.Y, 0);
        }

        //---------------------------------------------------------------------------

        public static Vector2 ToLocalUV(this Vector2 source, CameraData data)
        {
            Vector2 temp = source.ToLocal(data);
            return new Vector2(temp.X / (data.Width / 2.0f) - 1.0f, temp.Y / (data.Height / 2.0f) - 1.0f);
        }

        //---------------------------------------------------------------------------

        public static Vector3 Reflect(this Vector3 source, Vector3 normal)
        {
            Vector3 normalizedNormal = Vector3.Normalize(normal);
            return source - 2 * Vector3.Dot(source, normalizedNormal) * normalizedNormal;
        }

        //---------------------------------------------------------------------------

        public static Vector3 Reflect2(this Vector3 source, Vector3 normal)
        {
            Vector3 normalizedNormal = Vector3.Normalize(normal);
            float angle = Vector3.Dot(source, -normalizedNormal);
            return source + (normalizedNormal * (source * normalizedNormal).Length());
            //return source - (Vector3.Negate(Vector3.Normalize(normal)) * (source * Vector3.Normalize(normal)).Length());
        }

        //---------------------------------------------------------------------------

        public static EDirection Invert(this EDirection direction)
        {
            switch (direction)
            {
                case EDirection.Left: return EDirection.Right;
                case EDirection.Right: return EDirection.Left;
                case EDirection.Up: return EDirection.Down;
                case EDirection.Down: return EDirection.Up;
                default: return EDirection.None;
            }
        }
    }
}
