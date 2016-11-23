using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for MapPreviewContainer.xaml
    /// </summary>
    public partial class MapPreviewContainer : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsContainerVisible { get; set; }

        //---------------------------------------------------------------------------

        public MapPreviewContainer()
        {
            InitializeComponent();
            DataContext = this;

            Init();

            InputManager.Get().RegisterShortcut(Key.M, ModifierKeys.Control, () =>
            {
                SetContainerVisibility(!IsContainerVisible);
            });
        }

        //---------------------------------------------------------------------------

        private void Init()
        {
            string rootDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TilesetViewer/Maps");
            string[] files = Directory.GetFiles(rootDirectory, "*.data");
            foreach (string file in files)
            {
                AddPreview(file);
            }
        }

        //---------------------------------------------------------------------------

        public void AddPreview(BitmapImage thumbnail, string mapName, string mapPath)
        {
            MapPreview preview = new MapPreview();
            preview.VerticalAlignment = VerticalAlignment.Top;
            preview.Init(thumbnail, mapName, mapPath);
            Previews.Children.Add(preview);
        }

        //---------------------------------------------------------------------------

        public void AddPreview(string path)
        {
            MapMetaData data = null;
            try
            {
                using (FileStream stream = new FileStream(@path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data = JsonConvert.DeserializeObject<MapMetaData>(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {

            }
            if (data != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(data.ThumbnailPath));
                AddPreview(bitmap, data.MapName, data.MapPath);
            }
        }

        //---------------------------------------------------------------------------

        public void SetContainerVisibility(bool isVisible)
        {
            IsContainerVisible = isVisible;
            OnPropertyChanged("IsContainerVisible");
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
