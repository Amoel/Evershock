using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class LevelManager : BaseManager<LevelManager>
    {
        private LevelCanvas m_Canvas;

        //---------------------------------------------------------------------------

        protected LevelManager() { }

        //---------------------------------------------------------------------------

        public void RegisterCanvas(LevelCanvas canvas)
        {
            m_Canvas = canvas;
        }

        //---------------------------------------------------------------------------

        public void Create(int width, int height)
        {
            if (m_Canvas != null)
            {
                m_Canvas.Create(width, height);
            }
        }
    }
}
