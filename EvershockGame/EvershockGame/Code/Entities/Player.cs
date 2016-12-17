using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class Player : Entity
    {
        public static int PlayerCount { get; private set; }
        public PlayerIndex Index { get; private set; }

        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public InputComponent Input { get { return GetComponent<InputComponent>(); } }
        public MovementAnimationComponent Animation { get { return GetComponent<MovementAnimationComponent>(); } }

        //---------------------------------------------------------------------------

        public Player(string name) : base(name)
        {
            Index = (PlayerIndex)PlayerCount++;

            AddComponent<TransformComponent>().Init(new Vector3(Index == PlayerIndex.One ? 400 : 520, 450, 0));
            AddComponent<AttributesComponent>().Init(0, 130.0f);

            MovementAnimationComponent animation = AddComponent<MovementAnimationComponent>();
            animation.Init(AssetManager.Get().Find<Texture2D>("WalkingAnimation"), new Vector2(0.5f, 0.5f));
            animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true));
            animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7));

            AddComponent<ActorPhysicsComponent>().Init(0.9f, 1.0f, 0.0f);

            CircleColliderComponent collider = AddComponent<CircleColliderComponent>();
            collider.Init(22, BodyType.Dynamic);
            //collider.SetCollisionCategory(ECollisionCategory.Player);

            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"), Vector2.Zero, new Vector2(5, 5), Color.Blue, 1.0f);

            InputComponent input = AddComponent<InputComponent>();
            if (input != null)
            {
                input.ClearActions();
                switch (Index)
                {
                    case PlayerIndex.One:
                        input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_LEFT);
                        input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_RIGHT);
                        input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_UP);
                        input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_DOWN);
                        break;
                    case PlayerIndex.Two:
                        input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_A);
                        input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_D);
                        input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_W);
                        input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_S);
                        break;
                }
            }

            //playerIndicatorP1 = EntityFactory.Create<Entity>(GUID, "PlayerIndicatorP1");
            //playerIndicatorP1.AddComponent<TransformComponent>().Init(new Vector3(0, -65, 0));
            //piAnimationP1 = playerIndicatorP1.AddComponent<AnimationComponent>();
            //piAnimationP1.Init(AssetManager.Get().Find<Texture2D>("PlayerIndicatorAnimationP1"), new Vector2(0.4f, 0.4f));
            //piAnimationP1.AddSetting(0, new AnimationSetting(8, 1, 0, 7, false));
            //playerIndicatorP1.Disable();
        }
    }
}
