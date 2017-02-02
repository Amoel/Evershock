using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Components.UI;
using EntityComponent.Entities;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EntityComponent.Stages;
using EvershockGame.Code;
using EvershockGame.Code.Components;
using EvershockGame.Code.Entities.UI;
using EvershockGame.Code.Factories;
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
            SpriteFont debug_font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);

            //#region TextWrappingTest
            //TextControl control = EntityFactory.CreateUI<TextControl>("P1_Position");
            //control.Properties.Font = debug_font;
            //control.Size = new Point(600, 170);
            //control.Properties.TextAlignment = EHorizontalAlignment.Right;
            //control.Properties.IsWrapping = true;
            //control.VerticalAlignment = EVerticalAlignment.Bottom;
            //control.HorizontalAlignment = EHorizontalAlignment.Center;
            //control.Bind(player.Transform, "Location", (value) =>
            //{
            //    Vector3 location = (Vector3)value;
            //    control.Properties.Text = string.Format("Hello World!\nThis is a test to check the <Orange>COLORING</Orange> and <Purple>WRAPPING</Purple> of long texts. It should also work with dynamic text, like <Cyan>location[x: <Red>{0}</Red>, y: <Red>{1}</Red>]</Cyan>", (int)location.X, (int)location.Y);
            //});

            //TextControl control2 = EntityFactory.CreateUI<TextControl>("P1_Position");
            //control2.Properties.Font = debug_font;
            //control2.Size = new Point(600, 370);
            //control2.Properties.TextAlignment = EHorizontalAlignment.Left;
            //control2.Properties.IsWrapping = true;
            //control2.VerticalAlignment = EVerticalAlignment.Bottom;
            //control2.HorizontalAlignment = EHorizontalAlignment.Center;
            //control2.Bind(player.Transform, "Location", (value) =>
            //{
            //    Vector3 location = (Vector3)value;
            //    control2.Properties.Text = string.Format("Hello World!\nThis is a test to check the <Orange>COLORING</Orange> and <Purple>WRAPPING</Purple> of long texts. It should also work with dynamic text, like <Cyan>location[x: <Red>{0}</Red>, y: <Red>{1}</Red>]</Cyan>", (int)location.X, (int)location.Y);
            //});

            //TextControl control3 = EntityFactory.CreateUI<TextControl>("P1_Position");
            //control3.Properties.Font = debug_font;
            //control3.Size = new Point(600, 570);
            //control3.Properties.TextAlignment = EHorizontalAlignment.Center;
            //control3.Properties.IsWrapping = true;
            //control3.VerticalAlignment = EVerticalAlignment.Bottom;
            //control3.HorizontalAlignment = EHorizontalAlignment.Center;
            //control3.Bind(player.Transform, "Location", (value) =>
            //{
            //    Vector3 location = (Vector3)value;
            //    control3.Properties.Text = string.Format("Hello World!\nThis is a test to check the <Orange>COLORING</Orange> and <Purple>WRAPPING</Purple> of long texts. It should also work with dynamic text, like <Cyan>location[x: <Red>{0}</Red>, y: <Red>{1}</Red>]</Cyan>", (int)location.X, (int)location.Y);
            //});
            //#endregion

            //TextControl HP_Player1 = EntityFactory.CreateUI<TextControl>("HP_Player1");
            //HP_Player1.Properties.Font = debug_font;
            //HP_Player1.Size = new Point(50, 100);
            //HP_Player1.VerticalAlignment = EVerticalAlignment.Bottom;
            //HP_Player1.HorizontalAlignment = EHorizontalAlignment.Left;
            //HP_Player1.Bind(player.Attributes, "CurrentHealth", (value) =>
            //{
            //    HP_Player1.Properties.Text = string.Format("HP  <Red>{0}</Red>", new string('X', (int)(float)value / 20));
            //});

            //TextControl MP_Player1 = EntityFactory.CreateUI<TextControl>("MP_Player1");
            //MP_Player1.Properties.Font = debug_font;
            //MP_Player1.Size = new Point(50, 50);
            //MP_Player1.VerticalAlignment = EVerticalAlignment.Bottom;
            //MP_Player1.HorizontalAlignment = EHorizontalAlignment.Left;
            //MP_Player1.Bind(player.Attributes, "CurrentMana", (value) =>
            //{
            //    MP_Player1.Properties.Text = string.Format("MP  <Cyan>{0}</Cyan>", new string('x', (int)(float)value / 10));
            //});

            //TextControl HP_Player2 = EntityFactory.CreateUI<TextControl>("HP_Player2");
            //HP_Player2.Properties.Font = debug_font;
            //HP_Player2.Size = new Point(50, 100);
            //HP_Player2.VerticalAlignment = EVerticalAlignment.Bottom;
            //HP_Player2.HorizontalAlignment = EHorizontalAlignment.Right;
            //HP_Player2.Properties.TextAlignment = EHorizontalAlignment.Right;
            //HP_Player2.Bind(player2.Attributes, "CurrentHealth", (value) =>
            //{
            //    HP_Player2.Properties.Text = string.Format("<Red>{0}</Red>  HP", new string('X', (int)(float)value / 20));
            //});

            //TextControl MP_Player2 = EntityFactory.CreateUI<TextControl>("MP_Player2");
            //MP_Player2.Properties.Font = debug_font;
            //MP_Player2.Size = new Point(50, 50);
            //MP_Player2.VerticalAlignment = EVerticalAlignment.Bottom;
            //MP_Player2.HorizontalAlignment = EHorizontalAlignment.Right;
            //MP_Player2.Properties.TextAlignment = EHorizontalAlignment.Right;
            //MP_Player2.Bind(player2.Attributes, "CurrentMana", (value) =>
            //{
            //    MP_Player2.Properties.Text = string.Format("<Cyan>{0}</Cyan>  MP", new string('x', (int)(float)value / 10));
            //});

            Healthbar bar1 = EntityFactory.CreateUI<Healthbar>("HealthbarPlayer1");
            bar1.VerticalAlignment = EVerticalAlignment.Top;
            bar1.HorizontalAlignment = EHorizontalAlignment.Left;
            bar1.Margin = new Rectangle(15, 25, 0, 0);
            bar1.Properties.BindPlayer(player, EHorizontalAlignment.Left);

            Healthbar bar2 = EntityFactory.CreateUI<Healthbar>("HealthbarPlayer2");
            bar2.VerticalAlignment = EVerticalAlignment.Top;
            bar2.HorizontalAlignment = EHorizontalAlignment.Right;
            bar2.Margin = new Rectangle(0, 25, 15, 0);
            bar2.Properties.BindPlayer(player2, EHorizontalAlignment.Right);

            ParticleDesc fireDesc = ParticleDesc.Default;
            fireDesc.ParticleColor = (time) => Color.Lerp(Color.Orange, Color.Red, time);
            fireDesc.ParticleOpacity = (time) => time < 0.5f ? 1.0f : (1.0f - time) * 2.0f;
            fireDesc.ParticleSize = (time) => new Vector2((1.5f - time) * 4, (1.5f - time) * 6);
            fireDesc.LightColor = (time) => Color.Lerp(Color.Orange, Color.Red, time);
            fireDesc.LightOpacity = (time) => time < 0.5f ? 1.0f : (1.0f - time) * 2.0f;
            fireDesc.LightSize = (time) => new Vector2((1.5f - time) * 0.05f, (1.5f - time) * 0.05f);
            fireDesc.Gravity = (time) => -0.3f;
            fireDesc.LifeTime = 0.5f;
            fireDesc.HasShadow = false;

            for (int x = 0; x < 8; x++)
            {
                IEntity particleTest1 = EntityFactory.Create<Entity>("Test1");
                particleTest1.AddComponent<TransformComponent>().Init(new Vector3(1314 + x * 256, 1292, 100));
                particleTest1.AddComponent<ParticleSpawnerComponent>().Emitter.Description = fireDesc;
                particleTest1.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), new Vector2(0, 6), new Vector2(1, 1), Color.Orange, 0.6f);
            }

            ConsoleManager.Get().RegisterCommand("SpawnChestAtPosition", null, (Func<int, int, string>)Chest.SpawnChest);
            ConsoleManager.Get().RegisterCommand("SpawnChestAtCamera", null, (Func<int, string>)Chest.SpawnChest);
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
            ConsoleManager.Get().Font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);
        }

        //---------------------------------------------------------------------------

        protected override void UnloadContent()
        {
        }

        //---------------------------------------------------------------------------

        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            KeyboardState keyboardstate = Keyboard.GetState();

            if (keyboardstate.GetPressedKeys().Length != 0);
            {
                if (keyboardstate.IsKeyDown(Keys.F))
            {
                GameWindowSettings.ToggleFullscreen(graphics, Window);
            }

                if (keyboardstate.IsKeyDown(Keys.F9))
            {
                UIManager.Get().IsUIDebugViewActive = true;
            }
                else if (keyboardstate.IsKeyDown(Keys.F10))
            {
                UIManager.Get().IsUIDebugViewActive = false;
            }
                if (keyboardstate.IsKeyDown(Keys.F11))
            {
                CollisionManager.Get().IsDebugViewActive = true;
            }
                else if (keyboardstate.IsKeyDown(Keys.F12))
            {
                CollisionManager.Get().IsDebugViewActive = false;
            }

                if (keyboardstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            }

#endif
            GameManager.Get().Tick(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            base.Update(gameTime);

            //TODO_lukas Shift to UI
            //if ((Keyboard.GetState().IsKeyDown(Keys.LeftControl) || (Keyboard.GetState().IsKeyDown(Keys.RightControl))) && !playerIndicatorP1.IsEnabled)
            //{
            //    playerIndicatorP1.Enable();
            //    playerIndicatorP2.Enable();
            //}

            //if (Keyboard.GetState().IsKeyUp(Keys.LeftControl) && (Keyboard.GetState().IsKeyUp(Keys.RightControl)) && playerIndicatorP1.IsEnabled)
            //{
            //    playerIndicatorP1.Disable();
            //    playerIndicatorP2.Disable();
            //}
        }
        
        Code.Misc.FrameCounter frameCount = new Code.Misc.FrameCounter();
        protected override void Draw(GameTime gameTime)
        {
            frameCount.Update(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

            GameManager.Get().Render(GraphicsDevice, spriteBatch, gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            
            spriteBatch.Begin();
            spriteBatch.DrawString(AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont), string.Format("FPS: {0}", frameCount.AverageFramesPerSecond), Vector2.Zero, Color.Yellow);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
