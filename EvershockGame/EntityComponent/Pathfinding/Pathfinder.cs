using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Pathfinding
{
    public enum EBehaviour
    {
        Follow,
        Avoid
    }

    //---------------------------------------------------------------------------

    public class Pathfinder
    {
        private PathNode[,] m_Nodes;

        //---------------------------------------------------------------------------

        public Pathfinder()
        {
            m_Nodes = new PathNode[StageManager.Get().Width, StageManager.Get().Height];
        }

        //---------------------------------------------------------------------------

        public List<Vector3> ExecuteSearch(Vector3 startLocation, Vector3 endLocation, EBehaviour behaviour = EBehaviour.Follow)
        {
            Point startPoint = new Point(((int)startLocation.X - 16) / 32, ((int)startLocation.Y - 16) / 32);
            Point endPoint = new Point(((int)endLocation.X - 16) / 32, ((int)endLocation.Y - 16) / 32);

            List<Vector3> path = new List<Vector3>();
            foreach (Point position in ExecuteSearch(startPoint, endPoint, behaviour))
            {
                if (position.Equals(startPoint)) continue;
                Vector3 pathLocation = new Vector3(position.X * 32 + 16, position.Y * 32 + 16, startLocation.Z);
                path.Add(pathLocation);
            }
            return path;
        }

        //---------------------------------------------------------------------------

        private List<Point> ExecuteSearch(Point startPoint, Point endPoint, EBehaviour behaviour)
        {
            foreach (PathNode node in m_Nodes)
            {
                node.Reset();
            }

            if (StageManager.Get().IsBlocked(startPoint.X, startPoint.Y)) return new List<Point>();
            if (StageManager.Get().IsBlocked(endPoint.X, endPoint.Y)) return new List<Point>();

            SortedList<int, Point> open = new SortedList<int, Point>(new DuplicateKeyComparer<int>());
            List<Point> closed = new List<Point>();

            open.Add(0, startPoint);

            Point current = new Point();
            while (open.Count > 0)
            {
                current = open.First().Value;
                if (current.Equals(endPoint))
                {
                    break;
                }
                else
                {
                    open.RemoveAt(0);
                    closed.Add(current);

                    foreach (Tuple<Point, int> position in GetAdjacentNodes(current))
                    {
                        if (closed.Contains(position.Item1)) continue;
                        if (open.ContainsValue(position.Item1)) continue;
                        
                        m_Nodes[position.Item1.X, position.Item1.Y].Cost = m_Nodes[current.X, current.Y].Cost + position.Item2;
                        m_Nodes[position.Item1.X, position.Item1.Y].Heuristic = Math.Abs(endPoint.X - position.Item1.X) + Math.Abs(endPoint.Y - position.Item1.Y);
                        open.Add(m_Nodes[position.Item1.X, position.Item1.Y].Total, position.Item1);
                    }
                }
            }

            bool startFound = false;
            List<Point> path = new List<Point>();
            current = endPoint;

            while (!startFound)
            {
                foreach (Tuple<Point, int> position in GetAdjacentNodes(current))
                {
                    if (position.Item1.Equals(startPoint)) startFound = true;

                    if (closed.Contains(position.Item1) || open.ContainsValue(position.Item1))
                    {
                        if (m_Nodes[position.Item1.X, position.Item1.Y].Cost < m_Nodes[current.X, current.Y].Cost)
                        {
                            current = position.Item1;
                            path.Add(position.Item1);
                        }
                    }
                }
            }
            path.Reverse();

            return path;
        }

        //---------------------------------------------------------------------------

        private List<Tuple<Point, int>> GetAdjacentNodes(Point center)
        {
            List<Tuple<Point, int>> nodes = new List<Tuple<Point, int>>();

            if (!StageManager.Get().IsBlocked(center.X - 1, center.Y)) nodes.Add(new Tuple<Point, int>(new Point(center.X - 1, center.Y), 10));
            if (!StageManager.Get().IsBlocked(center.X + 1, center.Y)) nodes.Add(new Tuple<Point, int>(new Point(center.X + 1, center.Y), 10));
            if (!StageManager.Get().IsBlocked(center.X, center.Y - 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X, center.Y - 1), 10));
            if (!StageManager.Get().IsBlocked(center.X, center.Y + 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X, center.Y + 1), 10));

            //if (!StageManager.Get().IsBlocked(center.X - 1, center.Y - 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X - 1, center.Y - 1), 14));
            //if (!StageManager.Get().IsBlocked(center.X - 1, center.Y + 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X - 1, center.Y + 1), 14));
            //if (!StageManager.Get().IsBlocked(center.X + 1, center.Y - 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X + 1, center.Y - 1), 14));
            //if (!StageManager.Get().IsBlocked(center.X + 1, center.Y + 1)) nodes.Add(new Tuple<Point, int>(new Point(center.X + 1, center.Y + 1), 14));

            return nodes;
        }
    }
}
