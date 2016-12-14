using EntityComponent;
using EntityComponent.Manager;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace EvershockGame.Code
{
    [Serializable]
    public class AttributesComponent : Component, IInputReceiver
    {
        int m_MaxHealth;
        int m_CurrentHealth;
        int m_transmittableHealth;

        float m_BaseMovementSpeed;
        public float movementSpeed { get; private set; }

        private List<float> m_MovementFactors = new List<float>();
        private List<float> m_MovementBoni = new List<float>();

        //---------------------------------------------------------------------------

        public AttributesComponent(Guid entity) : base(entity)
        {
            m_MaxHealth = 500;
            m_BaseMovementSpeed = 1.0f;

            m_CurrentHealth = m_MaxHealth;
            m_transmittableHealth = m_MaxHealth;

            CalculateMovementSpeed();
        }

        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(int max_health)
        {
            m_MaxHealth = max_health;
        }

        public void Init(int max_health, int transmittable_health = 0)
        {
            m_MaxHealth = max_health;
            m_transmittableHealth = transmittable_health;
        }

        public void Init(int max_health, float base_movement_speed)
        {
            m_MaxHealth = max_health;
            m_BaseMovementSpeed = base_movement_speed;
            CalculateMovementSpeed();
        }

        /*--------------------------------------------------------------------------
                    Manipulate Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(int damage_dealt)
        {
            m_CurrentHealth -= damage_dealt;

            if (m_CurrentHealth <= 0)
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

        /*--------------------------------------------------------------------------
                    Manipulate Movement
        --------------------------------------------------------------------------*/

        public void TransmitMovementFactor (float factor)
        {
            m_MovementFactors.Add(factor);

            CalculateMovementSpeed();
        }

        public void TransmitMovementAddend(float addend)
        {
            m_MovementBoni.Add(addend);

            CalculateMovementSpeed();
        }

        void CalculateMovementSpeed()
        {
            float m_MovementFactorsCombined = 1.0f;
            float m_MovementBoniCombined = 0.0f;

            if (m_MovementFactors != null)
            {
                for (int j = 0; j < m_MovementFactors.Count; j++)
                {
                    m_MovementFactorsCombined += m_MovementFactors[j] - 1;
                }
            }

            if (m_MovementBoni != null)
            {
                for (int i = 0; i < m_MovementBoni.Count; i++)
                {
                    m_MovementBoniCombined += m_MovementBoni[i];
                }
            }

            movementSpeed = m_MovementFactorsCombined * (m_BaseMovementSpeed + m_MovementBoniCombined);
        }

        /*--------------------------------------------------------------------------
                    Input
        --------------------------------------------------------------------------*/

        bool IsMovementInput(GameActionCollection actions)
        {
            return (actions[EGameAction.MOVE_DOWN] > 0.0f) ||
                   (actions[EGameAction.MOVE_UP] > 0.0f) ||
                   (actions[EGameAction.MOVE_LEFT] > 0.0f) ||
                   (actions[EGameAction.MOVE_RIGHT] > 0.0f);
        }

        public void ReceiveInput(GameActionCollection actions, float deltaTime)
        {

        }
    }
}
