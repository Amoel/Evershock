﻿using Level;
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

            Closing += OnClosing;
        }

        //---------------------------------------------------------------------------

        public void UpdateTitle(string text)
        {
            Title = string.Format("TilesetViewer{0}", string.IsNullOrWhiteSpace(text) ? "" : string.Format(" - {}", text));
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

        private void OnLoadLevelClicked(object sender, EventArgs e)
        {
            MessageBoxResult result = MapManager.Get().CheckForChanges();
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ResourceManager.Get().Save(MapManager.Get().GetMap());
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    return;
            }

            Map map = ResourceManager.Get().Load();
            if (map != null)
            {
                MapManager.Get().SetMap(map);
            }
        }

        //---------------------------------------------------------------------------

        private void OnSaveLevelClicked(object sender, EventArgs e)
        {
            Map map = MapManager.Get().GetMap();
            if (map != null)
            {
                ResourceManager.Get().Save(map);
            }
        }

        //---------------------------------------------------------------------------

        private void LoadTileset(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage(new Uri(path));
                if (bitmap != null)
                {
                    TilesetManager.Get().CreateTileset(path);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //---------------------------------------------------------------------------

        private void OnUndo(object sender, EventArgs e)
        {
            UndoManager.Get().Undo();
        }

        //---------------------------------------------------------------------------

        private void OnClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MapManager.Get().CheckForChanges();
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ResourceManager.Get().Save(MapManager.Get().GetMap());
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
