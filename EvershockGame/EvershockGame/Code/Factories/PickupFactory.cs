using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EvershockGame.Code.Components;
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
        public static Pickup Create(EPickups pickupType, Vector3 location)
        {
            Pickup pickup = EntityFactory.Create<Pickup>(string.Format("{0}Pickup", pickupType.ToString()));
            pickup.AddComponent<TransformComponent>().Init(location);
            pickup.AddComponent<PhysicsComponent>();
            pickup.AddComponent<CircleColliderComponent>().Init(16);
            pickup.AddComponent<DespawnComponent>();

            IPickupComponent pickupComponent = null;

            switch (pickupType)
            {
                case EPickups.HEALTH:
                    pickupComponent = pickup.AddComponent<HealthPickupComponent>();
                    pickup.AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("RedOrb"));
                    break;
                

                case EPickups.MANA:
                    pickupComponent = pickup.AddComponent<ManaPickupComponent>();
                    pickup.AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("BlueOrb"));
                    break;

                case EPickups.COIN:
                    pickupComponent = pickup.AddComponent<CoinPickupComponent>();
                    pickup.AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("YellowOrb"));
                    break;

                default:
                    AssertManager.Get().Show("Value used by 'Create' does not fit range of EPickups");
                    break;

            }

            //---------------------------------------------------------------------------

            if (pickupComponent != null)
            {
                pickup.GetComponent<CircleColliderComponent>().Enter += (source, target) =>
                {
                    pickupComponent.OnPickup(target);
                };
            }

            //---------------------------------------------------------------------------

            return pickup;
        }
    }
}
