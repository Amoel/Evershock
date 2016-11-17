using Level;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl, INotifyPropertyChanged
    {
        public int PxTileWidth { get { return MapManager.Get().PxTileWidth; } }
        public int PxTileHeight { get { return MapManager.Get().PxTileHeight; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }

        public int PxWidth { get { return MapWidth * PxTileWidth; } }
        public int PxHeight { get { return MapHeight * PxTileHeight; } }
        
        private Dictionary<ELayerMode, MapLayerControl> m_Layers;
        
        private Point m_Dragged;
        private Point m_Clicked;

        //---------------------------------------------------------------------------

        public MapControl()
        {
            InitializeComponent();
            DataContext = this;

            m_Layers = new Dictionary<ELayerMode, MapLayerControl>();
            foreach (ELayerMode mode in Enum.GetValues(typeof(ELayerMode)))
            {
                MapLayerControl control = new MapLayerControl();
                control.IsHitTestVisible = false;

                m_Layers.Add(mode, control);
                LayerContainer.Children.Add(control);
            }

            MapManager.Get().Register(this);
        }

        //---------------------------------------------------------------------------

        public void Reset(int width, int height)
        {
            foreach (KeyValuePair<ELayerMode, MapLayerControl> kvp in m_Layers)
            {
                kvp.Value.Init(kvp.Key, width, height);
            }
            BlockerContainer.Init(width, height);

            MapWidth = width;
            OnPropertyChanged("MapWidth");
            OnPropertyChanged("PxWidth");

            MapHeight = height;
            OnPropertyChanged("MapHeight");
            OnPropertyChanged("PxHeight");
        }

        //---------------------------------------------------------------------------

        private void UpdateHighlight(int x, int y)
        {
            SelectionRect tilesetSelection = TilesetManager.Get().GetSelection();
            SelectionRect rect = new SelectionRect(x, y, tilesetSelection.Width, tilesetSelection.Height).Within(new SelectionRect(0, 0, MapWidth, MapHeight));
            switch (EditManager.Get().Mode)
            {
                case EEditMode.Tiles:
                    UpdateHighlight(rect.X, rect.Y, rect.Width, rect.Height);
                    break;
                case EEditMode.Eraser:
                    UpdateHighlight(rect.X, rect.Y, 1, 1);
                    break;
                case EEditMode.Fill:
                    UpdateHighlight(rect.X, rect.Y, 1, 1);
                    break;
            }
        }

        //---------------------------------------------------------------------------

        private void UpdateHighlight(int x, int y, int width, int height)
        {
            HighlightBorder.Margin = new Thickness(x * PxTileWidth, y * PxTileHeight, (MapWidth - width - x) * PxTileWidth, (MapHeight - height - y) * PxTileHeight);
        }

        //---------------------------------------------------------------------------

        public void SetTile(ELayerMode mode, int sourceX, int sourceY, int targetX, int targetY)
        {
            if (m_Layers.ContainsKey(mode))
            {
                m_Layers[mode].SetTile(sourceX, sourceY, targetX, targetY);
            }
        }

        //---------------------------------------------------------------------------

        public void EraseTiles(ELayerMode mode, int sourceX, int sourceY)
        {
            if (m_Layers.ContainsKey(mode))
            {
                m_Layers[mode].EraseTiles(sourceX, sourceY);
            }
        }

        //---------------------------------------------------------------------------

        public void SetBlocker(int sourceX, int sourceY, bool isBlocker)
        {
            BlockerContainer.SetBlocker(sourceX, sourceY, isBlocker);
        }

        //---------------------------------------------------------------------------

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            m_Clicked = e.GetPosition(this);
            m_Dragged = new Point(0, 0);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                UndoManager.Get().StartUndo(LayerManager.Get().Mode);
                MapManager.Get().ExecuteAction((int)m_Clicked.X / PxTileWidth, (int)m_Clicked.Y / PxTileHeight);
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            UndoManager.Get().StopUndo();
        }

        //---------------------------------------------------------------------------

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            m_Dragged = new Point(m_Dragged.X + (e.GetPosition(this).X - m_Clicked.X), m_Dragged.Y + (e.GetPosition(this).Y - m_Clicked.Y));
            m_Clicked = e.GetPosition(this);
            UpdateHighlight((int)m_Clicked.X / PxTileWidth, (int)m_Clicked.Y / PxTileHeight);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MapManager.Get().ExecuteAction((int)m_Clicked.X / PxTileWidth, (int)m_Clicked.Y / PxTileHeight);
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseLeave(object sender, EventArgs e)
        {
            UpdateHighlight(0, 0, 0, 0);
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //---------------------------------------------------------------------------

    public class TileControl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int X { get; private set; }
        public int Y { get; private set; }

        public BitmapSource Image { get; set; }
        public Rect View { get; set; }
        public bool IsBlocked { get; set; }

        //---------------------------------------------------------------------------

        public TileControl(int x, int y)
        {
            X = x;
            Y = y;
            Image = TilesetManager.Get().Tileset?.Source;
            View = new Rect();
        }

        //---------------------------------------------------------------------------

        public void SetTile(int x, int y)
        {
            View = TilesetManager.Get().Tileset.Crop(x, y);
            OnPropertyChanged("View");
        }

        //---------------------------------------------------------------------------

        public void SetBlocked(bool isBlocked)
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
