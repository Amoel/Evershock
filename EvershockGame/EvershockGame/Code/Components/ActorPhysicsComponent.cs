using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code
{
    [RequireComponent(typeof(AttributesComponent))]
    public class ActorPhysicsComponent : PhysicsComponent
    {
        public ActorPhysicsComponent(Guid entity) : base(entity) { }


        public override void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            AttributesComponent attributes = GetComponent<AttributesComponent>();

            if (attributes != null)
            {
                float xMovement = (actions[EGameAction.MOVE_RIGHT] - actions[EGameAction.MOVE_LEFT]) * deltaTime * attributes.BaseMovementSpeed * 100;
                float yMovement = (actions[EGameAction.MOVE_DOWN] - actions[EGameAction.MOVE_UP]) * deltaTime * attributes.BaseMovementSpeed * 100;

                Vector3 movement = new Vector3(xMovement, yMovement, 0);
                if (movement.Length() > 0.01f) ApplyForce(new Vector3(xMovement, yMovement, 0));
            }
#if DEBUG
            else
            {
                throw new NullReferenceException("ActorPhysicsComponent does not have an AttributesComponent");
            }
#endif




        }
    }
}
