using Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Manager
{
    public class AreaManager : BaseManager<AreaManager>
    {
        private List<Guid> m_Areas;
        private AreaData[,] m_AreaMap;

        //---------------------------------------------------------------------------

        protected AreaManager()
        {
            m_Areas = new List<Guid>();
        }

        //---------------------------------------------------------------------------

        public void Reset(int width, int height)
        {
            m_AreaMap = new AreaData[width, height];
        }

        //---------------------------------------------------------------------------

        public void Register(Area area)
        {
            if (area != null && !m_Areas.Contains(area.GUID))
            {
                m_Areas.Add(area.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void Unregister(Area area)
        {
            if (area != null && m_Areas.Contains(area.GUID))
            {
                m_Areas.Remove(area.GUID);
            }
        }

        //---------------------------------------------------------------------------

        public void AddAreaRect(Guid area, int x, int y, int width, int height)
        {
            for (int xPos = x; xPos < x + width; xPos++)
            {
                for (int yPos = y; yPos < y + height; yPos++)
                {
                    if (m_AreaMap[xPos, yPos] == null)
                    {
                        m_AreaMap[xPos, yPos] = new AreaData(area);
                    }
                    else
                    {
                        m_AreaMap[xPos, yPos].Add(area);
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public Area GetSharedArea(Vector2 left, Vector2 right)
        {
            Point leftPoint = new Point(((int)left.X) / 64, ((int)left.Y) / 64);
            Point rightPoint = new Point(((int)right.X) / 64, ((int)right.Y) / 64);

            if (m_AreaMap[leftPoint.X, leftPoint.Y] != null && m_AreaMap[rightPoint.X, rightPoint.Y] != null)
            {
                foreach (Guid area in m_AreaMap[leftPoint.X, leftPoint.Y].Areas)
                {
                    if (m_AreaMap[rightPoint.X, rightPoint.Y].Areas.Contains(area)) return EntityManager.Get().Find<Area>(area);
                }
            }
            return null;
        }

        //---------------------------------------------------------------------------

        public Area FindAreaFromEntity(Guid entity)
        {
            foreach (Area area in EntityManager.Get().Find<Area>())
            {
                if (area.Entities.Contains(entity)) return area;
            }
            return null;
        }

        //---------------------------------------------------------------------------

        class AreaData
        {
            public List<Guid> Areas { get; private set; }

            //---------------------------------------------------------------------------

            public AreaData(Guid area)
            {
                Areas = new List<Guid> { area };
            }

            //---------------------------------------------------------------------------

            public void Add(Guid area)
            {
                if (!Areas.Contains(area))
                {
                    Areas.Add(area);
                }
            }
        }
    }
}
