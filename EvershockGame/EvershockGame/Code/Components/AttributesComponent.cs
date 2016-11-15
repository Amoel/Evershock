using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using System;
using Microsoft.Xna.Framework;

namespace EvershockGame.Code
{
    [Serializable]
    public class AttributesComponent : Component, IInputReceiver
    {
        private int m_MaxHealth = 500;
        private int m_CurrentHealth;
        public int m_transmittableHealth;

        public float BaseMovementSpeed = 1.0f;

        public AttributesComponent(Guid entity) : base(entity)
        {
            m_MaxHealth = 500;
            BaseMovementSpeed = 1.0f;
            
            m_CurrentHealth = m_MaxHealth;
            m_transmittableHealth = m_MaxHealth;
    }

        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(int max_health)
        {
            m_MaxHealth = max_health;
        }

        public void Init(int max_health, int transmittable_health)
        {
            m_MaxHealth = max_health;
            m_transmittableHealth = transmittable_health;
        }

        public void Init(int max_health, float movement_speed)
        {
            m_MaxHealth = max_health;
            BaseMovementSpeed = movement_speed;
        }

        /*--------------------------------------------------------------------------
                    Change Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(int damage_dealt)
        {
            m_CurrentHealth -= damage_dealt;

            if (m_CurrentHealth == 0)
            {
                //TODO: Despawn character
            }
        }

        public void ReplenishHealth(int health_gained)
        {
            m_CurrentHealth += health_gained;

            if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            if (actions[EGameAction.ADD_HEALTH] > 0.0f)
            {
                Console.WriteLine(m_CurrentHealth);
            }
        }
    }
}
