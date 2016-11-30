using Level;
using Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TilesetViewer
{
    public class MapManager : BaseManager<MapManager>
    {
        private MapControl m_Control;
        private Map m_Map;

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public int PxTileWidth { get; private set; }
        public int PxTileHeight { get; private set; }

        public bool ContainsChanges { get; private set; }

        //---------------------------------------------------------------------------

        protected MapManager() { }

        //---------------------------------------------------------------------------

        public void Create(string name, int width, int height, int tileWidth, int tileHeight, Map map = null)
        {
            MapWidth = width;
            MapHeight = height;
            PxTileWidth = tileWidth;
            PxTileHeight = tileHeight;
            m_Map = (map ?? new Map(name, width, height));
            m_Control?.Reset(MapWidth, MapHeight);

            TilesetManager.Get().UpdateTileDimension(tileWidth, tileHeight);

            if (map != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Cell cell in m_Map.Cells)
                {
                    foreach (KeyValuePair<ELayerMode, Layer> kvp in cell.Layers)
                    {
                        if (kvp.Value.TargetX >= 0 && kvp.Value.TargetY >= 0)
                        {
                            m_Control?.SetTile(kvp.Key, cell.X, cell.Y, kvp.Value.TargetX, kvp.Value.TargetY);
                        }
                    }
                    m_Control?.SetBlocker(cell.X, cell.Y, cell.IsBlocked);
                }
                Mouse.OverrideCursor = null;
            }
        }

        //---------------------------------------------------------------------------

        public void Resize(int left, int right, int top, int bottom)
        {
            MapWidth = MapWidth + left + right;
            MapHeight = MapHeight + top + bottom;
            m_Map.Resize(left, right, top, bottom);
            
            Mouse.OverrideCursor = Cursors.Wait;
            m_Control?.Resize(left, right, top, bottom);
            Mouse.OverrideCursor = null;
        }

        //---------------------------------------------------------------------------

        public void Register(MapControl control)
        {
            m_Control = control;
        }

        //---------------------------------------------------------------------------   

        public void ExecuteAction(int sourceX, int sourceY, bool isLeftMousePressed)
        {
            SelectionRect selection = TilesetManager.Get().GetSelection();
            switch (EditManager.Get().Mode)
            {
                case EEditMode.Tiles:
                    if (isLeftMousePressed) SetTile(LayerManager.Get().Mode, sourceX, sourceY, selection.X, selection.Y, selection.Width, selection.Height);
                    break;
                case EEditMode.Eraser:
                    if (isLeftMousePressed) ClearTile(LayerManager.Get().Mode, sourceX, sourceY);
                    break;
                case EEditMode.Fill:
                    if (isLeftMousePressed) FillTile(LayerManager.Get().Mode, sourceX, sourceY, selection.X, selection.Y);
                    break;
                case EEditMode.Blocker:
                    if (isLeftMousePressed) SetBlocker(sourceX, sourceY, !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)));
                    break;
            }
        }

        //---------------------------------------------------------------------------

        public void SetTile(ELayerMode mode, int sourceX, int sourceY, int targetX, int targetY, int width, int height)
        {
            if (m_Map != null)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (m_Map.SetTile(mode, sourceX + x, sourceY + y, targetX + x, targetY + y))
                        {
                            ContainsChanges = true;
                            m_Control?.SetTile(mode, sourceX + x, sourceY + y, targetX + x, targetY + y);
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void ClearTile(ELayerMode mode, int sourceX, int sourceY)
        {
            if (m_Map != null)
            {
                if (m_Map.SetTile(mode, sourceX, sourceY, -1, -1))
                {
                    ContainsChanges = true;
                    m_Control?.EraseTile(mode, sourceX, sourceY);
                }
            }
        }

        //---------------------------------------------------------------------------   

        public void FillTile(ELayerMode mode, int sourceX, int sourceY, int targetX, int targetY)
        {
            if (m_Map != null)
            {
                FloodFill(mode, new SelectionPoint(sourceX, sourceY), new SelectionPoint(targetX, targetY));
            }
        }

        //---------------------------------------------------------------------------

        public void SetBlocker(int sourceX, int sourceY, bool isBlocker)
        {
            if (m_Map != null)
            {
                if (m_Map.SetBlocker(sourceX, sourceY, isBlocker))
                {
                    ContainsChanges = true;
                    m_Control?.SetBlocker(sourceX, sourceY, isBlocker);
                }
            }
        }

        //---------------------------------------------------------------------------

        private void FloodFill(ELayerMode mode, SelectionPoint center, SelectionPoint fill)
        {
            if (IsMatch(mode, center.X, center.Y, fill.X, fill.Y)) return;

            Layer layer = m_Map[mode, center.X, center.Y];
            if (layer != null)
            {
                SelectionPoint expected = new SelectionPoint(layer.TargetX, layer.TargetY);
                Queue<SelectionPoint> q = new Queue<SelectionPoint>();
                q.Enqueue(center);
                while (q.Count > 0)
                {
                    SelectionPoint n = q.Dequeue();
                    if (!IsMatch(mode, n.X, n.Y, expected.X, expected.Y)) continue;
                    SelectionPoint w = n, e = new SelectionPoint(n.X + 1, n.Y);
                    while ((w.X >= 0) && IsMatch(mode, w.X, w.Y, expected.X, expected.Y))
                    {
                        if (m_Map.SetTile(mode, w.X, w.Y, fill.X, fill.Y))
                        {
                            ContainsChanges = true;
                            m_Control?.SetTile(mode, w.X, w.Y, fill.X, fill.Y);
                        }
                        if ((w.Y > 0) && IsMatch(mode, w.X, w.Y - 1, expected.X, expected.Y))
                            q.Enqueue(new SelectionPoint(w.X, w.Y - 1));
                        if ((w.Y < m_Map.Height - 1) && IsMatch(mode, w.X, w.Y + 1, expected.X, expected.Y))
                            q.Enqueue(new SelectionPoint(w.X, w.Y + 1));
                        w.X--;
                    }
                    while ((e.X <= m_Map.Width - 1) && IsMatch(mode, e.X, e.Y, expected.X, expected.Y))
                    {
                        if (m_Map.SetTile(mode, e.X, e.Y, fill.X, fill.Y))
                        {
                            ContainsChanges = true;
                            m_Control?.SetTile(mode, e.X, e.Y, fill.X, fill.Y);
                        }
                        if ((e.Y > 0) && IsMatch(mode, e.X, e.Y - 1, expected.X, expected.Y))
                            q.Enqueue(new SelectionPoint(e.X, e.Y - 1));
                        if ((e.Y < m_Map.Height - 1) && IsMatch(mode, e.X, e.Y + 1, expected.X, expected.Y))
                            q.Enqueue(new SelectionPoint(e.X, e.Y + 1));
                        e.X++;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private bool IsMatch(ELayerMode mode, int x, int y, int expectedX, int expectedY)
        {
            if (m_Map != null)
            {
                Layer layer = m_Map[mode, x, y];
                if (layer != null)
                {
                    return layer.TargetX == expectedX && layer.TargetY == expectedY;
                }
            }
            return false;
        }

        //---------------------------------------------------------------------------

        public Map GetMap()
        {
            return m_Map;
        }

        //---------------------------------------------------------------------------

        public void SetMap(Map map)
        {
            Create(map.Name, map.Width, map.Height, 32, 32, map);
        }

        //---------------------------------------------------------------------------

        public void SetTileset(string name)
        {
            if (m_Map != null) m_Map.Tileset = name;
        }

        //---------------------------------------------------------------------------

        public void UpdateImage()
        {
            //m_Control?.UpdateImage();
        }

        //---------------------------------------------------------------------------

        public void SaveMapMetaData()
        {
            string rootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TilesetViewer/Maps");
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }

            string thumbnailPath = Path.Combine(rootDirectory, string.Format("{0}Thumbnail.png", m_Map.Name));
            m_Control?.SaveToPng(thumbnailPath);
            
            MapMetaData data = new MapMetaData(thumbnailPath, m_Map.Name, "");
            string dataPath = Path.Combine(rootDirectory, string.Format("{0}.data", m_Map.Name));
            try
            {
                using (FileStream stream = new FileStream(@dataPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(JsonConvert.SerializeObject(data, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        //---------------------------------------------------------------------------

        public MessageBoxResult CheckForChanges()
        {
            if (m_Map != null && ContainsChanges)
            {
                return MessageBox.Show("Do you want to save all changes?", "Unsaved changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            }
            return MessageBoxResult.None;
        }
    }
}
