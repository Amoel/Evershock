using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using EvershockGame.Code.Components;

namespace EvershockGame.Code
{
    [RequireComponent(typeof(AttributesComponent), typeof(MovementAnimationComponent))]
    public class ActorPhysicsComponent : PhysicsComponent
    {
        public ActorPhysicsComponent(Guid entity) : base(entity)
        {
            IsGravityAffected = false;
        }

        //---------------------------------------------------------------------------

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            MovementAnimationComponent animation = GetComponent<MovementAnimationComponent>();

            if (animation != null)
            {
                animation.Update(GetForce().X, GetForce().Y);
            }
        }

        //---------------------------------------------------------------------------

        public override void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            AttributesComponent attributes = GetComponent<AttributesComponent>();
            
            if (attributes != null)
            {
                float xMovement = (actions[EGameAction.MOVE_RIGHT].Value - actions[EGameAction.MOVE_LEFT].Value);// * deltaTime * attributes.MovementSpeed* 20;
                float yMovement = (actions[EGameAction.MOVE_DOWN].Value - actions[EGameAction.MOVE_UP].Value);// * deltaTime * attributes.MovementSpeed * 20;
                
                Vector3 movement = new Vector3(xMovement, yMovement, 0);
                if (movement.Length() > 0.01f)
                {
                    //ApplyForce(new Vector3(xMovement, yMovement, 0), true);
                    ApplyAbsoluteForce(Vector3.Normalize(movement) * deltaTime * attributes.MovementSpeed * 200.0f);

                }

                float xDirection = (actions[EGameAction.LOOK_RIGHT].Value - actions[EGameAction.LOOK_LEFT].Value);
                float yDirection = (actions[EGameAction.LOOK_DOWN].Value - actions[EGameAction.LOOK_UP].Value);

                Vector2 directon = new Vector2(xDirection, yDirection);
                if (directon.Length() > 0.01f)
                {
                    TransformComponent transform = GetComponent<TransformComponent>();
                    if (transform != null)
                    {
                        transform.OrientateTo(directon);
                    }
                }
            }
        }
    }
}
