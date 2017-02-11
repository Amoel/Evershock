using EvershockGame.Manager;
using System;

namespace EvershockGame
{   

#if WINDOWS || LINUX

    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null)
            {
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "ShowDebugView": CollisionManager.Get().IsDebugViewActive = true; break;
                        case "HideAsserts": AssertManager.Get().HideAsserts = true; break;
                        case "ShowUIDebugView": UIManager.Get().IsUIDebugViewActive = true; break;
                    }
                }
            }

            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
