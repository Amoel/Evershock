using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public class GameActionCollection
    {
        public Dictionary<EGameAction, float> Actions;

        //---------------------------------------------------------------------------

        public GameActionCollection()
        {
            Actions = new Dictionary<EGameAction, float>();
        }

        //---------------------------------------------------------------------------

        public float this[EGameAction action]
        {
            get { return (Actions.ContainsKey(action) ? Actions[action] : 0.0f); }
        }


    }
}
