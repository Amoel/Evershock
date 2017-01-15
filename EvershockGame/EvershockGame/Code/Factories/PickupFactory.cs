
using EntityComponent;
using EntityComponent.Factory;
using System;

namespace EvershockGame.Code.Factories
{
    class PickupFactory
    {
        public static Pickup Create(EPickups pickupType)
        {
            Pickup pickup = EntityFactory.Create<Pickup>(string.Format("{0}Pickup", pickupType.ToString()));

            switch (pickupType)
            {
                case EPickups.COINS:
                {
                    break;
                }

                default:
                    break;
            }

            return pickup;
        }
    }
}
