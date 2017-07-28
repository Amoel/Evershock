using EvershockGame.Code;
using EvershockGame.Code.Components;
using EvershockGame.Code.Stages;
using EvershockGame.Components;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelcroPhysics.Collision.Filtering;
using VelcroPhysics.Dynamics;

namespace EvershockGame.Code.Manager
{
    public class LightingManager : BaseManager<LightingManager>
    {
        private List<Vector2> m_AllHits;
        private List<HitInfo> m_Hits;

        public GraphicsDevice Device { get; set; }

        //---------------------------------------------------------------------------

        protected LightingManager()
        {
            m_AllHits = new List<Vector2>();
            m_Hits = new List<HitInfo>();
        }

        //---------------------------------------------------------------------------

        public void DrawArea(SpriteBatch batch, Vector2 center, CameraData data)
        {
            FindCorners(center);

            if (m_Hits.Count < 2) return;

            BasicEffect basicEffect = new BasicEffect(Device);
            basicEffect.VertexColorEnabled = true;
            VertexPositionColor[] vert = new VertexPositionColor[m_Hits.Count + 1];

            Vector2 local = (new Vector2(center.X, center.Y - 32)).ToLocalUV(data);
            vert[0].Position = new Vector3(local.X, -local.Y, 0);
            vert[0].Color = Color.Red;
            for (int i = 1; i < vert.Length; i++)
            {
                local = (m_Hits[i - 1].Hit - new Vector2(0, 32)).ToLocalUV(data);
                vert[i].Position = new Vector3(local.X, -local.Y, 0);
                vert[i].Color = Color.Red;
            }

            short[] ind = new short[m_Hits.Count * 3];

            for (int i = 1; i < vert.Length; i++)
            {
                ind[(i - 1) * 3] = (short)(i % (vert.Length - 1) + 1);
                ind[(i - 1) * 3 + 1] = (short)i;
                ind[(i - 1) * 3 + 2] = 0;
            }
            
            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);
            }
        }

        //---------------------------------------------------------------------------

        public void DrawDebug(SpriteBatch batch, CameraData data)
        {
            batch.Begin();
            for (int i = 0; i < m_AllHits.Count; i++)
            {
                Vector2 centerLocation = m_AllHits[i].ToLocal(data);
                batch.Draw(CollisionManager.Get().PointTexture, new Rectangle((int)(centerLocation.X - 3), (int)(centerLocation.Y - 3), 6, 6), CollisionManager.Get().PointTexture.Bounds, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
            }
            batch.End();
        }

        //---------------------------------------------------------------------------
        
        Vector2 cent;
        HitInfo info;
        private void FindCorners(Vector2 center)
        {
            cent = center / ColliderComponent.Unit;

            m_AllHits.Clear();
            m_Hits.Clear();
            List<Corner> corners = StageManager.Get().GetCorners();

            foreach (Corner corner in corners)
            {
                info = new HitInfo(null, Vector2.Zero, Vector2.Zero, Vector2.Zero, -1);
                PhysicsManager.Get().World.RayCast(RaycastCallback, center / ColliderComponent.Unit, (center + Vector2.Normalize(corner.AbsoluteLocation - center) * 1000) / ColliderComponent.Unit);
                
                if (info.Fixture != null) m_Hits.Add(info);
                m_AllHits.Add(corner.AbsoluteLocation);
            }
            m_Hits = m_Hits.OrderBy(info => Math.Atan2(info.Hit.X - center.X, info.Hit.Y - center.Y)).ToList();
        }

        //---------------------------------------------------------------------------

        private float RaycastCallback(Fixture fix, Vector2 hit, Vector2 normal, float fraction)
        {
            if (!fix.CollisionCategories.HasFlag(Category.Cat2)) return -1;

            if (info.Distance == -1 || fraction <= info.Distance)
            {
                info = new HitInfo(fix, cent * ColliderComponent.Unit, hit * ColliderComponent.Unit, normal, fraction);
            }
            return fraction;
        }

        //---------------------------------------------------------------------------

        class HitInfo
        {
            public Fixture Fixture { get; set; }
            public Vector2 Start { get; set; }
            public Vector2 Hit { get; set; }
            public Vector2 Normal { get; set; }
            public float Distance { get; set; }

            public bool IsNearest { get; set; }

            //---------------------------------------------------------------------------

            public HitInfo(Fixture fixture, Vector2 start, Vector2 hit, Vector2 normal, float distance)
            {
                Fixture = fixture;
                Start = start;
                Hit = hit;
                Normal = normal;
                Distance = distance;
                IsNearest = false;
            }
        }
    }
}
