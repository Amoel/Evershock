using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace EntityComponent.Components
{
    [Serializable]
    public class InputComponent : Component, ITickableComponent
    {
        private Dictionary<EGameAction, EInput[]> m_Mappings;
        private PlayerIndex m_PlayerIndex;

        //---------------------------------------------------------------------------

        public InputComponent(Guid entity) : base(entity)
        {
            m_Mappings = new Dictionary<EGameAction, EInput[]>();
            m_PlayerIndex = PlayerIndex.One;
        }

        //---------------------------------------------------------------------------

        public void PreTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void PostTick(float deltaTime) { }

        //---------------------------------------------------------------------------

        public void Tick(float deltaTime)
        {
            GameActionCollection actions = new GameActionCollection();
            foreach (EGameAction action in m_Mappings.Keys)
            {
                actions.Actions.Add(action, GetValue(action));
            }

            foreach (IComponent component in GetComponents())
            {
                if (component.Equals(this)) continue;

                if (component is IInputReceiver)
                {
                    (component as IInputReceiver).ReceiveInput(actions, deltaTime);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void MapAction(EGameAction action, params EInput[] input)
        {
            if (!m_Mappings.ContainsKey(action))
            {
                m_Mappings.Add(action, input);
            }
            else
            {
                m_Mappings[action] = input;
            }
        }

        //---------------------------------------------------------------------------

        private float GetValue(EGameAction action)
        {
            if (m_Mappings.ContainsKey(action))
            {
                float value = 0.0f;
                foreach (EInput input in m_Mappings[action])
                {
                    value = Math.Max(value, InputManager.Get().GetValue(input, m_PlayerIndex));
                }
                return value;
            }
            return 0;
        }

        //---------------------------------------------------------------------------

        public void ClearActions()
        {
            m_Mappings.Clear();
        }

        //---------------------------------------------------------------------------

        class InputMapping
        {
            public InputMapping() { }
        }
    }
}
