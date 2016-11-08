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
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Image { get { return TilesetManager.Get().Tileset.Source; } }
        public Rect View { get; set; }

        private int m_TileWidth = 16;
        private int m_tileHeight = 16;

        //---------------------------------------------------------------------------

        public Tile()
        {
            InitializeComponent();
            DataContext = this;
        }

        //---------------------------------------------------------------------------

        public void Init(int x, int y, int value)
        {
            Canvas.SetLeft(this, x * m_TileWidth);
            Canvas.SetTop(this, y * m_tileHeight);
            Width = m_TileWidth;
            Height = m_tileHeight;
            OnPropertyChanged("Image");

            Update(x, y);
        }

        //---------------------------------------------------------------------------

        public void Update(int x, int y)
        {
            View = TilesetManager.Get().Tileset.Crop(x, y);
            OnPropertyChanged("View");
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
