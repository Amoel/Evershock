using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public interface IInputReceiver
    {
        void ReceiveInput(GameActionCollection actions, float deltaTime);
    }
}
