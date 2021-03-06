﻿using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
using EvershockGame.Code.Components;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using EvershockGame.Code.Factory;

namespace EvershockGame.Code
{
    public class Player : Entity
    {
        public static int PlayerCount { get; private set; }
        public PlayerIndex Index { get; private set; }

        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public InputComponent Input { get { return GetComponent<InputComponent>(); } }
        public MovementAnimationComponent Animation { get { return GetComponent<MovementAnimationComponent>(); } }
        public PlayerAttributesComponent Attributes { get { return GetComponent<PlayerAttributesComponent>(); } }

        //---------------------------------------------------------------------------

        public Player(string name, Guid parent) : base(name, parent)
        {
            Index = (PlayerIndex)PlayerCount++;

            AddComponent<TransformComponent>().Init(new Vector3(Index == PlayerIndex.One ? 340 : 420, 400, 0));
            AddComponent<PlayerAttributesComponent>().Init(500.0f, 250.0f, 125.0f, 2.0f, 1.0f);
            AddComponent<InventoryComponent>();

            MovementAnimationComponent animation = AddComponent<MovementAnimationComponent>();
            if (Index == PlayerIndex.One)
            {
                animation.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.WalkingAnimation), Vector2.One);
            }
            else
            {
                animation.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.WalkingAnimation2), Vector2.One);
            }
            animation.AddSetting((int)Tag.MoveLeft, new AnimationSetting(8, 2, 8, 15, true, true));
            animation.AddSetting((int)Tag.MoveRight, new AnimationSetting(8, 2, 0, 7, true));

            AddComponent<ShadowComponent>().Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.RedOrb), new Vector2(6.0f, 2.0f), new Vector2(0.0f, 10.0f));

            AddComponent<ActorPhysicsComponent>().Init(BodyType.Dynamic, 0.8f, 1.0f, true);

            CircleColliderComponent collider = AddComponent<CircleColliderComponent>();
            collider.Init(44, BodyType.Dynamic);
            //collider.SetCollisionCategory(ECollisionCategory.Player);

            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), Vector2.Zero, new Vector2(5.0f, 5.0f), Color.White, 0.6f);

            InputComponent input = AddComponent<InputComponent>();
            if (input != null)
            {
                input.Init(Index);
                input.ClearActions();
                switch (Index)
                {
                    case PlayerIndex.One:
                        input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_LEFT, EInput.GAMEPAD_THUMBSTICK_LEFT_LEFT);
                        input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_RIGHT, EInput.GAMEPAD_THUMBSTICK_LEFT_RIGHT);
                        input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_UP, EInput.GAMEPAD_THUMBSTICK_LEFT_UP);
                        input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_DOWN, EInput.GAMEPAD_THUMBSTICK_LEFT_DOWN);
                        input.MapAction(EGameAction.LOOK_LEFT, EInput.GAMEPAD_THUMBSTICK_RIGHT_LEFT);
                        input.MapAction(EGameAction.LOOK_RIGHT, EInput.GAMEPAD_THUMBSTICK_RIGHT_RIGHT);
                        input.MapAction(EGameAction.LOOK_UP, EInput.GAMEPAD_THUMBSTICK_RIGHT_UP);
                        input.MapAction(EGameAction.LOOK_DOWN, EInput.GAMEPAD_THUMBSTICK_RIGHT_DOWN);
                        input.MapAction(EGameAction.INVENTORY_NEXT_ITEM, EInput.KEYBOARD_E, EInput.GAMEPAD_DPAD_DOWN);
                        input.MapAction(EGameAction.INVENTORY_PREVIOUS_ITEM, EInput.KEYBOARD_Q, EInput.GAMEPAD_DPAD_UP);
                        input.MapAction(EGameAction.INVENTORY_USE_ITEM, EInput.GAMEPAD_X);
                        input.MapAction(EGameAction.INVENTORY_DROP_ITEM, EInput.GAMEPAD_TRIGGER_RIGHT);
                        input.MapAction(EGameAction.PLAYER_ATTACK, EInput.GAMEPAD_TRIGGER_LEFT);
                        break;
                    case PlayerIndex.Two:
                        input.MapAction(EGameAction.MOVE_LEFT, EInput.KEYBOARD_A, EInput.GAMEPAD_THUMBSTICK_LEFT_LEFT);
                        input.MapAction(EGameAction.MOVE_RIGHT, EInput.KEYBOARD_D, EInput.GAMEPAD_THUMBSTICK_LEFT_RIGHT);
                        input.MapAction(EGameAction.MOVE_UP, EInput.KEYBOARD_W, EInput.GAMEPAD_THUMBSTICK_LEFT_UP);
                        input.MapAction(EGameAction.MOVE_DOWN, EInput.KEYBOARD_S, EInput.GAMEPAD_THUMBSTICK_LEFT_DOWN);
                        input.MapAction(EGameAction.LOOK_LEFT, EInput.GAMEPAD_THUMBSTICK_RIGHT_LEFT);
                        input.MapAction(EGameAction.LOOK_RIGHT, EInput.GAMEPAD_THUMBSTICK_RIGHT_RIGHT);
                        input.MapAction(EGameAction.LOOK_UP, EInput.GAMEPAD_THUMBSTICK_RIGHT_UP);
                        input.MapAction(EGameAction.LOOK_DOWN, EInput.GAMEPAD_THUMBSTICK_RIGHT_DOWN);
                        input.MapAction(EGameAction.INVENTORY_NEXT_ITEM, EInput.KEYBOARD_E, EInput.GAMEPAD_DPAD_DOWN);
                        input.MapAction(EGameAction.INVENTORY_PREVIOUS_ITEM, EInput.KEYBOARD_Q, EInput.GAMEPAD_DPAD_UP);
                        input.MapAction(EGameAction.INVENTORY_USE_ITEM, EInput.GAMEPAD_X);
                        input.MapAction(EGameAction.INVENTORY_DROP_ITEM, EInput.GAMEPAD_TRIGGER_RIGHT);
                        input.MapAction(EGameAction.PLAYER_ATTACK, EInput.GAMEPAD_TRIGGER_LEFT);
                        break;
                }
            }

            //playerIndicatorP1 = EntityFactory.Create<Entity>(GUID, "PlayerIndicatorP1");
            //playerIndicatorP1.AddComponent<TransformComponent>().Init(new Vector3(0, -65, 0));
            //piAnimationP1 = playerIndicatorP1.AddComponent<AnimationComponent>();
            //piAnimationP1.Init(AssetManager.Get().Find<Texture2D>("PlayerIndicatorAnimationP1"), new Vector2(0.4f, 0.4f));
            //piAnimationP1.AddSetting(0, new AnimationSetting(8, 1, 0, 7, false));
            //playerIndicatorP1.Disable();

            //RangeWeapon weapon = EntityFactory.Create<RangeWeapon>(GUID, "Primary Weapon P1");
            //weapon.Init();

            //if (Index == PlayerIndex.One)
            //{
            //    //GetComponent<ActorPhysicsComponent>().AddJoint(weapon.GetComponent<PhysicsComponent>());
            //}

            MeleeWeapon weapon = EntityFactory.Create<MeleeWeapon>(GUID, "Primary Weapon P1");
            if (Index == PlayerIndex.One)
            {
                GetComponent<ActorPhysicsComponent>().AddJoint(weapon.GetComponent<PhysicsComponent>());
            }
        }
    }
}
