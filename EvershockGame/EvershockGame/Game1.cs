using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Components.UI;
using EntityComponent.Entities;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EntityComponent.Stages;
using EvershockGame.Code;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;


namespace EvershockGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        IEntity playerIndicatorP1 = EntityFactory.Create<Entity>("playerIndicatorP1");
        IEntity playerIndicatorP2 = EntityFactory.Create<Entity>("playerIndicatorP2");

        //---------------------------------------------------------------------------

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };
            Content.RootDirectory = "Content";
        }

        //---------------------------------------------------------------------------

        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true;
            GameWindowSettings.SetWindowSettings(graphics, Window, 1680, 1050);

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            LightingManager.Get().Device = GraphicsDevice;

            SeedManager.Get().ResetBaseSeed(1234);

            /*--------------------------------------------------------------------------
                       Stage
            --------------------------------------------------------------------------*/

            Stage stage = new Stage(SeedManager.Get().NextSeed());
            StageManager.Get().Create(stage.CreateMap());
            StageManager.Get().Stage = stage;

            foreach (Room room in stage.Rooms)
            {
                int max = SeedManager.Get().NextSeed(3, 7);
                for (int i = 0; i < max; i++)
                {
                    Vector2 position = new Vector2(room.Bounds.Center.X * 64 + (float)Math.Sin((i / (max / 2.0f)) * Math.PI) * 300, room.Bounds.Center.Y * 64 + (float)Math.Cos((i / (max / 2.0f)) * Math.PI) * 300);
                    EntityFactory.Create<Chest>("hallo").Init(position);
                }
                for (int i = 0; i < 9; i++)
                {
                    EntityFactory.Create<Spike>("Spike").Init(new Vector2(room.Bounds.Center.X * 64 + (i % 3) * 64, room.Bounds.Center.Y * 64 + 128 + (i / 3) * 64));
                }
                EntityFactory.Create<SimpleTestEnemy>("Enemy").Init(new Vector2(room.Bounds.Center.X * 64, room.Bounds.Center.Y * 64));
            }
            

            stage.SaveStageAsImage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Map.png"));

            /*--------------------------------------------------------------------------
                       Player 1
            --------------------------------------------------------------------------*/

            Player player = EntityFactory.Create<Player>("Player1");

            /*--------------------------------------------------------------------------
                        Player 2
            --------------------------------------------------------------------------*/

            Player player2 = EntityFactory.Create<Player>("Player2");

            /*--------------------------------------------------------------------------
                        Camera
            --------------------------------------------------------------------------*/

            CameraManager.Get().Init(width, height);

            Camera cam1 = EntityFactory.Create<Camera>("Cam1");
            cam1.Properties.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>(ESpriteAssets.CameraBackground1), AssetManager.Get().Find<Effect>(EEffectAssets.DeferredLighting));
            cam1.Properties.Viewport = new Rectangle(0, 0, width / 2, height);
            cam1.Properties.BloomExtractEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomExtract);
            cam1.Properties.BloomCombineEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomCombine);
            cam1.Properties.BlurEffect = AssetManager.Get().Find<Effect>(EEffectAssets.Blur);
            cam1.Properties.Tileset = AssetManager.Get().Find<Texture2D>(ETilesetAssets.DungeonTileset1);
            cam1.Properties.AddTarget(player);
            //cam1.Properties.IsAmbientOcclusionEnabled = true;

            Camera cam2 = EntityFactory.Create<Camera>("Cam2");
            cam2.Properties.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>(ESpriteAssets.CameraBackground1), AssetManager.Get().Find<Effect>(EEffectAssets.DeferredLighting));
            cam2.Properties.Viewport = new Rectangle(width / 2, 0, width / 2, height);
            cam2.Properties.BloomExtractEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomExtract);
            cam2.Properties.BloomCombineEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomCombine);
            cam2.Properties.BlurEffect = AssetManager.Get().Find<Effect>(EEffectAssets.Blur);
            cam2.Properties.Tileset = AssetManager.Get().Find<Texture2D>(ETilesetAssets.DungeonTileset1);
            cam2.Properties.AddTarget(player2);
            //cam2.Properties.IsAmbientOcclusionEnabled = true;

            CameraManager.Get().FuseCameras(cam1, cam2, width / 2);

            /*--------------------------------------------------------------------------
                        UI
            --------------------------------------------------------------------------*/

            UIManager.Get().Init(GraphicsDevice, width, height);

            TextControl control = EntityFactory.CreateUI<TextControl>("Test");
            control.Properties.Font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);
            control.VerticalAlignment = EVerticalAlignment.Top;
            control.HorizontalAlignment = EHorizontalAlignment.Center;
            control.Bind(player.Transform, "Location", (value) =>
            {
                Vector3 location = (Vector3)value;
                control.Properties.Text = string.Format("X: {0}, Y: {1}", (int)location.X, (int)location.Y);
            });

            //ImageControl leftHP = EntityFactory.CreateUI<ImageControl>("HP");
            //leftHP.Image = AssetManager.Get().Find<Texture2D>(ESpriteAssets.ChestClosed1);
            //leftHP.VerticalAlignment = EVerticalAlignment.Top;
            //leftHP.HorizontalAlignment = EHorizontalAlignment.Left;
            //leftHP.Size = new Point(225, 100);

            //ImageControl rightHP = EntityFactory.CreateUI<ImageControl>("HP");
            //rightHP.VerticalAlignment = EVerticalAlignment.Top;
            //rightHP.HorizontalAlignment = EHorizontalAlignment.Right;
            //rightHP.Size = new Point(225, 100);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Get().Content = Content;
            AssetManager.Get().LoadAll();

#if DEBUG
            // Load debug content
            //CollisionManager.Get().RectTexture = Content.Load<Texture2D>("Graphics/Debug/RectTextureDebug");
            //CollisionManager.Get().CircleTexture = Content.Load<Texture2D>("Graphics/Debug/CircleTextureDebug");

            Texture2D pointTex = new Texture2D(GraphicsDevice, 1, 1);
            pointTex.SetData(new Color[] { Color.White });
            CollisionManager.Get().PointTexture = pointTex;
            SpriteComponent.DefaultTexture = AssetManager.Get().Find<Texture2D>(ESpriteAssets.DefaultTexture);
#endif
        }

        //---------------------------------------------------------------------------

        protected override void UnloadContent()
        {
        }

        //---------------------------------------------------------------------------

        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                GameWindowSettings.ToggleFullscreen(graphics, Window);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F9))
            {
                UIManager.Get().IsUIDebugViewActive = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F10))
            {
                UIManager.Get().IsUIDebugViewActive = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                CollisionManager.Get().IsDebugViewActive = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                CollisionManager.Get().IsDebugViewActive = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
#endif
            GameManager.Get().Tick(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            base.Update(gameTime);


            //TODO: Shift to UI

            if ((Keyboard.GetState().IsKeyDown(Keys.LeftControl) || (Keyboard.GetState().IsKeyDown(Keys.RightControl))) && !playerIndicatorP1.IsEnabled)
            {
                playerIndicatorP1.Enable();
                playerIndicatorP2.Enable();
            }

            if (Keyboard.GetState().IsKeyUp(Keys.LeftControl) && (Keyboard.GetState().IsKeyUp(Keys.RightControl)) && playerIndicatorP1.IsEnabled)
            {
                playerIndicatorP1.Disable();
                playerIndicatorP2.Disable();
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GameManager.Get().Render(GraphicsDevice, spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont), "Hello World!", new Vector2(10, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
