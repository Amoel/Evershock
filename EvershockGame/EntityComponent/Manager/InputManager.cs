using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityComponent.Manager
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EInput
    {
        // Gamepad
        GAMEPAD_A,
        GAMEPAD_B,
        GAMEPAD_X,
        GAMEPAD_Y,
        GAMEPAD_BUMPER_LEFT,
        GAMEPAD_BUMPER_RIGHT,
        GAMEPAD_THUMBSTICK_LEFT,
        GAMEPAD_THUMBSTICK_LEFT_LEFT,
        GAMEPAD_THUMBSTICK_LEFT_RIGHT,
        GAMEPAD_THUMBSTICK_LEFT_UP,
        GAMEPAD_THUMBSTICK_LEFT_DOWN,
        GAMEPAD_THUMBSTICK_RIGHT,
        GAMEPAD_THUMBSTICK_RIGHT_LEFT,
        GAMEPAD_THUMBSTICK_RIGHT_RIGHT,
        GAMEPAD_THUMBSTICK_RIGHT_UP,
        GAMEPAD_THUMBSTICK_RIGHT_DOWN,
        GAMEPAD_TRIGGER_LEFT,
        GAMEPAD_TRIGGER_RIGHT,
        GAMEPAD_DPAD_LEFT,
        GAMEPAD_DPAD_RIGHT,
        GAMEPAD_DPAD_UP,
        GAMEPAD_DPAD_DOWN,

        // Keyboard
        KEYBOARD_LEFT,
        KEYBOARD_RIGHT,
        KEYBOARD_UP,
        KEYBOARD_DOWN,
        KEYBOARD_SPACE,
        KEYBOARD_TAB,
        KEYBOARD_BACK,
        KEYBOARD_ENTER,

        KEYBOARD_W,
        KEYBOARD_A,
        KEYBOARD_S,
        KEYBOARD_D,
    }

    //---------------------------------------------------------------------------

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EGameAction
    {
        MOVE_UP,
        MOVE_DOWN,
        MOVE_LEFT,
        MOVE_RIGHT,
        MENU_UP,
        MENU_DOWN,
        MENU_LEFT,
        MENU_RIGHT,
        MENU_BACK,

        ADD_HEALTH,
        REDUCE_HEALTH,

        TAKE_SCREENSHOT
    }

    //---------------------------------------------------------------------------

    public class InputManager : BaseManager<InputManager>
    {
        private Dictionary<EInput, Keys> m_KeyboardMapping = new Dictionary<EInput, Keys>()
        {
            { EInput.KEYBOARD_LEFT, Keys.Left },
            { EInput.KEYBOARD_RIGHT, Keys.Right },
            { EInput.KEYBOARD_UP, Keys.Up },
            { EInput.KEYBOARD_DOWN, Keys.Down },

            { EInput.KEYBOARD_SPACE, Keys.Space },
            { EInput.KEYBOARD_TAB, Keys.Tab },
            { EInput.KEYBOARD_BACK, Keys.Back },
            { EInput.KEYBOARD_ENTER, Keys.Enter },

            { EInput.KEYBOARD_W, Keys.W },
            { EInput.KEYBOARD_A, Keys.A },
            { EInput.KEYBOARD_S, Keys.S },
            { EInput.KEYBOARD_D, Keys.D },
        };

        //---------------------------------------------------------------------------

        protected InputManager() { }

        //---------------------------------------------------------------------------

        public float GetValue(EInput input, PlayerIndex playerIndex)
        {
            switch (input)
            {
                case EInput.GAMEPAD_A:
                    return GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_B:
                    return GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_X:
                    return GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_Y:
                    return GamePad.GetState(playerIndex).Buttons.Y == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_BUMPER_LEFT:
                    return GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_BUMPER_RIGHT:
                    return GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_THUMBSTICK_LEFT:
                    return GamePad.GetState(playerIndex).Buttons.LeftStick == ButtonState.Pressed ? 1.0f : 0.0f;

#if DEBUG
                case EInput.GAMEPAD_THUMBSTICK_LEFT_LEFT:
                    return Keyboard.GetState().IsKeyDown(Keys.Left) ? 1.0f : 0.0f;
                case EInput.GAMEPAD_THUMBSTICK_LEFT_RIGHT:
                    return Keyboard.GetState().IsKeyDown(Keys.Right) ? 1.0f : 0.0f;
                case EInput.GAMEPAD_THUMBSTICK_LEFT_UP:
                    return Keyboard.GetState().IsKeyDown(Keys.Up) ? 1.0f : 0.0f;
                case EInput.GAMEPAD_THUMBSTICK_LEFT_DOWN:
                    return Keyboard.GetState().IsKeyDown(Keys.Down) ? 1.0f : 0.0f;
#else
                case EInput.GAMEPAD_THUMBSTICK_LEFT_LEFT:
                    return MathHelper.Clamp(-GamePad.GetState(playerIndex).ThumbSticks.Left.X, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_LEFT_RIGHT:
                    return MathHelper.Clamp(GamePad.GetState(playerIndex).ThumbSticks.Left.X, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_LEFT_UP:
                    return MathHelper.Clamp(-GamePad.GetState(playerIndex).ThumbSticks.Left.Y, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_LEFT_DOWN:
                    return MathHelper.Clamp(GamePad.GetState(playerIndex).ThumbSticks.Left.Y, 0.0f, 1.0f);
#endif

                case EInput.GAMEPAD_THUMBSTICK_RIGHT:
                    return GamePad.GetState(playerIndex).Buttons.RightStick == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_THUMBSTICK_RIGHT_LEFT:
                    return MathHelper.Clamp(-GamePad.GetState(playerIndex).ThumbSticks.Right.X, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_RIGHT_RIGHT:
                    return MathHelper.Clamp(GamePad.GetState(playerIndex).ThumbSticks.Right.X, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_RIGHT_UP:
                    return MathHelper.Clamp(-GamePad.GetState(playerIndex).ThumbSticks.Right.Y, 0.0f, 1.0f);
                case EInput.GAMEPAD_THUMBSTICK_RIGHT_DOWN:
                    return MathHelper.Clamp(GamePad.GetState(playerIndex).ThumbSticks.Right.Y, 0.0f, 1.0f);
                case EInput.GAMEPAD_TRIGGER_LEFT:
                    return GamePad.GetState(playerIndex).Triggers.Left;
                case EInput.GAMEPAD_TRIGGER_RIGHT:
                    return GamePad.GetState(playerIndex).Triggers.Right;
                case EInput.GAMEPAD_DPAD_LEFT:
                    return GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_DPAD_RIGHT:
                    return GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_DPAD_UP:
                    return GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed ? 1.0f : 0.0f;
                case EInput.GAMEPAD_DPAD_DOWN:
                    return GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed ? 1.0f : 0.0f;

                default:
                    if (m_KeyboardMapping.ContainsKey(input))
                    {
                        return Keyboard.GetState().IsKeyDown(m_KeyboardMapping[input]) ? 1.0f : 0.0f;
                    }
                    return 0.0f;
            }
        }
    }
}
