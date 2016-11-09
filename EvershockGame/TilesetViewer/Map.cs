using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TilesetViewer
{
    [Serializable]
    public class Map
    {
        public Cell[,] Data { get; set; }

        //---------------------------------------------------------------------------

        public Map() { }

        //---------------------------------------------------------------------------

        public Map(Tile[,] tiles)
        {
            Data = new Cell[tiles.GetLength(0), tiles.GetLength(1)];
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Data[x, y] = new Cell()
                    {
                        View = tiles[x, y].View,
                        IsBlocked = tiles[x, y].IsBlocked
                    };
                }
            }
        }
    }

    //---------------------------------------------------------------------------

    [Serializable]
    public class Cell
    {
        public Rect View { get; set; }
        public bool IsBlocked { get; set; }

        //---------------------------------------------------------------------------

        public Cell() { }
    }
}
