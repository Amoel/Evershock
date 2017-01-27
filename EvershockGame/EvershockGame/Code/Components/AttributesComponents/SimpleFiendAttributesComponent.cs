using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
{
    public class SimpleFiendAttributesComponent : AttributesComponent
    {
        public SimpleFiendAttributesComponent (Guid entity) : base (entity)
        {
            //Fill with stuff that only the Simple Fiend needs
        }
    }
}
