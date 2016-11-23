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
    /// Interaction logic for MapPreview.xaml
    /// </summary>
    public partial class MapPreview : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Thumbnail { get; set; }
        public string MapName { get; set; }

        public string MapPath { get; set; }

        //---------------------------------------------------------------------------

        public MapPreview()
        {
            InitializeComponent();
            DataContext = this;
        }

        //---------------------------------------------------------------------------

        public void Init(BitmapImage thumbnail, string mapName, string mapPath)
        {
            Thumbnail = thumbnail;
            OnPropertyChanged("Thumbnail");
            MapName = mapName;
            OnPropertyChanged("MapName");
            MapPath = mapPath;
            OnPropertyChanged("MapPath");
        }

        //---------------------------------------------------------------------------

        private void OnClick(object sender, EventArgs e)
        {
            Console.WriteLine(MapName);
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
