using Level;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class MapManager : BaseManager<MapManager>
    {
        private MapControl m_Control;
        private Map m_Map;

        //---------------------------------------------------------------------------

        protected MapManager() { }

        //---------------------------------------------------------------------------

        public void Register(MapControl control)
        {
            m_Control = control;
        }

        //---------------------------------------------------------------------------

        public void SetTile(int sourceX, int sourceY, int destinationX, int destinationY)
        {

        }

        //---------------------------------------------------------------------------

        public void ClearTile(int sourceX, int sourceY)
        {

        }

        //---------------------------------------------------------------------------   

        public void FillTile(int sourceX, int sourceY, int destinationX, int destinationY)
        {

        }

        //---------------------------------------------------------------------------

        public void UpdateImage()
        {
            //m_Control?.UpdateImage();
        }

        //---------------------------------------------------------------------------

        public void Reset(int width, int height)
        {
            m_Control?.Reset(width, height);
        }
    }
}
