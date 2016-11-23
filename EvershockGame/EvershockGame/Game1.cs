using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EvershockGame.Code;
using EvershockGame.Code.Managers;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EvershockGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        IEntity player;
        IEntity player2;
        IEntity playerIndicatorP1;
        IEntity playerIndicatorP2;
        AnimationComponent piAnimationP1;
        AnimationComponent piAnimationP2;

        //---------------------------------------------------------------------------

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //---------------------------------------------------------------------------

        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true;

            GameWindowSettings.SetWindowSettings(graphics,Window);

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;


            /*--------------------------------------------------------------------------
                       Player 1
            --------------------------------------------------------------------------*/

            player = EntityFactory.Create<Entity>("Player");
            player.AddComponent<TransformComponent>().Init(new Vector3(100, 410, 0));
            player.AddComponent<AttributesComponent>().Init(0, 5.0f);
            player.AddComponent<ActorPhysicsComponent>().Init(0.9f, 1.0f, 0.0f);
            player.AddComponent<CircleColliderComponent>().Init(22, BodyType.Dynamic);
            player.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(4, 4));

            MovementAnimationComponent animation = player.AddComponent<MovementAnimationComponent>();
            animation.Init(AssetManager.Get().Find<Texture2D>("WalkingAnimation"),new Vector2 (0.5f,0.5f));
            animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true));
            animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7));

            InputComponent input = player.AddComponent<InputComponent>();
            input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_LEFT);
            input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_RIGHT);
            input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_UP);
            input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_DOWN);

            playerIndicatorP1 = EntityFactory.Create<Entity>(player.GUID, "PlayerIndicatorP1");
            playerIndicatorP1.AddComponent<TransformComponent>().Init(new Vector3(0, -65, 0));
            piAnimationP1 = playerIndicatorP1.AddComponent<AnimationComponent>();
            piAnimationP1.Init(AssetManager.Get().Find<Texture2D>("PlayerIndicatorAnimationP1"), new Vector2(0.4f, 0.4f));
            piAnimationP1.AddSetting(0, new AnimationSetting(8, 1, 0, 7, false));
            playerIndicatorP1.Disable();

            /*--------------------------------------------------------------------------
                        Player 2
            --------------------------------------------------------------------------*/

            player2 = EntityFactory.Create<Entity>("Player2");
            player2.AddComponent<TransformComponent>().Init(new Vector3(800, 180, 0));

            player2.AddComponent<AttributesComponent>().Init(0, 5.0f);
            player2.AddComponent<ActorPhysicsComponent>().Init(0.9f, 1.0f, 0.0f);
            player2.AddComponent<CircleColliderComponent>().Init(22, BodyType.Dynamic);
            player2.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(4, 4));

            MovementAnimationComponent animation2 = player2.AddComponent<MovementAnimationComponent>();
            animation2.Init(AssetManager.Get().Find<Texture2D>("WalkingAnimation"), new Vector2(0.5f, 0.5f));
            animation2.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true));
            animation2.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7));

            InputComponent input2 = player2.AddComponent<InputComponent>();
            input2.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_A);
            input2.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_D);
            input2.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_W);
            input2.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_S);

            playerIndicatorP2 = EntityFactory.Create<Entity>(player2.GUID, "PlayerIndicatorP2");
            playerIndicatorP2.AddComponent<TransformComponent>().Init(new Vector3(0, -65, 0));
            piAnimationP2 = playerIndicatorP2.AddComponent<AnimationComponent>();
            piAnimationP2.Init(AssetManager.Get().Find<Texture2D>("PlayerIndicatorAnimationP2"), new Vector2(0.4f, 0.4f));
            piAnimationP2.AddSetting(0, new AnimationSetting(8, 1, 0, 7, false));
            playerIndicatorP2.Disable();

            /*--------------------------------------------------------------------------
                        Camera
            --------------------------------------------------------------------------*/

            IEntity camera = EntityFactory.Create<Entity>("Camera");

            camera.AddComponent<TransformComponent>();
            CameraComponent cam = camera.AddComponent<CameraComponent>();
            cam.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam.AddTarget(player);

            IEntity camera2 = EntityFactory.Create<Entity>("Camera");

            camera2.AddComponent<TransformComponent>();
            CameraComponent cam2 = camera2.AddComponent<CameraComponent>();
            cam2.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam2.AddTarget(player2);

            /*--------------------------------------------------------------------------
                        Other
            --------------------------------------------------------------------------*/

            IEntity rock3 = EntityFactory.Create<Entity>("Rock3");
            rock3.AddComponent<TransformComponent>().Init(new Vector3(200, 0, 0));
            rock3.AddComponent<PhysicsComponent>();
            rock3.AddComponent<CircleColliderComponent>().Init(30, Vector2.Zero, BodyType.Dynamic, 10.0f);
            rock3.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"));

            IEntity map = EntityFactory.Create<Entity>("Map");
            map.AddComponent<TransformComponent>();
            map.AddComponent<PhysicsComponent>();
            map.AddComponent<MapComponent>().Init(AssetManager.Get().Find<Level.Map>("TestMap"));
            
            MapManager.Get().CreateCollisionFromMap(map, AssetManager.Get().Find<Level.Map>("TestMap"));
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Get().Content = Content;
            AssetManager.Get().LoadMaps();

#if DEBUG
            // Load debug content
            CollisionManager.Get().RectTexture = Content.Load<Texture2D>("Graphics/Debug/RectTextureDebug");
            CollisionManager.Get().CircleTexture = Content.Load<Texture2D>("Graphics/Debug/CircleTextureDebug");

            Texture2D pointTex = new Texture2D(GraphicsDevice, 1, 1);
            pointTex.SetData(new Color[] { Color.White });
            CollisionManager.Get().PointTexture = pointTex;
#endif

            AssetManager.Get().Store<Texture2D>("Background1", "Graphics/Camera/BackgroundTexture1");
            AssetManager.Get().Store<Texture2D>("Background2", "Graphics/Camera/BackgroundTexture1");
            AssetManager.Get().Store<Texture2D>("GroundTile1", "Graphics/Camera/BackgroundTexture1");

            AssetManager.Get().Store<Texture2D>("ChestClosed1", "Graphics/Tiles/ChestClosed1");
            AssetManager.Get().Store<Texture2D>("ChestOpened1", "Graphics/Tiles/ChestOpened1");
            AssetManager.Get().Store<Texture2D>("Barrel1", "Graphics/Tiles/Barrel1");
            AssetManager.Get().Store<Texture2D>("PlayerIndicatorAnimationP1", "Graphics/Debug/ArrowSheetP1");
            AssetManager.Get().Store<Texture2D>("PlayerIndicatorAnimationP2", "Graphics/Debug/ArrowSheetP2");

            AssetManager.Get().Store<Texture2D>("tileset2", "Graphics/Tilesets/tileset2");

            AssetManager.Get().Store<Texture2D>("Kakariko_Village_Tiles", "Graphics/Tilesets/Debug/Kakariko_Village_Tiles");
            AssetManager.Get().Store<Texture2D>("WalkingAnimation", "Graphics/Tilesets/Debug/WalkingAnimation");

            AssetManager.Get().Store<Texture2D>("CircleLight", "Graphics/Lights/CircleLight");
            AssetManager.Get().Store<Effect>("LightingEffect", "Effects/DeferredLighting");
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
            ComponentManager.Get().TickComponents(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            PhysicsManager.Get().Step(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            base.Update(gameTime);

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
            CameraManager.Get().Render(GraphicsDevice, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
