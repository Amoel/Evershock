using EvershockGame.Manager;
using EvershockGame.Code.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvershockGame.Code.Manager;

namespace EvershockGame.Code.Entities.UI
{
    public class Healthbar : UIEntity
    {
        public HealthbarComponent Properties { get { return GetComponent<HealthbarComponent>(); } }

        //---------------------------------------------------------------------------

        public Healthbar(string name, Guid parent, Frame frame) : base(name, parent, frame)
        {
            AddComponent<HealthbarComponent>();
        }
    }
}
