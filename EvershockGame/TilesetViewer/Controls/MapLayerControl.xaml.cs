using Level;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TilesetViewer
{
    /// <summary>
    /// Interaction logic for MapLayerControl.xaml
    /// </summary>
    public partial class MapLayerControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private ELayerMode m_Mode;

        public int PxTileWidth { get { return MapManager.Get().PxTileWidth; } }
        public int PxTileHeight { get { return MapManager.Get().PxTileHeight; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public WriteableBitmap Image { get; set; }
        public float ImageOpacity { get; set; } = 1.0f;
        public float TintOpacity { get; set; } = 0.0f;

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }

        public int PxWidth { get { return MapWidth * PxTileWidth; } }
        public int PxHeight { get { return MapHeight * PxTileHeight; } }

        //---------------------------------------------------------------------------

        public MapLayerControl()
        {
            InitializeComponent();
            DataContext = this;

            LayerManager.Get().ModeChanged += OnModeChanged;
        }

        //---------------------------------------------------------------------------

        public void Dispose()
        {
            LayerManager.Get().ModeChanged -= OnModeChanged;
        }

        //---------------------------------------------------------------------------

        private void ResetImage()
        {
            Image = new WriteableBitmap(PxWidth, PxHeight, 96, 96, PixelFormats.Bgra32, null);
            OnPropertyChanged("Image");
        }

        //---------------------------------------------------------------------------

        public void Init(ELayerMode mode, int width, int height)
        {
            m_Mode = mode;
            Reset(width, height);
            OnModeChanged(mode);
        }

        //---------------------------------------------------------------------------

        public void Reset(int width, int height)
        {
            MapWidth = width;
            OnPropertyChanged("MapWidth");
            OnPropertyChanged("PxWidth");

            MapHeight = height;
            OnPropertyChanged("MapHeight");
            OnPropertyChanged("PxHeight");

            ResetImage();
        }

        //---------------------------------------------------------------------------

        public void SetTile(int sourceX, int sourceY, int targetX, int targetY)
        {
            BitmapSource source = TilesetManager.Get().Tileset?.Source;
            if (source != null)
            {
                int sourceStride = source.PixelWidth * (source.Format.BitsPerPixel + 7) / 8;
                int size = source.PixelHeight * sourceStride;
                byte[] data = new byte[size];

                TilesetManager.Get().Tileset.Source.CopyPixels(new Int32Rect(targetX * PxTileWidth, targetY * PxTileHeight, PxTileWidth, PxTileHeight), data, sourceStride, 0);
                Image.WritePixels(new Int32Rect(sourceX * PxTileWidth, sourceY * PxTileHeight, PxTileWidth, PxTileHeight), data, sourceStride, 0);
            }
        }

        //---------------------------------------------------------------------------

        public void EraseTiles(int sourceX, int sourceY)
        {
            int stride = Image.PixelWidth * (Image.Format.BitsPerPixel + 7) / 8;
            byte[] data = new byte[Image.PixelHeight * stride];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
            Image.WritePixels(new Int32Rect(sourceX * PxTileWidth, sourceY * PxTileHeight, PxTileWidth, PxTileHeight), data, stride, 0);
        }

        //---------------------------------------------------------------------------

        public void FillTiles(int sourceX, int sourceY)
        {

        }

        //---------------------------------------------------------------------------

        private void OnModeChanged(ELayerMode mode)
        {
            if (mode < m_Mode)
            {
                ImageOpacity = 0.3f;
                TintOpacity = 0.0f;
            }
            else if (mode > m_Mode)
            {
                ImageOpacity = 1.0f;
                TintOpacity = 0.5f;
            }
            else
            {
                ImageOpacity = 1.0f;
                TintOpacity = 0.0f;
            }
            OnPropertyChanged("ImageOpacity");
            OnPropertyChanged("TintOpacity");
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
