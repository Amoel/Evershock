using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EvershockGame.Code;
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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 940;
            graphics.PreferredBackBufferHeight = 560;
            graphics.ApplyChanges();

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;


            IEntity player = EntityFactory.Create<Entity>("Player");
            player.AddComponent<TransformComponent>().Init(new Vector3(-300, 0, 0));

            InputComponent input = player.AddComponent<InputComponent>();
            input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_LEFT);
            input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_RIGHT);
            input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_UP);
            input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_DOWN);

            player.AddComponent<PhysicsComponent>().Init(0.9f, 1.0f, 0.0f);
            RectColliderComponent rect = player.AddComponent<RectColliderComponent>();
            rect.Init(50, 50, BodyType.Dynamic);

            player.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(2, 2));

            player.AddComponent<AttributesComponent>();

            IEntity player2 = EntityFactory.Create<Entity>("Player2");
            player2.AddComponent<TransformComponent>().Init(new Vector3(-200, 0, 0));

            InputComponent input2 = player2.AddComponent<InputComponent>();
            input2.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_A);
            input2.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_D);
            input2.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_W);
            input2.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_S);

            player2.AddComponent<PhysicsComponent>().Init(0.9f, 1.0f, 0.0f);
            player2.AddComponent<RectColliderComponent>().Init(50, 50, BodyType.Dynamic);

            player2.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(2, 2));


            IEntity camera = EntityFactory.Create<Entity>("Camera");

            CameraComponent cam = camera.AddComponent<CameraComponent>();
            cam.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam.AddTarget(player);

            IEntity camera2 = EntityFactory.Create<Entity>("Camera");

            CameraComponent cam2 = camera2.AddComponent<CameraComponent>();
            cam2.Init(GraphicsDevice, width / 2, height, AssetManager.Get().Find<Texture2D>("GroundTile1"), AssetManager.Get().Find<Effect>("LightingEffect"));
            cam2.AddTarget(player2);

            IEntity rock2 = EntityFactory.Create<Entity>("Rock2");
            rock2.AddComponent<TransformComponent>().Init(new Vector3(100, 0, 0));
            rock2.AddComponent<RectColliderComponent>().Init(50, 50, Vector2.Zero, BodyType.Dynamic, 10.0f);

            IEntity rock3 = EntityFactory.Create<Entity>("Rock3");
            rock3.AddComponent<TransformComponent>().Init(new Vector3(200, 0, 0));
            rock3.AddComponent<CircleColliderComponent>().Init(30, Vector2.Zero, BodyType.Dynamic, 10.0f);
            rock3.AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"));

            IEntity wall = EntityFactory.Create<Entity>("Wall");
            wall.AddComponent<TransformComponent>().Init(new Vector3(0, 300, 0));
            wall.AddComponent<WallColliderComponent>().Init(new Vector2(0, 0), new Vector2(100, -100));

            int[,] map = new int[,]
            {
                { 128, 128, 128, 128, 128, 128, 128 },
                { 128, 128, 128, 128, 128, 128, 128 },
                { 128, 128, 112, 112, 112, 128, 128 },
                { 128, 112, 112, 112, 112, 112, 128 },
                { 128, 128, 112, 112, 112, 128, 128 },
                { 128, 128, 128, 128, 128, 128, 128 },
                { 128, 128, 128, 128, 128, 128, 128 },
            };

            IEntity test = EntityFactory.Create<Entity>("test");
            test.AddComponent<AreaSpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("Tileset1"), 16, 16, map);
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Get().Content = Content;

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
            AssetManager.Get().Store<Texture2D>("GroundTile1", "Graphics/Tiles/GroundTile1");

            AssetManager.Get().Store<Texture2D>("ChestClosed1", "Graphics/Tiles/ChestClosed1");
            AssetManager.Get().Store<Texture2D>("ChestOpened1", "Graphics/Tiles/ChestOpened1");

            AssetManager.Get().Store<Texture2D>("Tileset1", "Graphics/Tilesets/Tileset1");

            AssetManager.Get().Store<Texture2D>("CircleLight", "Graphics/Lights/CircleLight");
            AssetManager.Get().Store<Effect>("LightingEffect", "Effects/DeferredLighting");
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            ComponentManager.Get().TickComponents(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            PhysicsManager.Get().Step(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            CameraManager.Get().Render(GraphicsDevice, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
