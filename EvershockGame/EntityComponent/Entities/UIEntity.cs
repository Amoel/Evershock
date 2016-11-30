using EntityComponent.Components.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Entities
{
    public abstract class UIEntity : Entity
    {
        public Point Size
        {
            get { return GetComponent<UITransformComponent>().Size; }
            set { GetComponent<UITransformComponent>().Size = value; }
        }

        public EVerticalAlignment VerticalAlignment
        {
            get { return GetComponent<UITransformComponent>().VerticalAlignment; }
            set { GetComponent<UITransformComponent>().VerticalAlignment = value; }
        }

        public EHorizontalAlignment HorizontalAlignment
        {
            get { return GetComponent<UITransformComponent>().HorizontalAlignment; }
            set { GetComponent<UITransformComponent>().HorizontalAlignment = value; }
        }

        public Rectangle Margin
        {
            get { return GetComponent<UITransformComponent>().Margin; }
            set { GetComponent<UITransformComponent>().Margin = value; }
        }

        //---------------------------------------------------------------------------

        public UIEntity(string name) : base(name)
        {
            AddComponent<UITransformComponent>();
        }
    }
}
