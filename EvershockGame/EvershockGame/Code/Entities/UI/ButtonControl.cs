using EvershockGame.Components.UI;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Entities
{
    public delegate void ButtonClickEventHandler();

    //---------------------------------------------------------------------------

    public class ButtonControl : UIEntity
    {
        public event ButtonClickEventHandler Click;

        //---------------------------------------------------------------------------

        public ButtonControl(string name, Frame frame) : base(name, frame)
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
