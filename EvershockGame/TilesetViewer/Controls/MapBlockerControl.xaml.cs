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
    /// Interaction logic for MapBlockerControl.xaml
    /// </summary>
    public partial class MapBlockerControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WriteableBitmap Image { get; set; }
        public float ImageOpacity { get; set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public int PxTileWidth { get { return MapManager.Get().PxTileWidth; } }
        public int PxTileHeight { get { return MapManager.Get().PxTileHeight; } }

        public int PxWidth { get { return MapWidth * PxTileWidth; } }
        public int PxHeight { get { return MapHeight * PxTileHeight; } }

        //---------------------------------------------------------------------------

        public MapBlockerControl()
        {
            InitializeComponent();
            DataContext = this;

            EditManager.Get().ModeChanged += OnModeChanged;
        }

        //---------------------------------------------------------------------------

        public void Dispose()
        {
            EditManager.Get().ModeChanged -= OnModeChanged;
        }

        //---------------------------------------------------------------------------

        public void Init(int width, int height)
        {
            MapWidth = width;
            OnPropertyChanged("MapWidth");
            OnPropertyChanged("PxWidth");

            MapHeight = height;
            OnPropertyChanged("MapHeight");
            OnPropertyChanged("PxHeight");

            ResetImage();
            OnModeChanged(EditManager.Get().Mode);
        }

        //---------------------------------------------------------------------------

        private void ResetImage()
        {
            Image = new WriteableBitmap(PxWidth, PxHeight, 96, 96, PixelFormats.Bgra32, null);
            OnPropertyChanged("Image");
        }

        //---------------------------------------------------------------------------

        public void Resize(int left, int right, int top, int bottom)
        {

            Int32Rect targetRect = new Int32Rect(
                Math.Max(0, -left * PxTileWidth),
                Math.Max(0, -top * PxTileHeight),
                Image.PixelWidth - Math.Max(0, -(left + right) * PxTileWidth),
                Image.PixelHeight - Math.Max(0, -(top + bottom) * PxTileHeight));

            int bytesPerPixel = (Image.Format.BitsPerPixel + 7) / 8;
            int stride = bytesPerPixel * targetRect.Width;
            byte[] data = new byte[stride * targetRect.Height];
            Image.CopyPixels(targetRect, data, stride, 0);

            MapWidth += (left + right);
            MapHeight += (top + bottom);
            WriteableBitmap temp = new WriteableBitmap(PxWidth, PxHeight, 96, 96, PixelFormats.Bgra32, null);
            Int32Rect sourceRect = new Int32Rect(Math.Max(0, left * PxTileWidth), Math.Max(0, top * PxTileHeight), targetRect.Width, targetRect.Height);
            temp.WritePixels(sourceRect, data, stride, 0);
            Image = temp;
            OnPropertyChanged("Image");
        }

        //---------------------------------------------------------------------------

        public void SetBlocker(int sourceX, int sourceY, bool isBlocker)
        {
            int stride = Image.PixelWidth * (Image.Format.BitsPerPixel + 7) / 8;
            byte[] data = new byte[Image.PixelHeight * stride];
            for (int i = 0; i < data.Length; i += 4)
            {
                if (isBlocker)
                {
                    data[i] = 0;
                    data[i + 1] = 0;
                    data[i + 2] = 255;
                    data[i + 3] = 255;
                }
                else
                {
                    data[i] = 0;
                    data[i + 1] = 255;
                    data[i + 2] = 0;
                    data[i + 3] = 255;
                }
            }

            Image.WritePixels(new Int32Rect(sourceX * PxTileWidth, sourceY * PxTileHeight, PxTileWidth, PxTileHeight), data, stride, 0);
            OnPropertyChanged("Image");
        }

        //---------------------------------------------------------------------------

        private void OnModeChanged(EEditMode mode)
        {
            ImageOpacity = (mode == EEditMode.Blocker ? 0.4f : 0.0f);
            OnPropertyChanged("ImageOpacity");
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
