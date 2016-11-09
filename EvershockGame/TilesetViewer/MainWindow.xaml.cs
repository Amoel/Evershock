using Microsoft.Win32;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        //---------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        //---------------------------------------------------------------------------

        private void OnLoadTilesetClicked(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadTileset(dialog.FileName);
            }
        }

        //---------------------------------------------------------------------------

        private void OnExportTilesetClicked(object sender, EventArgs e)
        {
            LevelManager.Get().SaveAsTileset();
        }

        //---------------------------------------------------------------------------

        private void OnCreateLevelClicked(object sender, EventArgs e)
        {
            LevelCreationWindow window = new LevelCreationWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        //---------------------------------------------------------------------------

        private void LoadTileset(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(path));
                if (bitmap != null)
                {
                    TilesetManager.Get().CreateTileset(bitmap, 16, 16);
                    //LevelManager.Get().Create(20, 20);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
