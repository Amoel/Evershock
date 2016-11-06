using EntityComponent;
using EntityComponent.Factory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using EvershockGame.Code;
using EntityComponent.Components;
using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using System.Diagnostics;

namespace EvershockGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D backgroundTex1;
        Texture2D backgroundTex2;

        Texture2D lightingTex;

        Effect lightingEffect;

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

            player.AddComponent<LightingComponent>().Init(lightingTex, Vector2.Zero, new Vector2(2, 2));

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

            player2.AddComponent<LightingComponent>().Init(lightingTex, Vector2.Zero, new Vector2(2, 2));


            IEntity camera = EntityFactory.Create<Entity>("Camera");

            CameraComponent cam = camera.AddComponent<CameraComponent>();
            cam.Init(GraphicsDevice, width, height, backgroundTex1, lightingEffect);
            cam.AddTarget(player);
            cam.AddTarget(player2);

            IEntity rock = EntityFactory.Create<Entity>("Rock1");
            rock.AddComponent<TransformComponent>().Init(new Vector3(0, 0, 0));
            rock.AddComponent<RectColliderComponent>().Init(100, 100, Vector2.Zero, BodyType.Static, 10.0f);

            IEntity rock2 = EntityFactory.Create<Entity>("Rock2");
            rock2.AddComponent<TransformComponent>().Init(new Vector3(100, 0, 0));
            rock2.AddComponent<RectColliderComponent>().Init(50, 50, Vector2.Zero, BodyType.Dynamic, 10.0f);

            IEntity rock3 = EntityFactory.Create<Entity>("Rock3");
            rock3.AddComponent<TransformComponent>().Init(new Vector3(200, 0, 0));
            rock3.AddComponent<CircleColliderComponent>().Init(30, Vector2.Zero, BodyType.Dynamic, 10.0f);
            rock3.AddComponent<LightingComponent>().Init(lightingTex);

            IEntity wall = EntityFactory.Create<Entity>("Wall");
            wall.AddComponent<TransformComponent>().Init(new Vector3(0, 300, 0));
            wall.AddComponent<WallColliderComponent>().Init(new Vector2(0, 0), new Vector2(100, -100));
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
#if DEBUG
            // Load debug content
            CollisionManager.Get().RectTexture = Content.Load<Texture2D>("Graphics/Debug/RectTextureDebug");
            CollisionManager.Get().CircleTexture = Content.Load<Texture2D>("Graphics/Debug/CircleTextureDebug");

            Texture2D pointTex = new Texture2D(GraphicsDevice, 1, 1);
            pointTex.SetData(new Color[] { new Color(0.812f, 0.067f, 0.153f) });
            CollisionManager.Get().PointTexture = pointTex;
#endif

            backgroundTex1 = Content.Load<Texture2D>("Graphics/Camera/BackgroundTexture1");
            backgroundTex2 = Content.Load<Texture2D>("Graphics/Camera/BackgroundTexture2");

            lightingTex = Content.Load<Texture2D>("Graphics/Lights/CircleLight");

            lightingEffect = Content.Load<Effect>("Effects/DeferredLighting");
        }
        
        protected override void UnloadContent()
        {
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

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
