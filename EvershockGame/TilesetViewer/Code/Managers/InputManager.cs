using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TilesetViewer
{
    public class InputManager : BaseManager<InputManager>
    {
        private Dictionary<ModifierKeys, Dictionary<Key, Action>> m_Callbacks;
        private bool m_ListenersAreRegistered = false;

        //---------------------------------------------------------------------------

        protected InputManager()
        {
            m_Callbacks = new Dictionary<ModifierKeys, Dictionary<Key, Action>>();
        }

        //---------------------------------------------------------------------------

        public void RegisterShortcut(Key key, ModifierKeys modifier, Action callback)
        {
            if (!m_ListenersAreRegistered)
            {
                Application.Current.MainWindow.KeyDown += OnKeyDown;
                Application.Current.MainWindow.KeyUp += OnKeyUp;
                m_ListenersAreRegistered = true;
            }

            if (!m_Callbacks.ContainsKey(modifier))
            {
                m_Callbacks.Add(modifier, new Dictionary<Key, Action>() { { key, callback } });
            }
            else
            {
                if (!m_Callbacks[modifier].ContainsKey(key)) m_Callbacks[modifier].Add(key, callback);
            }
        }

        //---------------------------------------------------------------------------

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        //---------------------------------------------------------------------------

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (m_Callbacks.ContainsKey(Keyboard.Modifiers))
            {
                if (m_Callbacks[Keyboard.Modifiers].ContainsKey(e.Key))
                {
                    m_Callbacks[Keyboard.Modifiers][e.Key]();
                }
            }
        }
    }
}
