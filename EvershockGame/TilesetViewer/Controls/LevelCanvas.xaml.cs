using Level;
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
        
        private bool m_Dragging;
        private Point m_LastClicked;
        private Point m_LastMoved;

        private Point m_Offset = new Point(0, 0);

        private int m_TileWidth = 16;
        private int m_TileHeight = 16;

        //---------------------------------------------------------------------------

        public LevelCanvas()
        {
            InitializeComponent();
            DataContext = this;

            LevelManager.Get().RegisterCanvas(this);

            EditManager.Get().Register(EEditMode.Tiles, TilesEditMode);
            EditManager.Get().Register(EEditMode.Eraser, EraserEditMode);
            EditManager.Get().Register(EEditMode.Selection, SelectionEditMode);
            EditManager.Get().Register(EEditMode.Fill, FillEditMode);
            EditManager.Get().Register(EEditMode.Blocker, BlockerEditMode);

            EditManager.Get().ChangeMode(EEditMode.Tiles);

            LayerManager.Get().Register(ELayerMode.First, FirstLayer);
            LayerManager.Get().Register(ELayerMode.Second, SecondLayer);
            LayerManager.Get().Register(ELayerMode.Third, ThirdLayer);

            LayerManager.Get().ChangeMode(ELayerMode.First);

            #region Keybindings
            InputManager.Get().RegisterShortcut(Key.Z, ModifierKeys.Control, () =>
            {
                UndoManager.Get().Undo();
            });

            InputManager.Get().RegisterShortcut(Key.Y, ModifierKeys.Control, () =>
            {
                UndoManager.Get().Redo();
            });

            InputManager.Get().RegisterShortcut(Key.T, ModifierKeys.Control, () =>
            {
                EditManager.Get().ChangeMode(EEditMode.Tiles);
            });

            InputManager.Get().RegisterShortcut(Key.E, ModifierKeys.Control, () =>
            {
                EditManager.Get().ChangeMode(EEditMode.Eraser);
            });

            InputManager.Get().RegisterShortcut(Key.S, ModifierKeys.Control, () =>
            {
                EditManager.Get().ChangeMode(EEditMode.Selection);
            });

            InputManager.Get().RegisterShortcut(Key.F, ModifierKeys.Control, () =>
            {
                EditManager.Get().ChangeMode(EEditMode.Fill);
            });

            InputManager.Get().RegisterShortcut(Key.B, ModifierKeys.Control, () =>
            {
                EditManager.Get().ChangeMode(EEditMode.Blocker);
            });

            InputManager.Get().RegisterShortcut(Key.F1, ModifierKeys.None, () =>
            {
                LayerManager.Get().ChangeMode(ELayerMode.First);
            });

            InputManager.Get().RegisterShortcut(Key.F2, ModifierKeys.None, () =>
            {
                LayerManager.Get().ChangeMode(ELayerMode.Second);
            });

            InputManager.Get().RegisterShortcut(Key.F3, ModifierKeys.None, () =>
            {
                LayerManager.Get().ChangeMode(ELayerMode.Third);
            });
            #endregion
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(Point point, bool isHovering)
        {
            if (isHovering)
            {
                int beginX = (int)(point.X / m_TileWidth);
                int beginY = (int)(point.Y / m_TileHeight);

                switch (EditManager.Get().Mode)
                {
                    case EEditMode.Tiles:
                        SelectionRect selection = TilesetManager.Get().GetSelection();
                        SetHighlight(beginX, beginY, selection != null ? selection.Width : 1, selection != null ? selection.Height : 1, true);
                        break;
                    case EEditMode.Blocker:
                        SetHighlight(beginX, beginY, 1, 1, true);
                        break;
                }
            }
            else
            {
                SetHighlight(0, 0, 0, 0, false);
            }
        }

        //---------------------------------------------------------------------------

        private void SetHighlight(int x, int y, int width, int height, bool checkBounds)
        {
            if (Highlight == null)
            {
                Highlight = new SelectionRect(x, y, width, height, checkBounds ? Bounds : null);
            }
            else
            {
                Highlight.Update(x, y, width, height, checkBounds ? Bounds : null);
            }
            OnPropertyChanged("Highlight");
        }

        //---------------------------------------------------------------------------

        float zoom = 1.0f;
        private void Zoom(float delta)
        {
            float newZoom = Math.Max(0.2f, Math.Min(1.0f, zoom + delta));
            if (newZoom == zoom) return;

            double xDist = (TilesContainer.ActualWidth * (1.0 / newZoom)) / 2.0 - TilesCanvas.ActualWidth / 2.0;
            double yDist = (TilesContainer.ActualHeight * (1.0 / newZoom)) / 2.0 - TilesCanvas.ActualHeight / 2.0;

            UpdateOffset(xDist, yDist);

            TilesScale.ScaleX = newZoom;
            TilesScale.ScaleY = newZoom;

            MapSizeConverter.UpdateZoom(newZoom);
            BindingOperations.GetBindingExpression(TilesCanvas, WidthProperty).UpdateTarget();
            BindingOperations.GetBindingExpression(TilesCanvas, HeightProperty).UpdateTarget();

            zoom = newZoom;
        }

        //---------------------------------------------------------------------------

        private void UpdateOffset(double x, double y)
        {
            m_Offset = new Point(m_Offset.X + x, m_Offset.Y + y);
            Canvas.SetLeft(TilesMap, m_Offset.X);
            Canvas.SetTop(TilesMap, m_Offset.Y);
        }

        //---------------------------------------------------------------------------

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            UndoManager.Get().StartUndo(LayerManager.Get().Mode);

            var pos = e.GetPosition(TilesCanvas);
            m_LastClicked = pos;
            m_LastMoved = m_LastClicked;
            m_Dragging = true;
        }

        //---------------------------------------------------------------------------

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            UndoManager.Get().StopUndo();
        }

        //---------------------------------------------------------------------------

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(TilesCanvas);
            if (e.RightButton == MouseButtonState.Pressed)
            {
                UpdateOffset(e.GetPosition(TilesCanvas).X - m_LastMoved.X, e.GetPosition(TilesCanvas).Y - m_LastMoved.Y);

                //double x = (pos.X - m_LastMoved.X);
                //double y = (pos.Y - m_LastMoved.Y);
                //TilesTranslate.X += x;
                //TilesTranslate.Y += y;
                //SelectionTranslate.X += x;
                //SelectionTranslate.Y += y;
            }

            //switch (EditManager.Get().Mode)
            //{
            //    case EEditMode.Tiles:
            //        if (e.RightButton == MouseButtonState.Pressed)
            //        {
            //            double x = (pos.X - m_LastMoved.X);
            //            double y = (pos.Y - m_LastMoved.Y);
            //            TilesTranslate.X += x;
            //            TilesTranslate.Y += y;
            //            SelectionTranslate.X += x;
            //            SelectionTranslate.Y += y;
            //        }
            //        else
            //        {
            //            if (m_Dragging && e.LeftButton == MouseButtonState.Pressed)
            //            {
            //                PasteTiles(pos);
            //            }
            //        }
            //        break;
            //    case EEditMode.Blocker:
            //        if (e.LeftButton == MouseButtonState.Pressed)
            //        {
            //            SetBlocker(pos, false);
            //        }
            //        else if (e.RightButton == MouseButtonState.Pressed)
            //        {
            //            SetBlocker(pos, true);
            //        }
            //        break;
            //}
            m_LastMoved = pos;
        }

        //---------------------------------------------------------------------------

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Zoom(0.1f);
            }
            else if (e.Delta < 0)
            {
                Zoom(-0.1f);
            }
        }

        //---------------------------------------------------------------------------

        private void OnDrop(object sender, DragEventArgs e)
        {

        }

        //---------------------------------------------------------------------------

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            SetHighlight(0, 0, 0, 0, false);
        }

        //---------------------------------------------------------------------------

        private void OnUndoClicked(object sender, EventArgs e)
        {
            UndoManager.Get().Undo();
        }

        //---------------------------------------------------------------------------

        private void OnRedoClicked(object sender, EventArgs e)
        {
            UndoManager.Get().Redo();
        }

        //---------------------------------------------------------------------------

        private void OnZoomInClicked(object sender, EventArgs e)
        {
            ZoomManager.Get().Zoom(0.1f);
            MapManager.Get().SaveMapMetaData();
        }

        //---------------------------------------------------------------------------

        private void OnZoomOutClicked(object sender, EventArgs e)
        {
            ZoomManager.Get().Zoom(-0.1f);
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
