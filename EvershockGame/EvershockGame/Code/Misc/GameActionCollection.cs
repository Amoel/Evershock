using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame
{
    public class GameActionCollection
    {
        public Dictionary<EGameAction, GameActionData> Actions;

        //---------------------------------------------------------------------------

        public GameActionCollection()
        {
            Actions = new Dictionary<EGameAction, GameActionData>();
        }

        //---------------------------------------------------------------------------

        public GameActionData this[EGameAction action]
        {
            get { return (Actions.ContainsKey(action) ? Actions[action] : null); }
        }

        //---------------------------------------------------------------------------

        public void Update(EGameAction action, float value)
        {
            if (Actions.ContainsKey(action))
            {
                Actions[action].Update(value);
            }
            else
            {
                Actions.Add(action, new GameActionData(value));
            }
        }
    }

    //---------------------------------------------------------------------------

    public class GameActionData
    {
        public float Value { get; private set; }
        public bool IsPressed { get; private set; }
        public bool IsReleased { get; private set; }

        public bool IsButtonDown { get { return Value > 0.0f; } }

        //---------------------------------------------------------------------------

        public GameActionData(float value)
        {
            Value = value;
        }

        //---------------------------------------------------------------------------

        public void Update(float value)
        {
            IsPressed = (Value == 0.0f && value > 0.0f);
            IsReleased = (Value > 0.0f && value == 0.0f);
            Value = value;
        }
    }
}
