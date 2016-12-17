using EntityComponent.Components;
using FarseerPhysics.Dynamics;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class LightingManager : BaseManager<LightingManager>
    {
        private List<Vector2> m_Corners;

        public GraphicsDevice Device { get; set; }

        //---------------------------------------------------------------------------

        protected LightingManager() { }

        //---------------------------------------------------------------------------

        public void Draw(SpriteBatch batch, Vector2 center, CameraData data)
        {
            FindCorners(center);

            if (m_Corners.Count < 2) return;

            BasicEffect basicEffect = new BasicEffect(Device);
            basicEffect.VertexColorEnabled = true;
            VertexPositionColor[] vert = new VertexPositionColor[m_Corners.Count + 1];

            Vector2 local = (center - new Vector2(0, 32)).ToLocalUV(data);
            vert[0].Position = new Vector3(local.X, -local.Y, 0);
            vert[0].Color = Color.Red;
            for (int i = 1; i < vert.Length; i++)
            {
                local = (m_Corners[i - 1] - new Vector2(0, 32)).ToLocalUV(data);
                vert[i].Position = new Vector3(local.X, -local.Y, 0);
                vert[i].Color = Color.Red;
            }

            short[] ind = new short[m_Corners.Count * 3];

            for (int i = 1; i < vert.Length; i++)
            {
                ind[(i - 1) * 3] = (short)(i % (vert.Length - 1) + 1);
                ind[(i - 1) * 3 + 1] = (short)i;
                ind[(i - 1) * 3 + 2] = 0;
            }

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Device.RasterizerState = rasterizerState;

            //batch.Begin();
            //for (int i = 0; i < m_Corners.Count; i++)
            //{
            //    Vector2 location = center.ToLocal(data);
            //    float length = Vector2.Distance(center, m_Corners[i]);
            //    float angle = (float)Math.Atan2(m_Corners[i].Y - center.Y, m_Corners[i].X - center.X);
            //    batch.Draw(CollisionManager.Get().PointTexture, new Rectangle((int)(location.X), (int)(location.Y), (int)length, 2), CollisionManager.Get().PointTexture.Bounds, i % 2 == 0 ? Color.Red : Color.Yellow, angle, Vector2.Zero, SpriteEffects.None, 1.0f);
            //}
            //batch.End();

            foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vert, 0, vert.Length, ind, 0, ind.Length / 3);
            }
        }

        //---------------------------------------------------------------------------

        float closest = 1.0f;
        Vector2 point;
        private void FindCorners(Vector2 center)
        {
            m_Corners = new List<Vector2>();
            List<Vector2> corners = StageManager.Get().GetCorners(center, 1000);

            foreach (Vector2 corner in corners)
            {
                closest = 0.0f;
                PhysicsManager.Get().World.RayCast(RaycastCallback, center / ColliderComponent.Unit, corner / ColliderComponent.Unit);
                m_Corners.Add(point * ColliderComponent.Unit);
            }
            m_Corners = m_Corners.OrderBy(x => Math.Atan2(x.X - center.X, x.Y - center.Y)).ToList();
        }

        //---------------------------------------------------------------------------

        private float RaycastCallback(Fixture fix, Vector2 hit, Vector2 normal, float fraction)
        {
            if (!fix.CollisionCategories.HasFlag(Category.Cat2)) return -1;
            if (closest == 0)
            {
                closest = fraction;
                point = hit;
            }
            else if (fraction < closest)
            {
                closest = fraction;
                point = hit;
            }
            //if (!m_Corners.Contains(hit * ColliderComponent.Unit)) m_Corners.Add(hit * ColliderComponent.Unit);
            return fraction;
        }
    }
}
