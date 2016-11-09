using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TilesetViewer
{
    public class TilesetManager : BaseManager<TilesetManager>
    {
        public BitmapImage Image { get; private set; }

        public Tileset Tileset { get; private set; }

        private TilesetCanvas m_Canvas;

        //---------------------------------------------------------------------------

        protected TilesetManager()
        {
        }

        //---------------------------------------------------------------------------

        public void CreateTileset(BitmapImage source, int pxTileWidth, int pxTileHeight)
        {
            Tileset = new Tileset(source, pxTileWidth, pxTileHeight);
            m_Canvas?.Update();
            LevelManager.Get().UpdateTiles();
        }

        //---------------------------------------------------------------------------

        public void RegisterCanvas(TilesetCanvas canvas)
        {
            m_Canvas = canvas;
        }

        //---------------------------------------------------------------------------

        public SelectionRect GetSelection()
        {
            return m_Canvas?.Selection;
        }
    }
}
