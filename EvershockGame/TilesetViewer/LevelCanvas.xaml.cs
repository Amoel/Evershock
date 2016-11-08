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
    /// Interaction logic for LevelCanvas.xaml
    /// </summary>
    public partial class LevelCanvas : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Rect m_CanvasViewport = new Rect(0, 0, 16, 16);
        public Rect CanvasViewport
        {
            get { return m_CanvasViewport; }
            set { m_CanvasViewport = value; OnPropertyChanged("CanvasViewport"); }
        }

        public SelectionRect Bounds { get; set; }
        public SelectionRect Highlight { get; set; }

        private Tile[,] m_Tiles;
        private bool m_Dragging;
        private Point m_LastClicked;
        private Point m_LastMoved;

        private int m_TileWidth = 16;
        private int m_TileHeight = 16;

        //---------------------------------------------------------------------------

        public LevelCanvas()
        {
            InitializeComponent();
            DataContext = this;

            LevelManager.Get().RegisterCanvas(this);
        }

        //---------------------------------------------------------------------------

        public void Create(int width, int height)
        {
            TilesCanvas.Children.Clear();

            m_Tiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = new Tile();

                    TilesCanvas.Children.Add(tile);
                    tile.Init(x, y, 0);
                    m_Tiles[x, y] = tile;
                }
            }
            BorderRect.Width = width * m_TileWidth;
            BorderRect.Height = height * m_TileHeight;
            Bounds = new SelectionRect(0, 0, width, height);
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(Point point, bool isHovering)
        {
            if (isHovering)
            {
                int beginX = (int)(point.X / m_TileWidth);
                int beginY = (int)(point.Y / m_TileHeight);
                SelectionRect selection = TilesetManager.Get().GetSelection();
                SetHighlight(beginX, beginY, selection != null ? selection.Width : 1, selection != null ? selection.Height : 1);
            }
            else
            {
                SetHighlight(0, 0, 0, 0);
            }
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(int x, int y, int width, int height)
        {
            if (Highlight == null)
            {
                Highlight = new SelectionRect(x, y, width, height, Bounds);
            }
            else
            {
                Highlight.Update(x, y, width, height, Bounds);
            }

            Canvas.SetLeft(HighlightRect, Highlight.X * m_TileWidth);
            Canvas.SetTop(HighlightRect, Highlight.Y * m_TileHeight);
            HighlightRect.Width = Highlight.Width * m_TileWidth;
            HighlightRect.Height = Highlight.Height * m_TileHeight;

            OnPropertyChanged("Highlight");
        }

        //---------------------------------------------------------------------------

        private void PasteTiles(Point point)
        {
            int x = Math.Max(0, Math.Min(m_Tiles.GetLength(0) - 1, (int)point.X / m_TileWidth));
            int y = Math.Max(0, Math.Min(m_Tiles.GetLength(1) - 1, (int)point.Y / m_TileHeight));
            PasteTiles(x, y);
        }

        //---------------------------------------------------------------------------

        private void PasteTiles(int x, int y)
        {
            SelectionRect rect = TilesetManager.Get().GetSelection();
            if (rect != null)
            {
                for (int rectX = 0; rectX < rect.Width; rectX++)
                {
                    for (int rectY = 0; rectY < rect.Height; rectY++)
                    {
                        int posX = x + rectX;
                        int posY = y + rectY;
                        if (posX < 0 || posX >= Bounds.Width || posY < 0 || posY >= Bounds.Height) continue;

                        m_Tiles[posX, posY].Update(rect.X + rectX, rect.Y + rectY);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            m_LastClicked = e.GetPosition(border);
            m_LastMoved = m_LastClicked;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                PasteTiles(m_LastClicked);
            }
            m_Dragging = true;
            border.CaptureMouse();
        }

        //---------------------------------------------------------------------------

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            m_Dragging = false;
            border.ReleaseMouseCapture();
        }

        //---------------------------------------------------------------------------

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                double x = (e.GetPosition(border).X - m_LastMoved.X);
                double y = (e.GetPosition(border).Y - m_LastMoved.Y);
                TilesTranslate.X += x;
                TilesTranslate.Y += y;
                SelectionTranslate.X += x;
                SelectionTranslate.Y += y;
            }
            else
            {
                SetHighlight(e.GetPosition(border), IsMouseOver);

                if (m_Dragging && e.LeftButton == MouseButtonState.Pressed)
                {
                    PasteTiles(e.GetPosition(border));
                }
            }
            m_LastMoved = e.GetPosition(border);
        }

        //---------------------------------------------------------------------------

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                TilesTranslate.X = 0;
                TilesTranslate.Y = 0;
                SelectionTranslate.X = 0;
                SelectionTranslate.Y = 0;
            }
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
