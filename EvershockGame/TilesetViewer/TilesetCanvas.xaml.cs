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
    /// Interaction logic for TilesetCanvas.xaml
    /// </summary>
    public partial class TilesetCanvas : UserControl, INotifyPropertyChanged
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
        public SelectionRect Selection { get; set; }
        
        private bool m_Dragging;
        private Point m_LastClicked;
        private Point m_LastMoved;

        //---------------------------------------------------------------------------

        public TilesetCanvas()
        {
            InitializeComponent();
            DataContext = this;

            TilesetManager.Get().RegisterCanvas(this);
        }

        //---------------------------------------------------------------------------

        public void Update()
        {
            Tileset.Source = TilesetManager.Get().Tileset.Source;
            Tileset.Width = TilesetManager.Get().Tileset.PxWidth;
            Tileset.Height = TilesetManager.Get().Tileset.PxHeight;
            Bounds = TilesetManager.Get().Tileset.Bounds;
        }

        //---------------------------------------------------------------------------

        private void SetSelection(Point start, Point end)
        {
            int beginX = (int)Math.Max(0, (start.X / 16));
            int beginY = (int)Math.Max(0, (start.Y / 16));
            int endX = (int)Math.Max(0, (end.X / 16));
            int endY = (int)Math.Max(0, (end.Y / 16));
            SetSelection(Math.Min(beginX, endX), Math.Min(beginY, endY), Math.Max(1, Math.Abs(endX - beginX) + 1), Math.Max(1, Math.Abs(endY - beginY) + 1));
        }

        //---------------------------------------------------------------------------

        private void SetSelection(int x, int y, int width, int height)
        {
            if (Selection == null)
            {
                Selection = new SelectionRect(x, y, width, height, Bounds);
            }
            else
            {
                Selection.Update(x, y, width, height, Bounds);
            }

            Canvas.SetLeft(SelectionRect, Selection.X * 16);
            Canvas.SetTop(SelectionRect, Selection.Y * 16);
            SelectionRect.Width = Selection.Width * 16;
            SelectionRect.Height = Selection.Height * 16;

            OnPropertyChanged("Selection");
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(Point point)
        {
            int beginX = (int)(point.X / 16);
            int beginY = (int)(point.Y / 16);
            SetHighlight(beginX, beginY);
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(int x, int y)
        {
            if (Highlight == null)
            {
                Highlight = new SelectionRect(x, y, 1, 1, Bounds);
            }
            else
            {
                Highlight.Update(x, y, 1, 1, Bounds);
            }

            Canvas.SetLeft(HighlightRect, Highlight.X * 16);
            Canvas.SetTop(HighlightRect, Highlight.Y * 16);

            OnPropertyChanged("Highlight");
        }

        //---------------------------------------------------------------------------

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            m_LastClicked = e.GetPosition(image);
            m_LastMoved = m_LastClicked;
            m_Dragging = true;
            image.CaptureMouse();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetSelection(m_LastClicked, e.GetPosition(image));
            }
        }

        //---------------------------------------------------------------------------

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;
            m_Dragging = false;
            image.ReleaseMouseCapture();
        }

        //---------------------------------------------------------------------------

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Image image = (Image)sender;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                TilesTranslate.X += (e.GetPosition(image).X - m_LastMoved.X);
                TilesTranslate.Y += (e.GetPosition(image).Y - m_LastMoved.Y);
            }
            else
            {
                SetHighlight(e.GetPosition(image));

                if (m_Dragging && e.LeftButton == MouseButtonState.Pressed)
                {
                    SetSelection(m_LastClicked, e.GetPosition(image));
                }
            }
            m_LastMoved = e.GetPosition(image);
        }

        //---------------------------------------------------------------------------

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                TilesTranslate.X = 0;
                TilesTranslate.Y = 0;
            }
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
