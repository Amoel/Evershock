﻿using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
using EvershockGame.Code.Particles;
using EvershockGame.Code.Components;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using EvershockGame.Code.Manager;
using EvershockGame.Code.Factory;
using EvershockGame.Code.Items;

namespace EvershockGame.Code.Factories
{
    public enum EPickups : byte
    {
        Health,
        Mana,
        Coin,
        Item
    }

    public class PickupFactory
    {
        private static Dictionary<EPickups, Sprite> m_Sprites = new Dictionary<EPickups, Sprite>()
        {
            { EPickups.Health, new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 7, 1) },
            { EPickups.Mana, new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 1, 1) },
            { EPickups.Coin, new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 8, 23) }
        };

        //---------------------------------------------------------------------------

        public static Pickup Create(EPickups pickupType, Vector3 location, Vector3 force)
        {
            Pickup pickup = Create(pickupType.ToString(), location, force);
            
            IPickupComponent pickupComponent = null;

            Sprite sprite = m_Sprites[pickupType];
            pickup.AddComponent<SpriteComponent>().Init(sprite, Vector2.Zero, Vector2.One * 2);
            pickup.AddComponent<ShadowComponent>().Init(sprite, Vector2.One * 2, new Vector2(0, 3));
            pickup.AddComponent<LightingComponent>().Init(sprite, Vector2.Zero, Vector2.One * 2, Color.White, 0.5f);

            switch (pickupType)
            {
                case EPickups.Health:
                    pickupComponent = pickup.AddComponent<HealthPickupComponent>();
                    break;
                case EPickups.Mana:
                    pickupComponent = pickup.AddComponent<ManaPickupComponent>();
                    break;
                case EPickups.Coin:
                    pickupComponent = pickup.AddComponent<CoinPickupComponent>();
                    break;
                default:
                    AssertManager.Get().Show("Value used by 'Create' does not fit range of EPickups");
                    break;

            }

            if (pickupComponent != null)
            {
                pickup.GetComponent<CircleColliderComponent>().Enter += (source, target) =>
                {
                    pickupComponent.OnPickup(target);
                };
            }
            
            return pickup;
        }

        //---------------------------------------------------------------------------

        public static Pickup Create(EItemType itemType, Vector3 location, Vector3 force)
        {
            Pickup pickup = Create(itemType.ToString(), location, force);

            Sprite sprite = ItemManager.Get().Find(itemType).Sprite;
            pickup.AddComponent<SpriteComponent>().Init(sprite, Vector2.Zero, Vector2.One * 2);
            pickup.AddComponent<ShadowComponent>().Init(sprite, Vector2.One * 2, new Vector2(0, 3));
            pickup.AddComponent<LightingComponent>().Init(sprite, Vector2.Zero, Vector2.One * 2, Color.White, 0.5f);

            ParticleSpawnerComponent particleSpawner = pickup.AddComponent<ParticleSpawnerComponent>();
            Color color = ItemManager.Get().GetColorByType(itemType);
            ParticleDesc desc = ParticleDesc.Stars;
            desc.ParticleColor = (time) => color;
            desc.LightColor = (time) => color;
            particleSpawner.Emitter = new CircleParticleEmitter(0, 30, desc)
            {
                Sprite = CollisionManager.Get().PointTexture,
                Light = CollisionManager.Get().PointTexture,
                SpawnRate = (time) => 20
            };
            pickup.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), Vector2.Zero, Vector2.One * 0.5f, Color.White, 0.8f);
            
            ItemPickupComponent pickupComponent = pickup.AddComponent<ItemPickupComponent>();
            if (pickupComponent != null)
            {
                pickupComponent.Type = itemType;
                pickup.GetComponent<CircleColliderComponent>().Enter += (source, target) =>
                {
                    pickupComponent.OnPickup(target);
                };
            }

            return pickup;
        }

        //---------------------------------------------------------------------------

        private static Pickup Create(string name, Vector3 location, Vector3 force)
        {
            Pickup pickup = EntityFactory.Create<Pickup>(string.Format("{0}Pickup", name));
            pickup.AddComponent<TransformComponent>().Init(location);

            PhysicsComponent physics = pickup.AddComponent<PhysicsComponent>();
            physics.Init(BodyType.Dynamic, 0.97f, 2.0f);

            CircleColliderComponent collider = pickup.AddComponent<CircleColliderComponent>();
            collider.Init(25, BodyType.Dynamic);
            collider.SetCollidesWith(ECollisionCategory.Stage);
            collider.SetCollisionCategory(ECollisionCategory.Pickup);
            collider.SetRestitution(0.3f);
            //collider.SetSensor(true);

            physics.ApplyForce(force, true);

            pickup.AddComponent<DespawnComponent>();
            return pickup;
        }

        //---------------------------------------------------------------------------

#if DEBUG
        public static Pickup Create(string name, int cameraIndex)
        {
            EPickups pickupType;
            EItemType itemType;
            if (Enum.TryParse(name, out pickupType))
            {
                List<Camera> cams = EntityManager.Get().Find<Camera>();
                if (cams.Count > cameraIndex)
                {
                    Vector3 location = cams[cameraIndex].Transform.Location;
                    return Create(pickupType, location, new Vector3(0, 0, 50));
                }
            }
            else if (Enum.TryParse(name, out itemType))
            {
                List<Camera> cams = EntityManager.Get().Find<Camera>();
                if (cams.Count > cameraIndex)
                {
                    Vector3 location = cams[cameraIndex].Transform.Location;
                    return Create(itemType, location, new Vector3(0, 0, 50));
                }
            }
            return null;
        }
#endif
    }
}
