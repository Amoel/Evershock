﻿using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using EvershockGame.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code
{
    public class Chest : Entity
    {
        bool closed = true;

        public Chest(string name) : base(name)
        {
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AddComponent<PickupSpawnerComponent>();

            AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("ChestClosed1"), Vector2.Zero, new Vector2(2, 2));
            //AddComponent<ShadowComponent>().Init(AssetManager.Get().Find<Texture2D>("ChestClosed1"), new Vector2(2, 2), new Vector2(0, 3));
            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(2, 2), Color.White, 0.8f);

            RectColliderComponent chestCollider = AddComponent<RectColliderComponent>();
            chestCollider.Init(AssetManager.Get().Find<Texture2D>("ChestClosed1").Width, AssetManager.Get().Find<Texture2D>("ChestClosed1").Height);

            chestCollider.Enter += (source, target) =>
            {
                if (closed)
                {
                    if (source.HasComponent<SpriteComponent>())
                    {
                        source.GetComponent<SpriteComponent>().Texture = AssetManager.Get().Find<Texture2D>("ChestOpened1");
                    }

                    if (source.HasComponent<PickupSpawnerComponent>())
                    {
                        GetComponent<PickupSpawnerComponent>().Spawn();
                    }

                    closed = false;
                }
            };

            //AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"));
        }

        //---------------------------------------------------------------------------

        public void Init(Vector2 location)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                //transform.Init(location.To3D());
                transform.MoveTo(location.To3D());

                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (physics != null)
                {
                    physics.ResetLocation();
                }
            }
        }
        
    }
}
