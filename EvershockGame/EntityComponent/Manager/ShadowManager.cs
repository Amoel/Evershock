using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class ShadowManager : BaseManager<ShadowManager>
    {
        protected ShadowManager() { }

        //---------------------------------------------------------------------------

        public void Calculate(Vector2 center, List<Rectangle> rects)
        {
            List<Vector2> directions = new List<Vector2>();

            foreach (Rectangle rect in rects)
            {
                //directions.Add(Vector2.Normalize(new Vector2(rect.X, rect.Y) - center));
                //directions.Add(Vector2.Normalize(new Vector2(rect.X + rect.Width, rect.Y) - center));
                //directions.Add(Vector2.Normalize(new Vector2(rect.X + rect.Width, rect.Y + rect.Height) - center));
                //directions.Add(Vector2.Normalize(new Vector2(rect.X, rect.Y) - center));
                directions.Add(new Vector2(rect.X, rect.Y));
                directions.Add(new Vector2(rect.X + rect.Width, rect.Y));
                directions.Add(new Vector2(rect.X + rect.Width, rect.Y + rect.Height));
                directions.Add(new Vector2(rect.X, rect.Y));
            }

            directions.OrderBy(dir => Math.Atan2(dir.Y, dir.X));
        }
    }
}
