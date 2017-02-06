using EvershockGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public interface IPickupComponent
    {
        void OnPickup(IEntity collector);
    }
}
