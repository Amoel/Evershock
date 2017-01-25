using EntityComponent.Components.UI;
using EntityComponent.Manager;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Entities
{
    public class TextControl : UIEntity
    {
        public TextComponent Properties { get { return GetComponent<TextComponent>(); } }

        //---------------------------------------------------------------------------

        public TextControl(string name, Frame frame) : base(name, frame)
        {
            AddComponent<TextComponent>();
        }
    }
}
