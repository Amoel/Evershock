using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace TilesetViewer
{
    public enum EEditMode
    {
        None,
        Tiles,
        Blocker
    }

    //---------------------------------------------------------------------------

    public delegate void ModeChangedEventHandler(EEditMode mode);

    //---------------------------------------------------------------------------

    public class ModeManager : BaseManager<ModeManager>
    {
        public EEditMode Mode { get; private set; }
        public event ModeChangedEventHandler ModeChanged;

        private Dictionary<EEditMode, ToggleButton> m_Buttons;

        //---------------------------------------------------------------------------

        protected ModeManager()
        {
            m_Buttons = new Dictionary<EEditMode, ToggleButton>();
            Mode = EEditMode.None;
        }

        //---------------------------------------------------------------------------

        public void Register(EEditMode mode, ToggleButton button)
        {
            m_Buttons.Add(mode, button);
            button.Checked += (sender, e) => { ChangeMode(mode); };
            button.Unchecked += (sender, e) => { e.Handled = true; };
        }

        //---------------------------------------------------------------------------

        public void ChangeMode(EEditMode mode)
        {
            if (mode == Mode) return;
            if (m_Buttons.ContainsKey(mode))
            {
                if (Mode != EEditMode.None)
                {
                    m_Buttons[Mode].IsChecked = false;
                }
                Mode = mode;
                m_Buttons[Mode].IsChecked = true;
                OnModeChanged(Mode);
            }
        }

        //---------------------------------------------------------------------------

        private void OnModeChanged(EEditMode mode)
        {
            ModeChanged?.Invoke(mode);
        }
    }
}
