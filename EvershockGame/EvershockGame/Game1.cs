using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Components.UI;
using EvershockGame.Entities;
using EvershockGame.Factory;
using EvershockGame.Manager;
using EvershockGame.Stages;
using EvershockGame.Particles;
using EvershockGame.Code;
using EvershockGame.Code.Components;
using EvershockGame.Code.Entities.UI;
using EvershockGame.Code.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using EvershockGame.Items;

namespace EvershockGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyboardState, newKeyboardState;

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

            oldKeyboardState = Keyboard.GetState();

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            LightingManager.Get().Device = GraphicsDevice;

            SeedManager.Get().ResetBaseSeed(1234);

            /*--------------------------------------------------------------------------
                       Items
            --------------------------------------------------------------------------*/

            ItemManager.Get().LoadItems();
            ItemManager.Get().LoadItemPools();

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
                    EntityFactory.Create<Chest>("Chest").Init(position);
                }
                EntityFactory.Create<SimpleTestEnemy>("Enemy").Init(new Vector2(room.Bounds.Center.X * 64, room.Bounds.Center.Y * 64));

                for (int x = 0; x < room.Bounds.Width; x += 3)
                {
                    IEntity torch = EntityFactory.Create<Entity>("Torch");
                    torch.AddComponent<TransformComponent>().Init(new Vector3(room.Bounds.X * 64 + x * 64 + 32, room.Bounds.Y * 64 + 32, 110));
                    torch.AddComponent<ParticleSpawnerComponent>().Emitter.Description = ParticleDesc.Fire;
                    torch.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), new Vector2(0, 6), new Vector2(1, 1), Color.Orange, 0.6f);
                }
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
            cam1.Properties.Tileset = AssetManager.Get().Find<Texture2D>(ETilesetAssets.DungeonTileset2);
            cam1.Properties.AddTarget(player);
            //cam1.Properties.IsAmbientOcclusionEnabled = true;

            Camera cam2 = EntityFactory.Create<Camera>("Cam2");
            cam2.Properties.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>(ESpriteAssets.CameraBackground1), AssetManager.Get().Find<Effect>(EEffectAssets.DeferredLighting));
            cam2.Properties.Viewport = new Rectangle(width / 2, 0, width / 2, height);
            cam2.Properties.BloomExtractEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomExtract);
            cam2.Properties.BloomCombineEffect = AssetManager.Get().Find<Effect>(EEffectAssets.BloomCombine);
            cam2.Properties.BlurEffect = AssetManager.Get().Find<Effect>(EEffectAssets.Blur);
            cam2.Properties.Tileset = AssetManager.Get().Find<Texture2D>(ETilesetAssets.DungeonTileset2);
            cam2.Properties.AddTarget(player2);
            //cam2.Properties.IsAmbientOcclusionEnabled = true;

            CameraManager.Get().FuseCameras(cam1, cam2, width / 2);


            /*--------------------------------------------------------------------------
                        UI
            --------------------------------------------------------------------------*/

            UIManager.Get().Init(GraphicsDevice, width, height);
            SpriteFont debug_font = AssetManager.Get().Find<SpriteFont>(EFontAssets.DebugFont);

            UIEntity healthbar1 = EntityFactory.CreateUI<UIEntity>("HealthbarPlayer1");
            HealthbarComponent healthbarcomponent1 = healthbar1.AddComponent<HealthbarComponent>();
            healthbar1.VerticalAlignment = EVerticalAlignment.Top;
            healthbar1.HorizontalAlignment = EHorizontalAlignment.Left;
            healthbar1.Margin = new Rectangle(15, 25, 0, 0);
            healthbarcomponent1.BindPlayer(player, EHorizontalAlignment.Left);

            UIEntity healthbar2 = EntityFactory.CreateUI<UIEntity>("HealthbarPlayer2");
            HealthbarComponent healthbarcomponent2 = healthbar2.AddComponent<HealthbarComponent>();
            healthbar2.VerticalAlignment = EVerticalAlignment.Top;
            healthbar2.HorizontalAlignment = EHorizontalAlignment.Right;
            healthbar2.Margin = new Rectangle(0, 25, 15, 0);
            healthbarcomponent2.BindPlayer(player2, EHorizontalAlignment.Right);

            UIEntity coins = EntityFactory.CreateUI<UIEntity>("CollectedCoins");
            CoinCollectionComponent coincollectioncomponent = coins.AddComponent<CoinCollectionComponent>();
            coins.VerticalAlignment = EVerticalAlignment.Top;
            coins.HorizontalAlignment = EHorizontalAlignment.Center;
            coins.Margin = new Rectangle(0, 25, 0, 0);
            coincollectioncomponent.Bind(player, player2);

#if DEBUG
            ConsoleManager.Get().RegisterCommand("SpawnChestAtPosition", null, (Func<int, int, string>)Chest.SpawnChest);
            ConsoleManager.Get().RegisterCommand("SpawnChestAtCamera", null, (Func<int, string>)Chest.SpawnChest);
            ConsoleManager.Get().RegisterCommand("SpawnPickup", null, (Func<string, int, Pickup>)PickupFactory.Create);
#endif
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Get().Content = Content;
            AssetManager.Get().LoadAll();

#if DEBUG
            Texture2D pointTex = new Texture2D(GraphicsDevice, 1, 1);
            pointTex.SetData(new Color[] { Color.White });
            CollisionManager.Get().PointTexture = pointTex;
            SpriteComponent.DefaultSprite = AssetManager.Get().Find<Texture2D>(ESpriteAssets.DefaultTexture);
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
            newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.GetPressedKeys().Length != 0)
            {
                if (newKeyboardState.IsKeyDown(Keys.F) && oldKeyboardState.IsKeyUp(Keys.F))
                    GameWindowSettings.ToggleFullscreen(graphics, Window);

#if DEBUG
                if (newKeyboardState.IsKeyDown(Keys.F9) && oldKeyboardState.IsKeyUp(Keys.F9))
                    UIManager.Get().IsUIDebugViewActive = true;
                else if (newKeyboardState.IsKeyDown(Keys.F10) && oldKeyboardState.IsKeyUp(Keys.F10))
                    UIManager.Get().IsUIDebugViewActive = false;

                if (newKeyboardState.IsKeyDown(Keys.F11) && oldKeyboardState.IsKeyUp(Keys.F11))
                    CollisionManager.Get().IsDebugViewActive = true;
                else if (newKeyboardState.IsKeyDown(Keys.F12) && oldKeyboardState.IsKeyUp(Keys.F12))
                    CollisionManager.Get().IsDebugViewActive = false;

                if (newKeyboardState.IsKeyDown(Keys.Escape))
                    Exit();
#endif
            }

                oldKeyboardState = newKeyboardState;

                GameManager.Get().Tick(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);

                base.Update(gameTime);
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
