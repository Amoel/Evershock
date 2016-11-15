using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level
{
    [Serializable]
    public class Map
    {
        public string Tileset { get; set; }
        public Cell[,] Data { get; set; }

        [JsonIgnore]
        public int Width { get { return Data != null ? Data.GetLength(0) : 0; } }
        [JsonIgnore]
        public int Height { get { return Data != null ? Data.GetLength(1) : 0; } }

        //---------------------------------------------------------------------------

        public Map() { }

        //---------------------------------------------------------------------------

        public Map(int width, int height)
        {
            Data = new Cell[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Data[x, y] = new Cell(x, y);
                }
            }
        }

        //---------------------------------------------------------------------------

        public bool SetTile(int sourceX, int sourceY, int targetX, int targetY)
        {
            if (sourceX < 0 || sourceX >= Width || sourceY < 0 || sourceY >= Height) return false;

            //Cell cell = Data[sourceX, sourceY];
            //if (cell.X == targetX && cell.Y == targetY) return false;
            //cell.X = targetX;
            //cell.Y = targetY;
            return true;
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
        public ViewRect ViewFirstLayer { get; set; }
        public ViewRect ViewSecondLayer { get; set; }
        public ViewRect ViewThirdLayer { get; set; }
        public bool IsBlocked { get; set; }

        //---------------------------------------------------------------------------

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
