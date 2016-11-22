using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code
{
    public enum EWindowFormat
    {
        Windowed,
        CenteredWindow,
        Fullscreen,
        BorderlessFullscreen
    }

    public static class GameWindowSettings
    {
        //public static Dictionary<EWindowSettings, byte> WindowSettings = new Dictionary<EWindowSettings, byte>()
        //{
        //    {EWindowSettings.Default,0},
        //    {EWindowSettings.Windowed,1},
        //    {EWindowSettings.Fullscreen,2},
        //    {EWindowSettings.BorderlessFullscreen,3}
        //};

        static int m_displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        static int m_displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        static int m_WindowWidth;
        static int m_WindowHeight;

        //---------------------------------------------------------------------------

        public static void SetWindowSettings(GraphicsDeviceManager graphics, GameWindow window, int width = 960, int height = 540, int xPos = 100, int yPos = 100, EWindowFormat eWindowFormat = 0)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            m_WindowWidth = width;
            m_WindowHeight = height;

            window.Position = new Point(xPos, yPos);

            SetWindowFormat(graphics, window, eWindowFormat);

            graphics.ApplyChanges();
        }

        //---------------------------------------------------------------------------

        public static void SetBackBufferSize(GraphicsDeviceManager graphics, GameWindow window, int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            m_WindowWidth = width;
            m_WindowHeight = height;

            graphics.ApplyChanges();
        }

        //---------------------------------------------------------------------------

        public static void SetWindowPosition(GameWindow window, int xPos, int yPos)
        {
            window.Position = new Point(xPos, yPos);
        }

        //---------------------------------------------------------------------------

        public static void SetWindowFormat(GraphicsDeviceManager graphics, GameWindow window, EWindowFormat eWindowFormat)
        {
            switch (eWindowFormat)
            {
                case EWindowFormat.Windowed:
                    {
                        graphics.PreferredBackBufferWidth = m_WindowWidth;
                        graphics.PreferredBackBufferHeight = m_WindowHeight;
                        graphics.IsFullScreen = false;
                        window.IsBorderless = false;
                        window.Position = new Point(100, 100);
                        window.AllowUserResizing = true;
                        break;
                    }

                case EWindowFormat.CenteredWindow:
                    {
                        graphics.PreferredBackBufferWidth = m_WindowWidth;
                        graphics.PreferredBackBufferHeight = m_WindowHeight;
                        graphics.IsFullScreen = false;
                        window.IsBorderless = false;
                        window.Position = new Point((int)((m_displayWidth - graphics.PreferredBackBufferWidth) / 2), (int)((m_displayHeight - graphics.PreferredBackBufferHeight) / 2));
                        window.AllowUserResizing = false;
                        break;
                    }

                case EWindowFormat.Fullscreen:
                    {
                        graphics.PreferredBackBufferWidth = m_displayWidth;
                        graphics.PreferredBackBufferHeight = m_displayHeight;
                        window.IsBorderless = false;
                        window.Position = Point.Zero;
                        window.AllowUserResizing = true;
                        break;
                    }

                case EWindowFormat.BorderlessFullscreen:
                    {
                        graphics.IsFullScreen = true;
                        window.IsBorderless = true;
                        window.AllowUserResizing = false;
                        break;
                    }
            }
        }

        //---------------------------------------------------------------------------

        public static void ToggleFullscreen(GraphicsDeviceManager graphics, GameWindow window)
        {
            if (graphics.IsFullScreen)
            {
                SetWindowFormat(graphics, window, EWindowFormat.Windowed);
            }
            else
            {
                SetWindowFormat(graphics, window, EWindowFormat.BorderlessFullscreen);
            }

            graphics.ApplyChanges();
        }
    }
}
