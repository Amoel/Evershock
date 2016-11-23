using Level;
using Managers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            //m_Canvas?.Create(width, height);
        }

        //---------------------------------------------------------------------------

        public void SetTile(ELayerMode mode, int sourceX, int sourceY, int destinationX, int destinationY, bool isBlocked)
        {
            //m_Canvas?.SetTile(mode, sourceX, sourceY, destinationX, destinationY, isBlocked);
        }

        //---------------------------------------------------------------------------

        public void UpdateTiles()
        {
            //m_Canvas?.UpdateTiles();
        }

        //---------------------------------------------------------------------------

        public void SaveAsTileset()
        {
            if (m_Canvas == null)
            {
                MessageBox.Show("No level available. Create or load level first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image file (*.png)|*.png";
            if (dialog.ShowDialog() == true)
            {
                //m_Canvas.SaveAsImage(dialog.FileName);
            }
        }

        //---------------------------------------------------------------------------

        public Map GetMap()
        {
            return null;
            //return m_Canvas?.GetMap();
        }

        //---------------------------------------------------------------------------

        public void SetMap(Map map)
        {
            //m_Canvas?.SetMap(map);
        }
    }
}
