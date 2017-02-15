using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EvershockGame.Components.UI
{
    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class TextComponent : Component, IDrawableUIComponent
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public EHorizontalAlignment TextAlignment { get; set; }
        public bool IsWrapping { get; set; }
        public int Spacing { get; set; }

        //---------------------------------------------------------------------------

        public TextComponent(Guid entity) : base(entity)
        {
            Text = Name;
            TextAlignment = EHorizontalAlignment.Left;
            Spacing = 3;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, float deltaTime)
        {
            if (Font != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    Rectangle bounds = transform.Bounds();

                    int offset = 0;
                    int width = 0;
                    int height = (int)Font.MeasureString("X").Y + Spacing;
                    int line = 0;
                    List<TextSegment> segments = ParseToSegments();
                    foreach (TextSegment segment in segments)
                    {
                        width += (int)Font.MeasureString(segment.Text).X;
                    }

                    if (IsWrapping)
                    {
                        foreach (LineSegment lineSegment in ParseToLines(segments, bounds.Width))
                        {
                            foreach (TextSegment textSegment in lineSegment.Texts)
                            {
                                switch (TextAlignment)
                                {
                                    case EHorizontalAlignment.Left:
                                        DrawText(batch, textSegment.Text, new Vector2(bounds.X + offset, bounds.Y + lineSegment.LineIndex * height), textSegment.Color, true);
                                        break;
                                    case EHorizontalAlignment.Right:
                                        DrawText(batch, textSegment.Text, new Vector2((bounds.X + bounds.Width) - lineSegment.Width + offset, bounds.Y + lineSegment.LineIndex * height), textSegment.Color, true);
                                        break;
                                    case EHorizontalAlignment.Center:
                                        DrawText(batch, textSegment.Text, new Vector2((bounds.X + bounds.Width / 2) - lineSegment.Width / 2 + offset, bounds.Y + lineSegment.LineIndex * height), textSegment.Color, true);
                                        break;
                                }
                                offset += textSegment.GetWidth(Font);
                            }
                            offset = 0;
                        }
                    }

                    foreach (TextSegment segment in segments)
                    {
                        if (IsWrapping)
                        {
                            //int wrapIndex = 0;
                            //do
                            //{
                            //    int newWrapIndex = 0;
                            //    bool isWrapping = (Wrap(segment.Text.Substring(wrapIndex), bounds.Width - offset, out newWrapIndex));
                            //    string substring = segment.Text.Substring(wrapIndex, newWrapIndex);
                            //    if (offset == 0) substring = substring.TrimStart(' ');
                            //    switch (TextAlignment)
                            //    {
                            //        case EHorizontalAlignment.Left:
                            //            DrawText(batch, substring, new Vector2(bounds.X + offset, bounds.Y + line * height), segment.Color, true);
                            //            break;
                            //        case EHorizontalAlignment.Right:
                            //            DrawText(batch, substring, new Vector2((bounds.X + bounds.Width) - width + offset, bounds.Y + line * height), segment.Color, true);
                            //            break;
                            //        case EHorizontalAlignment.Center:
                            //            DrawText(batch, substring, new Vector2((bounds.X + bounds.Width / 2) - width / 2 + offset, bounds.Y + line * height), segment.Color, true);
                            //            break;
                            //    }
                            //    if (isWrapping)
                            //    {
                            //        line++;
                            //        offset = 0;
                            //    }
                            //    else
                            //    {
                            //        offset += (int)Font.MeasureString(segment.Text.Substring(wrapIndex, newWrapIndex)).X;
                            //    }
                            //    wrapIndex += newWrapIndex;
                            //}
                            //while (wrapIndex < segment.Text.Length);
                        }
                        else
                        {
                            switch (TextAlignment)
                            {
                                case EHorizontalAlignment.Left:
                                    DrawText(batch, segment.Text, new Vector2(bounds.X + offset, bounds.Y + line * height), segment.Color, true);
                                    break;
                                case EHorizontalAlignment.Right:
                                    DrawText(batch, segment.Text, new Vector2((bounds.X + bounds.Width) - width + offset, bounds.Y + line * height), segment.Color, true);
                                    break;
                                case EHorizontalAlignment.Center:
                                    DrawText(batch, segment.Text, new Vector2((bounds.X + bounds.Width / 2) - width / 2 + offset, bounds.Y + line * height), segment.Color, true);
                                    break;
                            }

                            offset += (int)Font.MeasureString(segment.Text).X;
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private void DrawText(SpriteBatch batch, string text, Vector2 location, Color color, bool hasShadow)
        {
            if (hasShadow)
            {
                batch.DrawString(Font, text, new Vector2(location.X, location.Y + 2), Color.Black);
            }
            batch.DrawString(Font, text, location, color);
        }

        //---------------------------------------------------------------------------

        private bool Wrap(string text, int maxWidth, out int index)
        {
            string pattern = "( )";
            int width = 0;
            index = 0;
            foreach(string word in Regex.Split(text, pattern))
            {
                if (string.IsNullOrEmpty(word)) continue;
                if (word.Contains('\n'))
                {
                    index += word.IndexOf('\n') + 1;
                    return true;
                }
                width += (int)Font.MeasureString(word).X;
                if (width > maxWidth)
                {
                    return true;
                }
                index += word.Length;
            }
            index = text.Length;
            return false;
        }

        //---------------------------------------------------------------------------

        private List<TextSegment> ParseToSegments()
        {
            List<TextSegment> segments = new List<TextSegment>();

            MatchCollection matches = Regex.Matches(Text, "<[A-Za-z/]+>");
            Stack<Match> open = new Stack<Match>();

            int index = 0;
            foreach (Match match in matches)
            {
                if (match.Value.Contains('/'))
                {
                    if (AssertManager.Get().Show(open.Count > 0 && IsClosingTag(open.Peek().Value, match.Value), "Tags in TextComponent don't match."))
                    {
                        return new List<TextSegment>() { new TextSegment(Text) };
                    }
                    Match tag = open.Pop();
                    if (match.Index > index)
                    {
                        segments.Add(TextSegment.Parse(Text.Substring(index, match.Index - index), tag.Value));
                    }
                    index = match.Index + match.Length;
                }
                else
                {
                    if (match.Index > index)
                    {
                        if (open.Count > 0)
                        {
                            segments.Add(TextSegment.Parse(Text.Substring(index, match.Index - index), open.Peek().Value));
                        }
                        else
                        {
                            segments.Add(new TextSegment(Text.Substring(index, match.Index - index)));
                        }
                    }
                    open.Push(match);
                    index = match.Index + match.Length;
                }
            }
            if (index < Text.Length)
            {
                segments.Add(new TextSegment(Text.Substring(index)));
            }

            return segments;
        }

        //---------------------------------------------------------------------------

        private List<LineSegment> ParseToLines(List<TextSegment> segments, int maxWidth)
        {
            List<LineSegment> lines = new List<LineSegment>();
            
            int lineIndex = 0;
            LineSegment line = new LineSegment(lineIndex);
            
            foreach (TextSegment segment in segments)
            {
                int wrapIndex = 0;
                do
                {
                    int newWrapIndex = 0;
                    if (Wrap(segment.Text.Substring(wrapIndex), maxWidth - line.Width, out newWrapIndex))
                    {
                        TextSegment extractedSegment = segment.Extract(wrapIndex, newWrapIndex);
                        if (!string.IsNullOrWhiteSpace(extractedSegment.Text))
                        {
                            line.Add(extractedSegment, Font);
                        }
                        line.Trim(Font);
                        lines.Add(line);
                        line = new LineSegment(++lineIndex);
                    }
                    else
                    {
                        if (wrapIndex > 0)
                        {
                            line.Add(segment.Extract(wrapIndex, segment.Text.Length - wrapIndex), Font);
                        }
                        else
                        {
                            line.Add(segment, Font);
                        }
                    }
                    wrapIndex += newWrapIndex;
                }
                while (wrapIndex < segment.Text.Length);
            }
            line.Trim(Font);
            lines.Add(line);

            return lines;
        }

        //---------------------------------------------------------------------------

        private bool IsClosingTag(string tag, string closingTag)
        {
            return Regex.IsMatch(closingTag, string.Format("</{0}", tag.Substring(1)));
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }

        //---------------------------------------------------------------------------

        class TextSegment
        {
            public Color Color { get; set; }
            public string Text { get; set; }

            //---------------------------------------------------------------------------

            public TextSegment(string text)
            {
                Text = text;
                Color = Color.White;
            }

            //---------------------------------------------------------------------------

            public TextSegment(string text, Color color)
            {
                Text = text;
                Color = color;
            }

            //---------------------------------------------------------------------------

            public int GetWidth(SpriteFont font)
            {
                return (int)font.MeasureString(Text).X;
            }

            //---------------------------------------------------------------------------

            public void Split(int index, out TextSegment left, out TextSegment right)
            {
                left = new TextSegment(Text.Substring(0, index), Color);
                right = new TextSegment(Text.Substring(index).TrimStart(' '), Color);
            }

            //---------------------------------------------------------------------------

            public TextSegment Extract(int index, int length)
            {
                return new TextSegment(Text.Substring(index, length), Color);
            }

            //---------------------------------------------------------------------------

            public static TextSegment Parse(string text, string tag)
            {
                PropertyInfo property = typeof(Color).GetProperty(tag.Substring(1, tag.Length - 2));
                if (property != null)
                {
                    return new TextSegment(text, (Color)property.GetValue(null, null));
                }
                return new TextSegment(text);
            }
        }

        //---------------------------------------------------------------------------

        class LineSegment
        {
            public List<TextSegment> Texts { get; private set; }
            public int LineIndex { get; set; }
            public int Width { get; set; }

            //---------------------------------------------------------------------------

            public LineSegment(int lineIndex)
            {
                Texts = new List<TextSegment>();
                LineIndex = lineIndex;
            }

            //---------------------------------------------------------------------------

            public void Add(TextSegment text, SpriteFont font)
            {
                Texts.Add(text);
                Width += text.GetWidth(font);
            }

            //---------------------------------------------------------------------------

            public void Trim(SpriteFont font)
            {
                if (Texts.Count > 0)
                {
                    TextSegment first = Texts.First();
                    first.Text = first.Text.TrimStart(' ');

                    TextSegment last = Texts.Last();
                    last.Text = last.Text.TrimEnd(' ');
                }
                Width = Texts.Sum(text => text.GetWidth(font));
            }
        }
    }
}
