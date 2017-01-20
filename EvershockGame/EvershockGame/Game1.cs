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
            cam1.Properties.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam1.Properties.Viewport = new Rectangle(0, 0, width / 2, height);
            cam1.Properties.AddTarget(player);
            //cam1.Properties.IsAmbientOcclusionEnabled = true;

            Camera cam2 = EntityFactory.Create<Camera>("Cam2");
            cam2.Properties.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam2.Properties.Viewport = new Rectangle(width / 2, 0, width / 2, height);
            cam2.Properties.AddTarget(player2);
            //cam2.Properties.IsAmbientOcclusionEnabled = true;

            CameraManager.Get().FuseCameras(cam1, cam2, width/2);

            /*--------------------------------------------------------------------------
                        Other
            --------------------------------------------------------------------------*/

            //EntityFactory.Create<Chest>("hallo").Init(new Vector2(300, 300));
            //EntityFactory.Create<Chest>("hallo").Init(new Vector2(500, 300));
            //EntityFactory.Create<Chest>("hallo").Init(new Vector2(300, 500));

            /*--------------------------------------------------------------------------
                        UI
            --------------------------------------------------------------------------*/

            UIManager.Get().Init(GraphicsDevice, width, height);

            ImageControl leftHP = EntityFactory.CreateUI<ImageControl>("HP");
            leftHP.VerticalAlignment = EVerticalAlignment.Top;
            leftHP.HorizontalAlignment = EHorizontalAlignment.Left;
            leftHP.Size = new Point(225, 50);

            ImageControl rightHP = EntityFactory.CreateUI<ImageControl>("HP");
            rightHP.VerticalAlignment = EVerticalAlignment.Top;
            rightHP.HorizontalAlignment = EHorizontalAlignment.Right;
            rightHP.Size = new Point(225, 50);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Get().Content = Content;
            AssetManager.Get().LoadMaps();

#if DEBUG
            // Load debug content
            //CollisionManager.Get().RectTexture = Content.Load<Texture2D>("Graphics/Debug/RectTextureDebug");
            //CollisionManager.Get().CircleTexture = Content.Load<Texture2D>("Graphics/Debug/CircleTextureDebug");

            Texture2D pointTex = new Texture2D(GraphicsDevice, 1, 1);
            pointTex.SetData(new Color[] { Color.White });
            CollisionManager.Get().PointTexture = pointTex;
            AssetManager.Get().Store<Texture2D>("DefaultPlaceholder", "Graphics/Debug/DefaultPlaceholder");
            SpriteComponent.DefaultTexture = AssetManager.Get().Find<Texture2D>("DefaultPlaceholder");
#endif

            AssetManager.Get().LoadAll<Texture2D>("Graphics");
            AssetManager.Get().LoadAll<Effect>("Effects");

            //AssetManager.Get().Store<Texture2D>("Background1", "Graphics/Camera/BackgroundTexture1");
            //AssetManager.Get().Store<Texture2D>("Background2", "Graphics/Camera/BackgroundTexture1");
            //AssetManager.Get().Store<Texture2D>("GroundTile1", "Graphics/Camera/BackgroundTexture1");

            //AssetManager.Get().Store<Texture2D>("Healthbar", "Graphics/Debug/Healthbar");

            //AssetManager.Get().Store<Texture2D>("RedOrb", "Graphics/Various/RedOrb");
            //AssetManager.Get().Store<Texture2D>("BlueOrb", "Graphics/Various/BlueOrb");
            //AssetManager.Get().Store<Texture2D>("YellowOrb", "Graphics/Various/YellowOrb");

            //AssetManager.Get().Store<Texture2D>("ChestClosed1", "Graphics/Tiles/ChestClosed1");
            //AssetManager.Get().Store<Texture2D>("ChestOpened1", "Graphics/Tiles/ChestOpened1");
            //AssetManager.Get().Store<Texture2D>("Barrel1", "Graphics/Tiles/Barrel1");
            //AssetManager.Get().Store<Texture2D>("PlayerIndicatorAnimationP1", "Graphics/Debug/ArrowSheetP1");
            //AssetManager.Get().Store<Texture2D>("PlayerIndicatorAnimationP2", "Graphics/Debug/ArrowSheetP2");

            //AssetManager.Get().Store<Texture2D>("BasicTileset", "Graphics/Tilesets/DungeonTileset");

            //AssetManager.Get().Store<Texture2D>("Kakariko_Village_Tiles", "Graphics/Tilesets/Debug/Kakariko_Village_Tiles");
            //AssetManager.Get().Store<Texture2D>("WalkingAnimation", "Graphics/Tilesets/Debug/WalkingAnimation");
            //AssetManager.Get().Store<Texture2D>("WalkingAnimation2", "Graphics/Tilesets/Debug/WalkingAnimation2");
            //AssetManager.Get().Store<Texture2D>("WalkingAnimation3", "Graphics/Tilesets/Debug/WalkingAnimation3");

            //AssetManager.Get().Store<Texture2D>("CircleLight", "Graphics/Lights/CircleLight");

            //AssetManager.Get().Store<Effect>("LightingEffect", "Effects/DeferredLighting");
            //AssetManager.Get().Store<Effect>("Occlusion", "Effects/Occlusion");
            //AssetManager.Get().Store<Effect>("Blur", "Effects/Blur");
            //AssetManager.Get().Store<Effect>("BloomExtract", "Effects/BloomExtract");
            //AssetManager.Get().Store<Effect>("BloomCombine", "Effects/BloomCombine");
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
            base.Draw(gameTime);
        }
    }
}
