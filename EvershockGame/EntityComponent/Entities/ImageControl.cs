using EntityComponent.Components.UI;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Entities
{
    public class ImageControl : UIEntity
    {
        public Texture2D Image
        {
            get { return GetComponent<ImageComponent>().Image; }
            set { GetComponent<ImageComponent>().Image = value; }
        }

        //---------------------------------------------------------------------------

        public ImageControl(string name) : base(name)
        {
            AddComponent<ImageComponent>();
        }
    }
}
