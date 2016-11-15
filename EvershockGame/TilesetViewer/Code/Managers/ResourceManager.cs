using Level;
using Managers;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TilesetViewer
{
    public class ResourceManager : BaseManager<ResourceManager>
    {
        protected ResourceManager() { }

        //---------------------------------------------------------------------------

        public void Save(Map map)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Map file (*.map)|*.map";

            if (dialog.ShowDialog() == true)
            {
                Map.Save(map, dialog.FileName);
            }
        }

        //---------------------------------------------------------------------------

        public Map Load()
        {
            Map map = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Map file (*.map)|*.map";

            if (dialog.ShowDialog() == true)
            {
                map = Map.Load(dialog.FileName);
            }
            return map;
        }
    }
}
