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
    public class TextControl : UIEntity
    {
        public TextComponent Properties { get { return GetComponent<TextComponent>(); } }

        //---------------------------------------------------------------------------

        public TextControl(string name, Guid parent, Frame frame) : base(name, parent, frame)
        {
            AddComponent<TextComponent>();
        }
    }
}
