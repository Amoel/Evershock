using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using Level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    [RequireComponent(typeof(TransformComponent))]
    public class MapComponent : Component, IDrawableComponent
    {
        private Texture2D m_Tileset;

        public Map Map { get; set; }

        //---------------------------------------------------------------------------

        public MapComponent(Guid entity) : base(entity) { }

        //---------------------------------------------------------------------------

        public void Init(Map map)
        {
            Map = map;
            m_Tileset = AssetManager.Get().Find<Texture2D>(map.Tileset);
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data)
        {
            if (Map != null && m_Tileset != null)
            {
                TransformComponent transform = GetComponent<TransformComponent>();
                if (transform != null)
                {
                    Vector2 location = transform.AbsoluteLocation.ToLocal2D(data);
                    for (int x = 0; x < Map.Width; x++)
                    {
                        for (int y = 0; y < Map.Height; y++)
                        {
                            Layer layer = Map[ELayerMode.First, x, y];
                            batch.Draw(m_Tileset, new Rectangle((int)location.X + x * 16, (int)location.Y + y * 16, 16, 16), new Rectangle(layer.TargetX * 16, layer.TargetY * 16, 16, 16), Color.White);
                        }
                    }
                }
            }
        }
    }
}
