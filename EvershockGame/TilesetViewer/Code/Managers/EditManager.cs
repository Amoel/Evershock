using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public enum EEditMode
    {
        None,
        Tiles,
        Eraser,
        Selection,
        Fill,
        Blocker
    }

    //---------------------------------------------------------------------------

    public class EditManager : ModeManager<EEditMode>
    {
        protected EditManager() { }
    }
}
