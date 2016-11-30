using EntityComponent.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Entities
{
    public delegate void ButtonClickEventHandler();

    //---------------------------------------------------------------------------

    public class ButtonControl : UIEntity
    {
        public event ButtonClickEventHandler Click;

        //---------------------------------------------------------------------------

        public ButtonControl(string name) : base(name)
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
