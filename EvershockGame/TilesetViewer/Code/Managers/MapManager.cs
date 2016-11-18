using Level;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class MapManager : BaseManager<MapManager>
    {
        private MapControl m_Control;
        private Map m_Map;

        public int PxTileWidth { get; private set; }
        public int PxTileHeight { get; private set; }

        //---------------------------------------------------------------------------

        protected MapManager() { }

        //---------------------------------------------------------------------------

        public void Create(int width, int height, int tileWidth, int tileHeight, Map map = null)
        {
            PxTileWidth = tileWidth;
            PxTileHeight = tileHeight;
            m_Map = (map ?? new Map(width, height));
            m_Control?.Reset(width, height);

            TilesetManager.Get().UpdateTileDimension(tileWidth, tileHeight);

            if (map != null)
            {
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
            }
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
                    SetTile(LayerManager.Get().Mode, sourceX, sourceY, selection.X, selection.Y, selection.Width, selection.Height);
                    break;
                case EEditMode.Eraser:
                    ClearTile(LayerManager.Get().Mode, sourceX, sourceY);
                    break;
                case EEditMode.Fill:
                    FillTile(LayerManager.Get().Mode, sourceX, sourceY, selection.X, selection.Y);
                    break;
                case EEditMode.Blocker:
                    SetBlocker(sourceX, sourceY, true);
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
                    m_Control?.EraseTiles(mode, sourceX, sourceY);
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
            Create(map.Width, map.Height, 32, 32, map);
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
    }
}
