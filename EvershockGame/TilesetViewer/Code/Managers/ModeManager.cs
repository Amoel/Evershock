using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace TilesetViewer
{
    public delegate void ModeChangedEventHandler<T>(T mode);

    //---------------------------------------------------------------------------

    public class ModeManager<T> : BaseManager<ModeManager<T>> where T : IComparable
    {
        public T Mode { get; private set; }
        public event ModeChangedEventHandler<T> ModeChanged;

        private Dictionary<T, ToggleButton> m_Buttons;

        //---------------------------------------------------------------------------

        protected ModeManager()
        {
            m_Buttons = new Dictionary<T, ToggleButton>();
        }

        //---------------------------------------------------------------------------

        public void Register(T mode, ToggleButton button)
        {
            m_Buttons.Add(mode, button);
            button.Checked += (sender, e) => { ChangeMode(mode); };
            button.Unchecked += (sender, e) => { e.Handled = true; };
        }

        //---------------------------------------------------------------------------

        public void ChangeMode(T mode)
        {
            if (mode.Equals(Mode)) return;
            if (m_Buttons.ContainsKey(mode))
            {
                if (m_Buttons.ContainsKey(Mode))
                {
                    m_Buttons[Mode].IsChecked = false;
                }
                Mode = mode;
                m_Buttons[Mode].IsChecked = true;
                OnModeChanged(Mode);
            }
        }

        //---------------------------------------------------------------------------

        private void OnModeChanged(T mode)
        {
            ModeChanged?.Invoke(mode);
        }
    }
}
