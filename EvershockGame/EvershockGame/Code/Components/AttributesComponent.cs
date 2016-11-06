using EntityComponent;
using EntityComponent.Components;
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
        private int m_MaxHealth;
        private byte m_MaxInventorySlots;

        private float m_BaseMovementSpeed;
        private float m_BaseDamage;
        private float m_BaseArmor;

        private int m_CurrentHealth;
        private float m_CurrentMovementSpeed;


        public int Health
        {
            get { return m_CurrentHealth; }
            set { m_CurrentHealth = MathHelper.Clamp(value, 0, m_MaxHealth); }
        }

        public AttributesComponent(Guid entity) : base(entity)
        {
            m_MaxHealth = 500;
            m_BaseMovementSpeed = 1.0f;
            m_MaxInventorySlots = 0;

            Health = 50;

            // Collision test
            ICollider collider = GetComponent<ColliderComponent>();
            if (collider != null)
            {
                collider.Enter += (source, target) =>
                {
                    Console.WriteLine("Test");
                };
            }
        }

        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(int max_health)
        {
            m_MaxHealth = max_health;
        }

        public void Init(int max_health, float movement_speed)
        {
            m_MaxHealth = max_health;
            m_BaseMovementSpeed = movement_speed;
        }

        /*--------------------------------------------------------------------------
                    Change Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(int damage_dealt)
        {
            Health -= damage_dealt;

            if (Health == 0)
            {
                //TODO: Despawn character
            }
        }

        public void ReplenishHealth(int health_gained)
        {
            Health += health_gained;
        }

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            if (actions[EGameAction.ADD_HEALTH] > 0.0f)
            {
                Console.WriteLine("TEST");
            }
        }
    }
}
