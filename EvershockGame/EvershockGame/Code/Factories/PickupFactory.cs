using System;
using EntityComponent.Components;
using EntityComponent.Factory;
using EvershockGame.Code.Components;
using Microsoft.Xna.Framework;

namespace EvershockGame.Code.Factories
{
    public enum EPickups : byte
    {
        HEALTH,
        MOVEMENTSPEED,
        COINS,
        ITEM1,
        ITEM2,
        ITEM3,
        OTHER
    }

    public class PickupFactory
    {
        public static Pickup Create(EPickups pickupType, Vector3 location)
        {
            Pickup pickup = EntityFactory.Create<Pickup>(string.Format("{0}Pickup", pickupType.ToString()));
            pickup.AddComponent<TransformComponent>().Init(location);
            pickup.AddComponent<SpriteComponent>().Init(SpriteComponent.DefaultTexture);
            pickup.AddComponent<PhysicsComponent>();
            pickup.AddComponent<CircleColliderComponent>().Init(16);
            pickup.AddComponent<DespawnComponent>();

            IPickupComponent pickupComponent = null;

            switch (pickupType)
            {
                case EPickups.COINS:
                {

                    break;
                }

                case EPickups.HEALTH:
                    {
                        pickupComponent = pickup.AddComponent<HealthPickupComponent>();
                        break;
                    }

                default:
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
