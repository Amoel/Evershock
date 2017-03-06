using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EvershockGame.Code.Manager
{
    public delegate void CommandDelegate(object arg);

    //---------------------------------------------------------------------------

    public class ConsoleManager : BaseManager<ConsoleManager>
    {
        private Dictionary<string, ConsoleCommand> m_Commands;
        private List<ConsoleLog> m_Logs;
        private KeyboardState m_State;

        private List<string> m_Inputs;
        private int m_InputIndex;
        private string m_Input;
        private string m_Autocomplete;

        private float m_Time;

        public SpriteFont Font { get; set; }
        public bool IsVisible { get; private set; }

        //---------------------------------------------------------------------------

        protected ConsoleManager()
        {
            m_Commands = new Dictionary<string, ConsoleCommand>();
            m_Logs = new List<ConsoleLog>();
            m_State = Keyboard.GetState();

            m_Inputs = new List<string>();
            m_Input = string.Empty;
            m_Autocomplete = string.Empty;
        }

        //---------------------------------------------------------------------------

        public void RegisterCommand(string name, object target, Delegate function)
        {
            if (!m_Commands.ContainsKey(name))
            {
                m_Commands.Add(name, new ConsoleCommand(target, function));
            }
        }

        //---------------------------------------------------------------------------

        private void ToggleConsole(bool isVisible)
        {
            IsVisible = isVisible;
        }

        //---------------------------------------------------------------------------

        public void Tick()
        {
            KeyboardState state = Keyboard.GetState();
            var keys = state.GetPressedKeys().Except(m_State.GetPressedKeys());
            bool shift = (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift));

            foreach (Keys key in keys)
            {
                if (IsVisible)
                {
                    switch (key)
                    {
                        case Keys.LeftShift:
                        case Keys.RightShift:
                        case Keys.LeftAlt:
                        case Keys.RightAlt:
                        case Keys.LeftControl:
                        case Keys.RightControl:
                            break;
                        case Keys.OemPipe:
                            ToggleConsole(false);
                            break;
                        case Keys.Tab:
                            if (m_Commands.ContainsKey(m_Autocomplete))
                            {
                                m_Input = m_Autocomplete;
                            }
                            break;
                        case Keys.Enter:
                            Execute();
                            break;
                        case Keys.Back:
                            if (m_Input.Length > 0)
                            {
                                m_Input = m_Input.Substring(0, m_Input.Length - 1);
                                UpdateAutocomplete();
                            }
                            break;
                        case Keys.Up:
                            break;
                        case Keys.Down:
                            break;
                        default:
                            char? chr = GetChar(key, shift);
                            if (chr != null)
                            {
                                m_Input += chr.Value;
                                UpdateAutocomplete();
                            }
                            break;
                    }
                }
                else
                {
                    switch (key)
                    {
                        case Keys.OemPipe:
                            ToggleConsole(true);
                            break;
                    }
                }
            }

            m_State = state;
        }

        //---------------------------------------------------------------------------

        private void UpdateAutocomplete()
        {
            foreach (string name in m_Commands.Keys)
            {
                if (name.StartsWith(m_Input))
                {
                    m_Autocomplete = name;
                    return;
                }
            }
            m_Autocomplete = m_Input;
        }

        //---------------------------------------------------------------------------

        private void Execute()
        {
            Match name = Regex.Match(m_Input, @"^([\w\-]+)");

            if (m_Commands.ContainsKey(name.Value))
            {
                m_Logs.Add(m_Commands[name.Value].Execute(m_Input));
            }
            else
            {
                m_Logs.Add(new ConsoleLog(m_Input, "Unknown command"));
            }

            m_Inputs.Add(m_Input);
            m_InputIndex = m_Inputs.Count;
            m_Input = string.Empty;
            UpdateAutocomplete();
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, float deltaTime)
        {
            m_Time += deltaTime;

            if (IsVisible && Font != null)
            {
                float textHeight = Font.MeasureString("X").Y;
                float screenHeight = UIManager.Get().ScreenBounds.Height;

                DrawText(batch, string.Format(">{0}", m_Autocomplete), new Vector2(10, screenHeight - textHeight), Color.Gray);
                DrawText(batch, string.Format(">{0}{1}", m_Input, (int)(m_Time * 2) % 2 == 0 ? "|" : ""), new Vector2(10, screenHeight - textHeight), Color.White);
                for (int i = 0; i < m_Logs.Count; i++)
                {
                    DrawText(batch, m_Logs[i].Input, new Vector2(10, (screenHeight - textHeight) - ((textHeight + 4) * (i * 2 + 2))), Color.White);
                    DrawText(batch, m_Logs[i].Message, new Vector2(25, (screenHeight - textHeight) - ((textHeight + 4) * (i * 2 + 1))), Color.Cyan);
                }
            }
        }

        //---------------------------------------------------------------------------

        private Vector2 DrawText(SpriteBatch batch, string text, Vector2 position, Color color)
        {
            if (Font != null)
            {
                batch.DrawString(Font, text, position, color);
                return new Vector2(position.X + Font.MeasureString(text).X, position.Y);
            }
            return position;
        }

        //---------------------------------------------------------------------------

        private char? GetChar(Keys key, bool shift = false)
        {
            switch (key)
            {
                case Keys.A: return shift ? 'A' : 'a';
                case Keys.B: return shift ? 'B' : 'b';
                case Keys.C: return shift ? 'C' : 'c';
                case Keys.D: return shift ? 'D' : 'd';
                case Keys.E: return shift ? 'E' : 'e';
                case Keys.F: return shift ? 'F' : 'f';
                case Keys.G: return shift ? 'G' : 'g';
                case Keys.H: return shift ? 'H' : 'h';
                case Keys.I: return shift ? 'I' : 'i';
                case Keys.J: return shift ? 'J' : 'j';
                case Keys.K: return shift ? 'K' : 'k';
                case Keys.L: return shift ? 'L' : 'l';
                case Keys.M: return shift ? 'M' : 'm';
                case Keys.N: return shift ? 'N' : 'n';
                case Keys.O: return shift ? 'O' : 'o';
                case Keys.P: return shift ? 'P' : 'p';
                case Keys.Q: return shift ? 'Q' : 'q';
                case Keys.R: return shift ? 'R' : 'r';
                case Keys.S: return shift ? 'S' : 's';
                case Keys.T: return shift ? 'T' : 't';
                case Keys.U: return shift ? 'U' : 'u';
                case Keys.V: return shift ? 'V' : 'v';
                case Keys.W: return shift ? 'W' : 'w';
                case Keys.X: return shift ? 'X' : 'x';
                case Keys.Y: return shift ? 'Y' : 'y';
                case Keys.Z: return shift ? 'Z' : 'z';
                case Keys.OemComma: return ',';
                case Keys.OemPeriod: return '.';
                case Keys.Space: return ' ';

                case Keys.D0:
                case Keys.NumPad0:
                    return shift ? '=' : '0';
                case Keys.D1:
                case Keys.NumPad1:
                    return shift ? '!' : '1';
                case Keys.D2:
                case Keys.NumPad2:
                    return shift ? '"' : '2';
                case Keys.D3:
                case Keys.NumPad3:
                    return shift ? '§' : '3';
                case Keys.D4:
                case Keys.NumPad4:
                    return shift ? '$' : '4';
                case Keys.D5:
                case Keys.NumPad5:
                    return shift ? '%' : '5';
                case Keys.D6:
                case Keys.NumPad6:
                    return shift ? '&' : '6';
                case Keys.D7:
                case Keys.NumPad7:
                    return shift ? '/' : '7';
                case Keys.D8:
                case Keys.NumPad8:
                    return shift ? '(' : '8';
                case Keys.D9:
                case Keys.NumPad9:
                    return shift ? ')' : '9';

                default: return null;
            }
        }
    }

    //---------------------------------------------------------------------------

    public struct ConsoleLog
    {
        public string Input { get; private set; }
        public string Message { get; private set; }

        //---------------------------------------------------------------------------

        public ConsoleLog(string input, string message) : this()
        {
            Input = string.Format("[{0}]", input);
            Message = message;
        }

        //---------------------------------------------------------------------------

        public static ConsoleLog Empty
        {
            get { return new ConsoleLog(); }
        }
    }

    //---------------------------------------------------------------------------

    public class ConsoleCommand
    {
        public object Target { get; private set; }
        public Delegate Function { get; private set; }

        //---------------------------------------------------------------------------

        public ConsoleCommand(object target, Delegate function)
        {
            Target = target;
            Function = function;
        }

        //---------------------------------------------------------------------------

        public ConsoleLog Execute(string command)
        {
            if (Function != null)
            {
                //try
                //{
                Match match = Regex.Match(command, @"\b[^()]+\((.*)\)$");
                string[] args = match.Groups[1].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<object> arguments = new List<object>();
                ParameterInfo[] infos = Function.Method.GetParameters();
                if (infos.Length == args.Length)
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        if (infos[i].ParameterType == typeof(int))
                        {
                            arguments.Add(int.Parse(args[i]));
                        }
                        else if (infos[i].ParameterType == typeof(float))
                        {
                            arguments.Add(float.Parse(args[i]));
                        }
                        else if (infos[i].ParameterType == typeof(string))
                        {
                            arguments.Add(args[i]);
                        }
                    }
                }
                else
                {
                    return new ConsoleLog(command, "Argument mismatch");
                }
                var data = Function.DynamicInvoke(arguments.ToArray());
                if (data != null)
                {
                    return new ConsoleLog(command, data.ToString());
                }
                //}
                //catch (Exception e)
                //{
                //    return new ConsoleLog(command, e.Message);
                //}
            }
            return new ConsoleLog(command, string.Empty);
        }
    }
}
