﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EvershockGame.Manager;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using EvershockGame.Code.Manager;

namespace EvershockGame.Code.Components
{
    public class WallColliderComponent : ColliderComponent
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        //---------------------------------------------------------------------------

        public WallColliderComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Init(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            //Body = BodyFactory.CreateEdge(PhysicsManager.Get().World, Start / Unit, End / Unit, Entity);
            //foreach (Fixture fix in Body.FixtureList)
            //{
            //    fix.UserData = Entity;
            //}
            //Body.BodyType = BodyType.Static;
            //Body.IgnoreGravity = true;

            TransformComponent transform = GetComponent<TransformComponent>();
            //if (transform != null)
            //{
            //    Body.Position = (transform.Location.To2D() + Offset) / Unit;
            //}

            PhysicsComponent physics = GetComponent<PhysicsComponent>();
            if (physics != null)
            {
                Fixture fixture = FixtureFactory.AttachEdge(start / Unit, end / Unit, physics.Body, Entity);
                fixture.OnCollision += OnCollision;
                fixture.OnSeparation += OnSeparation;
            }

            //Body.OnCollision += OnCollision;
            //Body.OnSeparation += OnSeparation;
        }

        //---------------------------------------------------------------------------

        public override void Draw(SpriteBatch batch, CameraData data, float deltaTime)
        {
            if (CollisionManager.Get().IsDebugViewActive)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                Texture2D tex = CollisionManager.Get().PointTexture;
                if (transform != null && tex != null)
                {
                    Vector2 position = Start.ToLocal(data);
                    float length = Vector2.Distance(Start, End);
                    float angle = (float)Math.Atan2(End.Y - Start.Y, End.X - Start.X);
                    batch.Draw(tex, new Rectangle((int)(position.X), (int)(position.Y), (int)length, 2), tex.Bounds, GetDebugColor(), angle, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
