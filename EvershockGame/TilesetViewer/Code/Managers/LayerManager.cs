using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public enum ELayerMode
    {
        None,
        First,
        Second,
        Third
    }

    //---------------------------------------------------------------------------

    public class LayerManager : ModeManager<ELayerMode>
    {
        protected LayerManager() { }
    }
}
