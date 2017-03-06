using EvershockGame.Code.Components.UI;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Entities
{
    public class ImageControl : UIEntity
    {
        public Texture2D Image
        {
            get { return GetComponent<ImageComponent>().Image; }
            set { GetComponent<ImageComponent>().Image = value; }
        }

        //---------------------------------------------------------------------------

        public ImageControl(string name, Guid parent, Frame frame) : base(name, parent, frame)
        {
            AddComponent<ImageComponent>();
        }
    }
}
