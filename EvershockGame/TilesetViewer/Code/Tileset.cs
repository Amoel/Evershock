using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TilesetViewer
{
    public class Tileset
    {
        public string Name { get; set; }
        public BitmapImage Source { get; set; }
        public int PxTileWidth { get; set; }
        public int PxTileHeight { get; set; }

        public int Width { get { return Source != null ? Source.PixelWidth / PxTileWidth : 0; } }
        public int Height { get { return Source != null ? Source.PixelHeight / PxTileHeight : 0; } }
        public int PxWidth { get { return Source != null ? Source.PixelWidth : 0; } }
        public int PxHeight { get { return Source != null ? Source.PixelHeight : 0; } }

        public SelectionRect Bounds { get { return new SelectionRect(0, 0, Width, Height); } }

        //---------------------------------------------------------------------------

        public Tileset(string name, BitmapImage source, int pxTileWidth, int pxTileHeight)
        {
            Update(name, source, pxTileWidth, pxTileHeight);
        }

        //---------------------------------------------------------------------------

        public void Update(BitmapImage source)
        {
            Update(Name, source, PxTileWidth, PxTileHeight);
        }

        //---------------------------------------------------------------------------

        public void Update(int pxTileWidth, int pxTileHeight)
        {
            Update(Name, Source, pxTileWidth, pxTileHeight);
        }

        //---------------------------------------------------------------------------

        public void Update(string name, BitmapImage source, int pxTileWidth, int pxTileHeight)
        {
            Name = name;
            Source = source;
            PxTileWidth = pxTileWidth;
            PxTileHeight = pxTileHeight;
        }

        //---------------------------------------------------------------------------

        public Rect Crop(int x, int y)
        {
            return new Rect((x * PxTileWidth) / (float)PxWidth, (y * PxTileHeight) / (float)PxHeight, PxTileWidth / (float)PxWidth, PxTileHeight / (float)PxHeight);
        }

        //---------------------------------------------------------------------------

        public Rect Crop(int value)
        {
            return Crop(value % Width, value / Width);
        }
    }
}
