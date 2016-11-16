﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level
{
    public enum ELayerMode
    {
        None,
        First,
        Second,
        Third
    }

    //---------------------------------------------------------------------------

    [Serializable]
    public class Map
    {
        public string Tileset { get; set; }
        public Cell[,] Cells { get; set; }

        [JsonIgnore]
        public int Width { get { return Cells != null ? Cells.GetLength(0) : 0; } }
        [JsonIgnore]
        public int Height { get { return Cells != null ? Cells.GetLength(1) : 0; } }

        //---------------------------------------------------------------------------

        public Map() { }

        //---------------------------------------------------------------------------

        public Map(int width, int height)
        {
            Cells = new Cell[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cells[x, y] = new Cell(x, y, false);
                }
            }
        }

        //---------------------------------------------------------------------------

        public Cell this[int x, int y]
        {
            get { return (x >= 0 && x < Width && y >= 0 && y < Height) ? Cells[x, y] : null; }
        }

        //---------------------------------------------------------------------------

        public Layer this[ELayerMode mode, int x, int y]
        {
            get
            {
                Cell cell = this[x, y];
                return cell != null ? cell[mode] : null;
            }
        }

        //---------------------------------------------------------------------------

        public bool SetTile(ELayerMode mode, int sourceX, int sourceY, int targetX, int targetY)
        {
            if (sourceX < 0 || sourceX >= Width || sourceY < 0 || sourceY >= Height) return false;
            Layer layer = Cells[sourceX, sourceY][mode];
            if (layer != null)
            {
                if (layer.TargetX != targetX || layer.TargetY != targetY)
                {
                    layer.TargetX = targetX;
                    layer.TargetY = targetY;
                    return true;
                }
            }
            return false;
        }

        //---------------------------------------------------------------------------

        public static void Save(Map map, string path)
        {
            try
            {
                using (FileStream stream = new FileStream(@path, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(JsonConvert.SerializeObject(map, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        //---------------------------------------------------------------------------

        public static Map Load(string path)
        {
            Map map = null;
            try
            {
                using (FileStream stream = new FileStream(@path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        map = JsonConvert.DeserializeObject<Map>(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {

            }
            return map;
        }
    }

    //---------------------------------------------------------------------------

    [Serializable]
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<ELayerMode, Layer> Layers { get; set; }

        public ViewRect ViewFirstLayer { get; set; }
        public ViewRect ViewSecondLayer { get; set; }
        public ViewRect ViewThirdLayer { get; set; }
        public bool IsBlocked { get; set; }

        //---------------------------------------------------------------------------

        public Cell(int x, int y, bool isBlocked)
        {
            X = x;
            Y = y;
            Layers = new Dictionary<ELayerMode, Layer>();
            foreach (ELayerMode mode in Enum.GetValues(typeof(ELayerMode)))
            {
                Layers.Add(mode, new Layer(-1, -1));
            }
            IsBlocked = isBlocked;
        }

        //---------------------------------------------------------------------------

        public Layer this[ELayerMode mode]
        {
            get { return Layers.ContainsKey(mode) ? Layers[mode] : null; }
        }
    }

    //---------------------------------------------------------------------------

    public class Layer
    {
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        //---------------------------------------------------------------------------

        public Layer(int targetX, int targetY)
        {
            TargetX = targetX;
            TargetY = targetY;
        }
    }
}
