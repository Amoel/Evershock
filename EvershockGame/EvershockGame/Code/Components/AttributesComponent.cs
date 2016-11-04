using EntityComponent;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    [Serializable]
    public class AttributesComponent : Component, IInputReceiver
    {
        private int MaxHealth;
        private float Movement_speed;
        private int health;

        public int Health
        {
            get { return health; }
            set
            {
                health = MathHelper.Clamp(value, 0, MaxHealth);
                Console.WriteLine(health);
            }
        }

        public AttributesComponent(Guid entity) : base(entity)
        {
            Health = 50;
            MaxHealth = 500;
            Movement_speed = 1.0f;
        }

        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(int max_health)
        {
            MaxHealth = max_health;
        }

        public void Init(int max_health, float movement_speed)
        {
            MaxHealth = max_health;
            Movement_speed = movement_speed;
        }

        /*--------------------------------------------------------------------------
                    Change Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(int damage_dealt)
        {
            Health -= damage_dealt;

            if (Health <= 0)
            {
                //Despawn character
            }
        }

        public void ReplenishHealth(int health_gained)
        {
            Health += health_gained;
        }

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection actions)
        {
            if (actions[EGameAction.ADD_HEALTH] > 0.0f)
            {
                Console.WriteLine("TEST");
            }
        }
    }
}
