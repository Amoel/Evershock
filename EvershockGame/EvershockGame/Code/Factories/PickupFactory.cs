using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Items;
using EntityComponent.Manager;
using EntityComponent.Particles;
using EvershockGame.Code.Components;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
            physics.Init(0.94f, 4.0f, 0.3f);

            CircleColliderComponent collider = pickup.AddComponent<CircleColliderComponent>();
            collider.Init(8, BodyType.Dynamic);
            collider.SetCollidesWith(ECollisionCategory.Player);
            collider.SetCollisionCategory(ECollisionCategory.Pickup);
            collider.SetSensor(true);

            physics.ApplyForce(force, true);

            pickup.AddComponent<DespawnComponent>();
            return pickup;
        }
    }
}
