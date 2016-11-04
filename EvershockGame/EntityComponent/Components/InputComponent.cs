using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Components
{
    [Serializable]
    [RequireComponent(typeof(PhysicsComponent))]
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

        public void Tick(float deltaTime)
        {
            //TransformComponent transform = GetComponent<TransformComponent>();
            //if (transform != null)
            //{
            //    float xMovement = (GetValue(EGameAction.MOVE_RIGHT) - GetValue(EGameAction.MOVE_LEFT)) * deltaTime * 150;
            //    float yMovement = (GetValue(EGameAction.MOVE_DOWN) - GetValue(EGameAction.MOVE_UP)) * deltaTime * 150;

            //    Vector3 movement = new Vector3(xMovement, yMovement, 0);
            //    if (movement.Length() > 0.0f)
            //    {
            //        transform.MoveBy(movement);
            //    }
            //}

            PhysicsComponent physics = GetComponent<PhysicsComponent>();
            if (physics != null)
            {
                float xMovement = (GetValue(EGameAction.MOVE_RIGHT) - GetValue(EGameAction.MOVE_LEFT)) * deltaTime * 200;
                float yMovement = (GetValue(EGameAction.MOVE_DOWN) - GetValue(EGameAction.MOVE_UP)) * deltaTime * 200;

                Vector3 movement = new Vector3(xMovement, yMovement, 0);
                if (movement.Length() > 0.0f)
                {
                    physics.ApplyForce(movement);
                }
            }

            //Health manipulation

            AttributesComponent attribute = GetComponent<AttributesComponent>();
            if (attribute != null)
            {
                float keyValue = GetValue(EGameAction.ADD_HEALTH);
                if (keyValue > 0)
                {
                    attribute.ReplenishHealth(1);
                }

                keyValue = GetValue(EGameAction.REDUCE_HEALTH);
                if (keyValue > 0)
                {
                    attribute.TakeDamage(1);
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

        class InputMapping
        {
            public InputMapping() { }
        }
    }
}
