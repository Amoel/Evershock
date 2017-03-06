using EvershockGame.Code.Components.UI;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Entities
{
    public delegate void ButtonClickEventHandler();

    //---------------------------------------------------------------------------

    public class ButtonControl : UIEntity
    {
        public event ButtonClickEventHandler Click;

        //---------------------------------------------------------------------------

        public ButtonControl(string name, Guid parent, Frame frame) : base(name, parent, frame)
        {
            AddComponent<ButtonComponent>();
        }

        //---------------------------------------------------------------------------

        private void OnClick()
        {
            Click?.Invoke();
        }
    }
}
