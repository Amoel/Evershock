using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityComponent.Components.UI
{
    [Serializable]
    [RequireComponent(typeof(UITransformComponent))]
    public class TextComponent : Component, IDrawableUIComponent
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public EHorizontalAlignment TextAlignment { get; set; }

        //---------------------------------------------------------------------------

        public TextComponent(Guid entity) : base(entity)
        {
            Text = Name;
            TextAlignment = EHorizontalAlignment.Left;
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch)
        {
            if (Font != null)
            {
                UITransformComponent transform = GetComponent<UITransformComponent>();
                if (transform != null)
                {
                    Rectangle bounds = transform.Bounds();

                    int offset = 0;
                    int width = 0;
                    List<TextSegment> segments = ParseText();
                    foreach (TextSegment segment in segments)
                    {
                        width += (int)Font.MeasureString(segment.Text).X;
                    }

                    foreach (TextSegment segment in segments)
                    {
                        switch (TextAlignment)
                        {
                            case EHorizontalAlignment.Left:
                                batch.DrawString(Font, segment.Text, new Vector2(bounds.X + offset, bounds.Y), segment.Color);
                                break;
                            case EHorizontalAlignment.Right:
                                batch.DrawString(Font, segment.Text, new Vector2((bounds.X + bounds.Width) - width + offset, bounds.Y), segment.Color);
                                break;
                            case EHorizontalAlignment.Center:
                                batch.DrawString(Font, segment.Text, new Vector2((bounds.X + bounds.Width / 2) - width / 2 + offset, bounds.Y), segment.Color);
                                break;
                        }
                        offset += (int)Font.MeasureString(segment.Text).X;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private List<TextSegment> ParseText()
        {
            List<TextSegment> segments = new List<TextSegment>();

            MatchCollection matches = Regex.Matches(Text, "<[A-Za-z/]+>");
            Stack<Match> open = new Stack<Match>();

            int index = 0;
            bool addText = true;
            foreach (Match match in matches)
            {
                if (match.Value.Contains('/'))
                {
                    if (AssertManager.Get().Show(open.Count > 0 && IsClosingTag(open.Peek().Value, match.Value), "Tags in TextComponent don't match."))
                    {
                        return new List<TextSegment>() { new TextSegment(Text) };
                    }
                    Match tag = open.Pop();
                    if (match.Index > tag.Index + tag.Length && addText)
                    {
                        segments.Add(TextSegment.Parse(Text.Substring(tag.Index + tag.Length, match.Index - (tag.Index + tag.Length)), tag.Value));
                    }
                    index = match.Index + match.Length;
                    addText = false;
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
                    addText = true;
                }
            }

            return segments;
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
            public Color Color { get; private set; }
            public string Text { get; private set; }

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
    }
}
