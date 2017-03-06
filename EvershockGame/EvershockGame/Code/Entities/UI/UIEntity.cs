using EvershockGame.Code.Components.UI;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Entities
{
    public class UIEntity : Entity
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

        public UIEntity Previous { get; set; }
        public UIEntity Next { get; set; }

        //---------------------------------------------------------------------------

        public UIEntity(string name, Guid parent, Frame frame) : base(name, parent)
        {
            AddComponent<UITransformComponent>();
        }

        //---------------------------------------------------------------------------

        public void Focus()
        {
            UIManager.Get().Focus(this);
        }

        //---------------------------------------------------------------------------

        public void FocusNext()
        {
            UIManager.Get().Focus(Previous);
        }

        //---------------------------------------------------------------------------

        public void FocusPrevious()
        {
            UIManager.Get().Focus(Next);
        }

        //---------------------------------------------------------------------------

        public void Bind(IComponent component, string property, PropertyChangedEventHandler callback)
        {
            if (component != null)
            {
                UIManager.Get().RegisterListener(component, property, callback);
            }
        }
    }
}
