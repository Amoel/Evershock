using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class MovementAnimationComponent : AnimationComponent, IInputReceiver
    {
        public MovementAnimationComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection collection, float deltaTime)
        {
            if (collection[EGameAction.MOVE_LEFT] > 0)
            {
                Play((int)Tag.MoveLeft);
            }
            else if (collection[EGameAction.MOVE_RIGHT] > 0)
            {
                Play((int)Tag.MoveRight);
            }
            else if (collection[EGameAction.MOVE_UP] > 0 || collection[EGameAction.MOVE_DOWN] > 0)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }
}
