using EvershockGame;
using EvershockGame.Components;
using Level;
using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Managers
{
    public class MapManager : BaseManager<MapManager>
    {
        protected MapManager() { }

        //---------------------------------------------------------------------------

        public void CreateCollisionFromMap(IEntity entity, Map map)
        {
            MultiPathColliderComponent path = entity.AddComponent<MultiPathColliderComponent>();
            path.Init();

            if (path != null)
            {
                for (int x = 0; x < map.Width - 1; x++)
                {
                    Vector2 start = new Vector2((x + 1) * 32, 0);
                    int length = 0;
                    for (int y = 0; y < map.Height; y++)
                    {
                        if (map[x, y].IsBlocked != map[x + 1, y].IsBlocked)
                        {
                            length++;
                        }
                        else
                        {
                            if (length > 0)
                            {
                                path.AddPath(start, new Vector2((x + 1) * 32, y * 32));
                                length = 0;
                            }
                            start = new Vector2((x + 1) * 32, (y + 1) * 32);
                        }
                    }
                }

                for (int y = 0; y < map.Height - 1; y++)
                {
                    Vector2 start = new Vector2(0, (y + 1) * 32);
                    int length = 0;
                    for (int x = 0; x < map.Width; x++)
                    {
                        if (map[x, y].IsBlocked != map[x, y + 1].IsBlocked)
                        {
                            length++;
                        }
                        else
                        {
                            if (length > 0)
                            {
                                path.AddPath(start, new Vector2(x * 32, (y + 1) * 32));
                                length = 0;
                            }
                            start = new Vector2((x + 1) * 32, (y + 1) * 32);
                        }
                    }
                }
            }
        }
    }
}
