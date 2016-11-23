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
    public delegate void TilesetSelectionChangedEventHandler(SelectionRect selection);

    //---------------------------------------------------------------------------

    public class TilesetManager : BaseManager<TilesetManager>
    {
        public event TilesetSelectionChangedEventHandler TilesetSelectionChanged;

        public BitmapImage Image { get; private set; }
        public Tileset Tileset { get; private set; }

        private TilesetCanvas m_Canvas;

        //---------------------------------------------------------------------------

        protected TilesetManager()
        {
        }

        //---------------------------------------------------------------------------

        public void CreateTileset(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(path));
                if (bitmap != null)
                {
                    int pxTileWidth = MapManager.Get().PxTileWidth;
                    int pxTileHeight = MapManager.Get().PxTileHeight;
                    Tileset = new Tileset(Path.GetFileNameWithoutExtension(path), bitmap, pxTileWidth, pxTileHeight);
                    m_Canvas?.Update();
                    MapManager.Get().SetTileset(Path.GetFileNameWithoutExtension(path));
                    Properties.Settings.Default.LastTilesetPath = path;
                    Properties.Settings.Default.Save();
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

        //---------------------------------------------------------------------------

        public void UpdateSelection()
        {
            OnTilesetSelectionChanged(m_Canvas?.Selection);
        }

        //---------------------------------------------------------------------------

        public void UpdateTileDimension(int width, int height)
        {
            if (Tileset != null)
            {
                Tileset.PxTileWidth = width;
                Tileset.PxTileHeight = height;
            }
        }

        //---------------------------------------------------------------------------

        private void OnTilesetSelectionChanged(SelectionRect selection)
        {
            TilesetSelectionChanged?.Invoke(selection);
        }
    }
}
