using EvershockGame;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using EvershockGame.Components;
using EvershockGame.Code.Manager;

namespace EvershockGame.Code.Components
{
    [Serializable]
    public class AttributesComponent : Component, ITickableComponent
    {
        //Base Values
        protected float m_MaxHealth;
        protected float m_CurrentHealth;
        protected float m_BaseMovementSpeed;
        public float MovementSpeed { get; protected set; }

        public float MaxHealth  //handle for UI
        {
            get { return m_MaxHealth; }
            set { if (m_MaxHealth != value) { m_MaxHealth = value; OnPropertyChanged(m_MaxHealth); } }
        }

        public float CurrentHealth  //handle for UI
        { 
            get { return m_CurrentHealth; }
            set { if (m_CurrentHealth != value) { m_CurrentHealth = value;  OnPropertyChanged(m_CurrentHealth); } }
        }


        /*--------------------------------------------------------------------------
                    Constructor
        --------------------------------------------------------------------------*/

        public AttributesComponent(Guid entity) : base(entity)
        {
            m_MaxHealth = 100.0f;
            CurrentHealth = m_MaxHealth;
            m_BaseMovementSpeed = 100.0f;

            MovementSpeed = m_BaseMovementSpeed;
        }


        /*--------------------------------------------------------------------------
                    Init
        --------------------------------------------------------------------------*/

        public void Init(float max_health = 100, float base_movement_speed = 100)
        {
            m_MaxHealth = max_health;
            m_BaseMovementSpeed = base_movement_speed;
            MovementSpeed = m_BaseMovementSpeed;
        }


        /*--------------------------------------------------------------------------
                    Manipulate Health
        --------------------------------------------------------------------------*/

        public void TakeDamage(float damage_dealt)
        {
            CurrentHealth -= damage_dealt;

            if (CurrentHealth <= 0)
            {
                DespawnComponent despawn = GetComponent<DespawnComponent>();
                if (despawn != null)
                {
                    despawn.Trigger();
                }
                if (EntityManager.Get().Find<Player>(Entity) != null)
                {
                    //TODO_lukas: Think about what happens when Players are killed
                }
            }
        }

        //---------------------------------------------------------------------------
        
        public void ReplenishHealth(float health_gain)
        {
            CurrentHealth += health_gain;

            if (CurrentHealth > m_MaxHealth)
                CurrentHealth = m_MaxHealth;
        }

        
        /*--------------------------------------------------------------------------
                    Update
        --------------------------------------------------------------------------*/

        public void PreTick (float deltaTime) { }

        public virtual void Tick(float deltaTime) { }

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
