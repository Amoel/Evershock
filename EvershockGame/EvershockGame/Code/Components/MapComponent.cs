using EvershockGame;
using EvershockGame.Components;
using EvershockGame.Manager;
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
            //m_Tileset = AssetManager.Get().Find<Texture2D>(ESpriteAssets.CameraBackground1);
        }

        //---------------------------------------------------------------------------

        public void CreateCollisionFromMap(Map map)
        {
        }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, CameraData data, float deltaTime)
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
                            Cell cell = Map[x, y];
                            foreach (KeyValuePair<ELayerMode, Layer> kvp in cell.Layers)
                            {
                                switch (kvp.Key)
                                {
                                    case ELayerMode.First:
                                        batch.Draw(m_Tileset, new Rectangle((int)location.X + x * 32, (int)location.Y + y * 32, 32, 32), new Rectangle(kvp.Value.TargetX * 32, kvp.Value.TargetY * 32, 32, 32), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.00001f);
                                        break;
                                    case ELayerMode.Second:
                                        batch.Draw(m_Tileset, new Rectangle((int)location.X + x * 32, (int)location.Y + y * 32, 32, 32), new Rectangle(kvp.Value.TargetX * 32, kvp.Value.TargetY * 32, 32, 32), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.00002f);
                                        break;
                                    case ELayerMode.Third:
                                        batch.Draw(m_Tileset, new Rectangle((int)location.X + x * 32, (int)location.Y + y * 32, 32, 32), new Rectangle(kvp.Value.TargetX * 32, kvp.Value.TargetY * 32, 32, 32), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }
}
