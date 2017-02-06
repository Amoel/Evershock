using EvershockGame.Entities;
using EvershockGame.Manager;
using EvershockGame.Code.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Entities.UI
{
    public class Healthbar : UIEntity
    {
        public HealthbarComponent Properties { get { return GetComponent<HealthbarComponent>(); } }

        //---------------------------------------------------------------------------

        public Healthbar(string name, Frame frame) : base(name, frame)
        {
            AddComponent<HealthbarComponent>();
        }
    }
}
