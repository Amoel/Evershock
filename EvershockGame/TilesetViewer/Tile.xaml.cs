﻿using System;
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
    public partial class Tile : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Image { get { return TilesetManager.Get().Tileset?.Source; } }
        public Rect View { get; set; }
        public bool IsBlocked { get; set; }

        private int m_TileWidth = 16;
        private int m_tileHeight = 16;

        //---------------------------------------------------------------------------

        public Tile()
        {
            InitializeComponent();
            DataContext = this;

            ModeManager.Get().ModeChanged += OnModeChanged;
        }

        //---------------------------------------------------------------------------

        public void Dispose()
        {
            ModeManager.Get().ModeChanged -= OnModeChanged;
        }

        //---------------------------------------------------------------------------

        private void OnModeChanged(EEditMode mode)
        {
            UpdateMode(mode);
        }

        private void UpdateMode(EEditMode mode)
        {
            switch (mode)
            {
                case EEditMode.Tiles:
                    BlockerTint.Visibility = Visibility.Hidden;
                    break;
                case EEditMode.Blocker:
                    BlockerTint.Visibility = Visibility.Visible;
                    break;
            }
        }

        //---------------------------------------------------------------------------

        public void Init(int x, int y)
        {
            Canvas.SetLeft(this, x * m_TileWidth);
            Canvas.SetTop(this, y * m_tileHeight);
            Width = m_TileWidth;
            Height = m_tileHeight;
            UpdateImage();
            UpdateMode(ModeManager.Get().Mode);
        }

        public void Init(int x, int y, Cell cell)
        {
            Init(x, y);
            Update(cell.IsBlocked);
            Update(cell.View);
        }

        //---------------------------------------------------------------------------
        
        public void UpdateImage()
        {
            OnPropertyChanged("Image");
        }

        //---------------------------------------------------------------------------

        public void Update(int x, int y)
        {
            View = TilesetManager.Get().Tileset.Crop(x, y);
            OnPropertyChanged("View");
        }

        //---------------------------------------------------------------------------

        public void Update(Rect view)
        {
            View = view;
            OnPropertyChanged("View");
        }

        //---------------------------------------------------------------------------

        public void Update(bool isBlocked)
        {
            IsBlocked = isBlocked;
            OnPropertyChanged("IsBlocked");
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
