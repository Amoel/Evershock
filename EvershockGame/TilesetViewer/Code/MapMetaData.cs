using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    [Serializable]
    public class MapMetaData
    {
        public string ThumbnailPath { get; set; }
        public string MapName { get; set; }
        public string MapPath { get; set; }

        //---------------------------------------------------------------------------

        public MapMetaData() { }

        //---------------------------------------------------------------------------

        public MapMetaData(string thumbnailPath, string mapName, string mapPath)
        {
            ThumbnailPath = thumbnailPath;
            MapName = mapName;
            MapPath = mapPath;
        }
    }
}
