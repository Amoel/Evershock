using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TilesetViewer
{
    public enum EPanel
    {
        Tilset,
        Map
    }

    //---------------------------------------------------------------------------

    public class ZoomManager : BaseManager<ZoomManager>
    {
        private Dictionary<EPanel, FrameworkElement> m_Panels;

        //---------------------------------------------------------------------------

        protected ZoomManager()
        {
            m_Panels = new Dictionary<EPanel, FrameworkElement>();
        }

        //---------------------------------------------------------------------------

        public void Register()
        {

        }

        //---------------------------------------------------------------------------

        public void Zoom(float delta)
        {

        }
    }
}
