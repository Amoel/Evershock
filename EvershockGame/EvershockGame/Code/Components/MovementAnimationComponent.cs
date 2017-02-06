using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class MovementAnimationComponent : AnimationComponent
    {
        public MovementAnimationComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Update(float horizontalMovement, float verticalMovement)
        {
            if (horizontalMovement < -1.0f)
            {
                Play((int)Tag.MoveLeft);
            }
            else if (horizontalMovement > 1.0f)
            {
                Play((int)Tag.MoveRight);
            }
            else if (Math.Abs(verticalMovement) > 1.0f)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }

        //---------------------------------------------------------------------------

        //public void ReceiveInput(GameActionCollection collection, float deltaTime)
        //{
        //    if (collection[EGameAction.MOVE_LEFT] > 0)
        //    {
        //        Play((int)Tag.MoveLeft);
        //    }
        //    else if (collection[EGameAction.MOVE_RIGHT] > 0)
        //    {
        //        Play((int)Tag.MoveRight);
        //    }
        //    else if (collection[EGameAction.MOVE_UP] > 0 || collection[EGameAction.MOVE_DOWN] > 0)
        //    {
        //        Play();
        //    }
        //    else
        //    {
        //        Stop();
        //    }
        //}
    }
}
