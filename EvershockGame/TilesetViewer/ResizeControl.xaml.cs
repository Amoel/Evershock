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
    public enum EResizeMode
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    //---------------------------------------------------------------------------

    public partial class ResizeControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public float LayerOpacity { get; set; }
        public Thickness LayerMargin { get; set; }
        public Brush LayerColor { get; set; }

        public Thickness LeftBorderMargin { get; set; }
        public Thickness RightBorderMargin { get; set; }
        public Thickness TopBorderMargin { get; set; }
        public Thickness BottomBorderMargin { get; set; }

        public int MapWidth { get { return MapManager.Get().MapWidth; } }
        public int MapHeight { get { return MapManager.Get().MapHeight; } }

        public int PxTileWidth { get { return MapManager.Get().PxTileWidth; } }
        public int PxTileHeight { get { return MapManager.Get().PxTileHeight; } }

        public int MapMinWidth { get { return 3; } }
        public int MapMinHeight { get { return 3; } }

        private bool m_IsDrag;
        private Point m_Drag;
        private Point m_Click;

        private SelectionPoint m_Size;
        private EResizeMode m_Mode;

        Dictionary<FrameworkElement, EResizeMode> m_ModeMapping;

        //---------------------------------------------------------------------------

        public ResizeControl()
        {
            InitializeComponent();
            DataContext = this;

            m_Size = new SelectionPoint();
            m_Mode = EResizeMode.None;

            m_ModeMapping = new Dictionary<FrameworkElement, EResizeMode>()
            {
                { LeftBorder, EResizeMode.Left },
                { RightBorder, EResizeMode.Right },
                { TopBorder, EResizeMode.Top },
                { BottomBorder, EResizeMode.Bottom }
            };

            UpdateLeftBorderMargin(0);
            UpdateRightBorderMargin(0);
            UpdateTopBorderMargin(0);
            UpdateBottomBorderMargin(0);
        }

        //---------------------------------------------------------------------------

        private void UpdateResizeMode(FrameworkElement element)
        {
            if (m_ModeMapping.ContainsKey(element))
            {
                m_Mode = m_ModeMapping[element];
            }
            else
            {
                m_Mode = EResizeMode.None;
            }
        }

        //---------------------------------------------------------------------------

        private void UpdateDrag(bool drag)
        {
            m_IsDrag = drag;
            LayerOpacity = (m_IsDrag ? 0.4f : 0.0f);
            OnPropertyChanged("LayerOpacity");
        }

        //---------------------------------------------------------------------------

        private void UpdateLeftBorderMargin(int value)
        {
            LeftBorderMargin = new Thickness(value * PxTileWidth - 3, 0, 0, 0);
            OnPropertyChanged("LeftBorderMargin");

            if (value < 0)
            {
                LayerMargin = new Thickness(value * PxTileWidth, 0, MapWidth * PxTileWidth, 0);
            }
            else
            {
                LayerMargin = new Thickness(0, 0, (MapWidth * PxTileWidth) - (value * PxTileWidth), 0);
            }
            OnPropertyChanged("LayerMargin");
            LayerColor = (value < 0 ? Brushes.Green : Brushes.Red);
            OnPropertyChanged("LayerColor");
        }

        //---------------------------------------------------------------------------

        private void UpdateRightBorderMargin(int value)
        {
            RightBorderMargin = new Thickness(0, 0, -(value * PxTileWidth + 3), 0);
            OnPropertyChanged("RightBorderMargin");

            if (value > 0)
            {
                LayerMargin = new Thickness(MapWidth * PxTileWidth, 0, -value * PxTileWidth, 0);
            }
            else
            {
                LayerMargin = new Thickness((MapWidth * PxTileWidth) + (value * PxTileWidth), 0, 0, 0);
            }
            OnPropertyChanged("LayerMargin");
            LayerColor = (value > 0 ? Brushes.Green : Brushes.Red);
            OnPropertyChanged("LayerColor");
        }

        //---------------------------------------------------------------------------

        private void UpdateTopBorderMargin(int value)
        {
            TopBorderMargin = new Thickness(0, value * PxTileHeight - 3, 0, 0);
            OnPropertyChanged("TopBorderMargin");

            if (value < 0)
            {
                LayerMargin = new Thickness(0, value * PxTileHeight, 0, MapHeight * PxTileHeight);
            }
            else
            {
                LayerMargin = new Thickness(0, 0, 0, (MapHeight * PxTileHeight) - (value * PxTileHeight));
            }
            OnPropertyChanged("LayerMargin");
            LayerColor = (value < 0 ? Brushes.Green : Brushes.Red);
            OnPropertyChanged("LayerColor");
        }

        //---------------------------------------------------------------------------

        private void UpdateBottomBorderMargin(int value)
        {
            BottomBorderMargin = new Thickness(0, 0, 0, -(value * PxTileHeight + 3));
            OnPropertyChanged("BottomBorderMargin");

            if (value > 0)
            {
                LayerMargin = new Thickness(0, MapHeight * PxTileHeight, 0, -value * PxTileHeight);
            }
            else
            {
                LayerMargin = new Thickness(0, (MapHeight * PxTileHeight) + (value * PxTileHeight), 0, 0);
            }
            OnPropertyChanged("LayerMargin");
            LayerColor = (value > 0 ? Brushes.Green : Brushes.Red);
            OnPropertyChanged("LayerColor");
        }

        //---------------------------------------------------------------------------

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_Click = e.GetPosition(Root);
                m_Drag = new Point(0, 0);

                UpdateResizeMode((FrameworkElement)sender);
                Mouse.Capture((IInputElement)sender);
                UpdateDrag(true);
                switch (m_Mode)
                {
                    case EResizeMode.Left:
                        UpdateLeftBorderMargin(0);
                        break;
                    case EResizeMode.Right:
                        UpdateRightBorderMargin(0);
                        break;
                    case EResizeMode.Top:
                        UpdateTopBorderMargin(0);
                        break;
                    case EResizeMode.Bottom:
                        UpdateBottomBorderMargin(0);
                        break;
                }
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsDrag)
            {
                m_Drag = new Point(e.GetPosition(Root).X - m_Click.X, e.GetPosition(Root).Y - m_Click.Y);
                switch (m_Mode)
                {
                    case EResizeMode.Left:
                        m_Size = new SelectionPoint(Math.Min(MapWidth - MapMinWidth, (int)m_Drag.X / PxTileWidth), (int)m_Drag.Y / PxTileHeight);
                        UpdateLeftBorderMargin(m_Size.X);
                        break;
                    case EResizeMode.Right:
                        m_Size = new SelectionPoint(Math.Max(MapMinWidth - MapWidth, (int)m_Drag.X / PxTileWidth), (int)m_Drag.Y / PxTileHeight);
                        UpdateRightBorderMargin(m_Size.X);
                        break;
                    case EResizeMode.Top:
                        m_Size = new SelectionPoint((int)m_Drag.X / PxTileWidth, Math.Min(MapHeight - MapMinHeight, (int)m_Drag.Y / PxTileHeight));
                        UpdateTopBorderMargin(m_Size.Y);
                        break;
                    case EResizeMode.Bottom:
                        m_Size = new SelectionPoint((int)m_Drag.X / PxTileWidth, Math.Max(MapMinHeight - MapHeight, (int)m_Drag.Y / PxTileHeight));
                        UpdateBottomBorderMargin(m_Size.Y);
                        break;
                }
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
            UpdateDrag(false);

            if (m_Size != null && (m_Size.X != 0 || m_Size.Y != 0))
            {
                switch (m_Mode)
                {
                    case EResizeMode.Left: MapManager.Get().Resize(-m_Size.X, 0, 0, 0); break;
                    case EResizeMode.Right: MapManager.Get().Resize(0, m_Size.X, 0, 0); break;
                    case EResizeMode.Top: MapManager.Get().Resize(0, 0, -m_Size.Y, 0); break;
                    case EResizeMode.Bottom: MapManager.Get().Resize(0, 0, 0, m_Size.Y); break;
                }
            }
            UpdateLeftBorderMargin(0);
            UpdateRightBorderMargin(0);
            UpdateTopBorderMargin(0);
            UpdateBottomBorderMargin(0);
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
