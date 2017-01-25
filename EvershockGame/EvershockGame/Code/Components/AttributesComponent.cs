using EntityComponent;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;

namespace EvershockGame.Code
{
    [Serializable]
    public class AttributesComponent : Component, IInputReceiver, ITickableComponent
    {
        //TODO_lukas: Split into several Attribute Components for Enemies / Players, etc.

        //Health
        float m_MaxHealth;
        float m_CurrentHealth;
        float m_BaseHealthRegen;
        Dictionary<string, float> m_HealthRegenFactors = new Dictionary<string, float>();
        Dictionary<string, float> m_HealthRegenBoni = new Dictionary<string, float>();

        //Mana
        float m_MaxMana;
        float m_CurrentMana;
        float m_BaseManaRegen;
        Dictionary<string, float> m_ManaRegenFactors = new Dictionary<string, float>();
        Dictionary<string, float> m_ManaRegenBoni = new Dictionary<string, float>();

        //MovementSpeed
        float m_BaseMovementSpeed;
        Dictionary<string, float> m_MovementFactors = new Dictionary<string, float>();
        Dictionary<string, float> m_MovementBoni = new Dictionary<string, float>();

        //public read-only handles
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
        /// Factors above 0 increase Health Regeneration; Factors below 0 decrease Health Regeneration;
        /// </summary>
        public void AddHealthRegenFactor(string name, float factor)
        {
            m_HealthRegenFactors.Add(name,factor);

            UpdateHealthRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Remove Health Regeneration Factors by name;
        /// </summary>
        public void RemoveHealthRegenFactor (string name)
        {
            m_HealthRegenFactors.Remove(name);

            UpdateHealthRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Values above 0 increase Health Regeneration; Values below 0 decrease Health Regeneration;
        /// </summary>
        public void AddHealthRegenBonus(string name, float bonus)
        {
            m_HealthRegenBoni.Add(name, bonus);

            UpdateHealthRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Remove Health Regeneration Boni/Mali by name;
        /// </summary>
        public void RemoveHealthRegenBonus(string name)
        {
            m_HealthRegenBoni.Remove(name);

            UpdateHealthRegen();
        }

        //---------------------------------------------------------------------------

        void UpdateHealthRegen() //If this becomes too big, it would be more efficient not to go through all values each time
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;

            foreach (float factor in m_HealthRegenFactors.Values)
            {
                combinedFactors += factor;
            }
            
            foreach (float bonus in m_HealthRegenBoni.Values)
            {
                combinedBoni += bonus;
            }

            HealthRegen = (m_BaseHealthRegen + combinedBoni) * combinedFactors;
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
        /// Factors above 0 increase Mana Regeneration; Factors below 0 decrease Mana Regeneration;
        /// </summary>
        public void AddManaRegenFactor(string name, float factor)
        {
            m_ManaRegenFactors.Add(name, factor);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Remove Mana Regeneration Factors by name;
        /// </summary>
        public void RemoveManaRegenFactor(string name)
        {
            m_ManaRegenFactors.Remove(name);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Values above 0 increase Mana Regeneration; Values below 0 decrease Mana Regeneration;
        /// </summary>
        public void AddManaRegenBonus(string name, float bonus)
        {
            m_ManaRegenBoni.Add(name, bonus);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        /// <summary>
        /// Remove Mana Regeneration Boni/Mali by name;
        /// </summary>
        public void RemoveManaRegenBonus(string name)
        {
            m_ManaRegenBoni.Remove(name);

            UpdateManaRegen();
        }

        //---------------------------------------------------------------------------

        void UpdateManaRegen() //If this becomes too big, it would be more efficient not to go through all values each time
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;
            
            foreach (float factor in m_ManaRegenFactors.Values)
            {
                combinedFactors += factor;
            }

            foreach (float bonus in m_ManaRegenBoni.Values)
            {
                combinedBoni += bonus;
            }

            ManaRegen = combinedFactors * (m_BaseManaRegen + combinedBoni);
        }


        /*--------------------------------------------------------------------------
                    Manipulate Movement
        --------------------------------------------------------------------------*/

        public void AddMovementFactor(string name, float factor)
        {
            m_MovementFactors.Add(name, factor);

            UpdateMovementSpeed();
        }

        //---------------------------------------------------------------------------

        public void AddMovementBonus(string name, float bonus)
        {
            m_MovementBoni.Add(name, bonus);

            UpdateMovementSpeed();
        }

        //---------------------------------------------------------------------------

        void UpdateMovementSpeed()  //If this becomes too big, it would be more efficient not to go through all values each time
        {
            float combinedFactors = 1.0f;
            float combinedBoni = 0.0f;

            foreach (float factor in m_MovementFactors.Values)
            {
                combinedFactors += factor;
            }

            foreach (float bonus in m_MovementBoni.Values)
            {
                combinedBoni += bonus;
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
