using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EvershockGame.Code.Components;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code.Factories
{
    public enum EPickups : byte
    {
        HEALTH,
        MANA,
        COIN
    }

    public class PickupFactory
    {
        public static Pickup Create(EPickups pickupType, Vector3 location, Vector3 force)
        {
            Pickup pickup = EntityFactory.Create<Pickup>(string.Format("{0}Pickup", pickupType.ToString()));
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

            IPickupComponent pickupComponent = null;
            SpriteComponent sprite = pickup.AddComponent<SpriteComponent>();
            ShadowComponent shadow = pickup.AddComponent<ShadowComponent>();
            LightingComponent light = pickup.AddComponent<LightingComponent>();

            switch (pickupType)
            {
                case EPickups.HEALTH:
                    pickupComponent = pickup.AddComponent<HealthPickupComponent>();
                    sprite.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 1, 1), Vector2.Zero, Vector2.One * 2);
                    shadow.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 1, 1), Vector2.One * 2, new Vector2(0, 3));
                    light.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 1, 1), Vector2.Zero, Vector2.One * 2, Color.Orange, 0.5f);
                    break;
                
                case EPickups.MANA:
                    pickupComponent = pickup.AddComponent<ManaPickupComponent>();
                    sprite.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 6, 0), Vector2.Zero, Vector2.One * 2);
                    shadow.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 6, 0), Vector2.One * 2, new Vector2(0, 3));
                    light.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 6, 0), Vector2.Zero, Vector2.One * 2, Color.Purple, 0.5f);
                    break;

                case EPickups.COIN:
                    pickupComponent = pickup.AddComponent<CoinPickupComponent>();
                    sprite.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 0, 1), Vector2.Zero, Vector2.One * 2);
                    shadow.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 0, 1), Vector2.One * 2, new Vector2(0, 3));
                    light.Init(new Sprite(AssetManager.Get().Find<Texture2D>(ETilesetAssets.Items), 14, 30, 0, 1), Vector2.Zero, Vector2.One * 2, Color.Red, 0.5f);
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
    }
}
