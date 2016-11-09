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
                try
                {
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
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
        }

        //---------------------------------------------------------------------------

        public Map Load()
        {
            Map map = null;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Map file (*.map)|*.map";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open))
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
            }
            return map;
        }
    }
}
