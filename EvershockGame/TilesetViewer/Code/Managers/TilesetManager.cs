using Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public void CreateTileset(string path, int pxTileWidth, int pxTileHeight)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(path));
                if (bitmap != null)
                {
                    Tileset = new Tileset(Path.GetFileNameWithoutExtension(path), bitmap, pxTileWidth, pxTileHeight);
                    m_Canvas?.Update();
                    LevelManager.Get().UpdateTiles();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        //---------------------------------------------------------------------------

        public void RegisterCanvas(TilesetCanvas canvas)
        {
            m_Canvas = canvas;
        }

        //---------------------------------------------------------------------------

        public SelectionRect GetSelection()
        {
            return m_Canvas?.Selection ?? new SelectionRect(0, 0, 0, 0);
        }
    }
}
