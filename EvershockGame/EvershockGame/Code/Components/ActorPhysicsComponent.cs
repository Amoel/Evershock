using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

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
                float xMovement = (actions[EGameAction.MOVE_RIGHT] - actions[EGameAction.MOVE_LEFT]) * deltaTime * attributes.MovementSpeed* 200;
                float yMovement = (actions[EGameAction.MOVE_DOWN] - actions[EGameAction.MOVE_UP]) * deltaTime * attributes.MovementSpeed * 200;

                Vector3 movement = new Vector3(xMovement, yMovement, 0);
                if (movement.Length() > 0.01f) ApplyForce(new Vector3(xMovement, yMovement, 0));
            }

//#if DEBUG
//            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
//            {
//                ColliderComponent collider = GetComponent<ColliderComponent>();
//                if (collider != null)
//                {
//                    collider.SetCollisionState(false);
//                }
//            }
//            else
//            {
//                ColliderComponent collider = GetComponent<ColliderComponent>();
//                if (collider != null)
//                {
//                    collider.SetCollisionState(true);
//                }
//            }
//#endif
        }
    }
}
