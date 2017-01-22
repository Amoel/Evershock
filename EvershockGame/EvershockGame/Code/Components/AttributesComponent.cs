using EntityComponent;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;

namespace EvershockGame.Code
{
    [Serializable]
    public class AttributesComponent : Component, IInputReceiver, ITickableComponent
    {
        //TODO_lukas: Think about negative influences on Factors and Boni and re-evaluate formulas, if necessary (Health, Mana, MovementSpeed)

        //Health
        float m_MaxHealth;
        float m_CurrentHealth;
        float m_BaseHealthRegen;
        List<float> m_HealthRegenFactors = new List<float>();
        List<float> m_HealthRegenBoni = new List<float>();

        //Mana
        float m_MaxMana;
        float m_CurrentMana;
        float m_BaseManaRegen;
        List<float> m_ManaRegenFactors = new List<float>();
        List<float> m_ManaRegenBoni = new List<float>();

        //MovementSpeed
        float m_BaseMovementSpeed;
        List<float> m_MovementFactors = new List<float>();
        List<float> m_MovementBoni = new List<float>();

        //public read only handles
        public float HealthRegen { get; private set; }
        public float ManaRegen { get; private set; }
        public float MovementSpeed { get; private set; }

        //---------------------------------------------------------------------------

        public AttributesComponent(Guid entity) : base(entity)
        {
            m_MaxHealth = 500.0f;
            m_BaseHealthRegen = 0.0f;
            m_MaxMana = 0.0f;
            m_BaseManaRegen = 0.0f;
            m_BaseMovementSpeed = 100.0f;

            m_CurrentHealth = m_MaxHealth;
            m_CurrentMana = m_MaxMana;
            
            UpdateAttributes();
        }


        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(float max_health = 500, float max_mana = 0, float base_movement_speed = 100.0f, float base_health_regen = 0.0f, float base_mana_regen = 0.0f)
        {
            m_MaxHealth = max_health;
            m_MaxMana = max_mana;
            m_BaseMovementSpeed = base_movement_speed;
            m_BaseHealthRegen = base_health_regen;
            m_BaseManaRegen = base_mana_regen;

            UpdateAttributes();
        }


        /*--------------------------------------------------------------------------
                    Manipulate Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(float damage_dealt)
        {
            m_CurrentHealth -= damage_dealt;

            if (m_CurrentHealth <= 0)
            {
                //TODO_lukas: Despawn character
            }
        }

        //---------------------------------------------------------------------------
        
        public void ReplenishHealth(float health_gain)
        {
            m_CurrentHealth += health_gain;

            if (m_CurrentHealth > m_MaxHealth)
                m_CurrentHealth = m_MaxHealth;
        }

        //---------------------------------------------------------------------------

        /// <summary>
        ///Values above 0 increase Health Regeneration; Values below 0 decrease Health Regeneration;
        ///Standard Factor is 1.0f; If factor is negative, Health will be constantly drained.
        /// </summary>
        public void AddHealthRegenFactor(float factor)
        {
            m_HealthRegenFactors.Add(factor);

            UpdateHealthRegen();
        }

        //---------------------------------------------------------------------------

        public void AddHealthRegenBonus(float bonus)
        {
            m_HealthRegenBoni.Add(bonus);

            UpdateHealthRegen();
        }

        //TODO_lukas: Create 'Remove Health Regen' Factor/Bonus methods

        //---------------------------------------------------------------------------

        void UpdateHealthRegen()
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;

            if (m_HealthRegenFactors.Count != 0)
            {
                for (int j = 0; j < m_HealthRegenFactors.Count; j++)
                {
                    combinedFactors += m_HealthRegenFactors[j];
                }
            }

            if (m_HealthRegenBoni.Count != 0)
            {
                for (int i = 0; i < m_HealthRegenBoni.Count; i++)
                {
                    combinedBoni += m_HealthRegenBoni[i];
                }
            }

            HealthRegen = combinedFactors * (m_BaseHealthRegen + combinedBoni);
        }


        /*--------------------------------------------------------------------------
                     Manipulate Mana
        --------------------------------------------------------------------------*/

        public bool UseMana(float mana_needed)
        {
            if (m_CurrentMana >= mana_needed)
            {
                m_CurrentMana -= mana_needed;
                return true;
            }
            else
                return false;
        }

        //---------------------------------------------------------------------------

        public void ReplenishMana(float mana_gain)
        {
            m_CurrentMana += mana_gain;

            if (m_CurrentMana > m_MaxMana)
                m_CurrentMana = m_MaxMana;
        }

        //---------------------------------------------------------------------------
        
        /// <summary>
        ///Values above 0 increase Mana Regeneration; Values below 0 decrease Mana Regeneration.
        ///Standard Factor is 1.0f; If factor is negative, Mana will be constantly drained.
        /// </summary>
        public void AddManaRegenFactor(float factor)
        {
            m_ManaRegenFactors.Add(factor);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        public void AddManaRegenBonus(float bonus)
        {
            m_ManaRegenBoni.Add(bonus);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        //TODO_lukas: Create 'Remove Mana Regen' Factor/Bonus methods

        void UpdateManaRegen()
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;

            if (m_ManaRegenFactors.Count != 0)
            {
                for (int j = 0; j < m_ManaRegenFactors.Count; j++)
                {
                    combinedFactors += m_ManaRegenFactors[j];
                }
            }

            if (m_ManaRegenBoni.Count != 0)
            {
                for (int i = 0; i < m_ManaRegenBoni.Count; i++)
                {
                    combinedBoni += m_ManaRegenBoni[i];
                }
            }

            ManaRegen = combinedFactors * (m_BaseManaRegen + combinedBoni);
        }


        /*--------------------------------------------------------------------------
                    Manipulate Movement
        --------------------------------------------------------------------------*/

        public void AddMovementFactor (float factor)
        {
            m_MovementFactors.Add(factor);

            UpdateMovementSpeed();
        }

        //---------------------------------------------------------------------------

        public void AddMovementBonus(float bonus)
        {
            m_MovementBoni.Add(bonus);

            UpdateMovementSpeed();
        }

        //---------------------------------------------------------------------------

        //TODO_lukas: Create 'Remove MovementSpeed' Factor/Bonus methods

        void UpdateMovementSpeed()
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;

            if (m_MovementFactors.Count != 0)
            {
                for (int j = 0; j < m_MovementFactors.Count; j++)
                {
                    combinedFactors += m_MovementFactors[j];
                }
            }

            if (m_MovementBoni.Count != 0)
            {
                for (int i = 0; i < m_MovementBoni.Count; i++)
                {
                    combinedBoni += m_MovementBoni[i];
                }
            }

            MovementSpeed = combinedFactors * (m_BaseMovementSpeed + combinedBoni);
        }


        /*--------------------------------------------------------------------------
                    Update
        --------------------------------------------------------------------------*/

        void UpdateAttributes()
        {
            if (m_MaxHealth > 0)
                UpdateHealthRegen();

            if (m_MaxMana > 0)
                UpdateManaRegen();

            if (m_BaseMovementSpeed > 0)
                UpdateMovementSpeed();
        }

        //---------------------------------------------------------------------------

        public void PreTick (float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            ReplenishHealth(HealthRegen * deltaTime);
            ReplenishMana(ManaRegen * deltaTime);
        }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }


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

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection actions, float deltaTime)
        {

        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
