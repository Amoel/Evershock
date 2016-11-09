using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    [Serializable]
    public class Map
    {
        public int[,] Data { get; set; }

        //---------------------------------------------------------------------------

        public Map() { }

        //---------------------------------------------------------------------------

        public Map(Tile[,] tiles)
        {

        }

        //---------------------------------------------------------------------------

        public Map(int[,] data)
        {
            Data = data;
        }
    }
}
