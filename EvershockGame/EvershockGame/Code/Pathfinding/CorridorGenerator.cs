using EvershockGame.Code;
using EvershockGame.Code.Stages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Pathfinding
{
    public class CorridorGenerator
    {
        private byte[,] m_Map;
        private PathNode[,] m_Nodes;

        public int Size { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        //---------------------------------------------------------------------------

        public CorridorGenerator(byte[,] map, int size)
        {
            Width = map.GetLength(0);
            Height = map.GetLength(1);
            Size = size;

            m_Map = map;
            m_Nodes = new PathNode[Width, Height];
        }

        //---------------------------------------------------------------------------

        public Corridor Execute(Exit startExit, Exit endExit)
        {
            Point startPoint = new Point(startExit.GetLocation().X / Size, startExit.GetLocation().Y / Size);
            Point endPoint = new Point(endExit.GetLocation().X / Size, endExit.GetLocation().Y / Size);

            if (startPoint.Equals(endPoint))
            {
                Corridor c = new Corridor();
                CorridorSegment segment = c.AddSegment(new Point(startPoint.X * Size + Size / 2, startPoint.Y * Size + Size / 2),
                    new Point(endPoint.X * Size + Size / 2, endPoint.Y * Size + Size / 2), Size);

                segment.AddExtension(startExit.Direction.Invert());
                c.AddCorners();

                return c;
            }

            foreach (PathNode node in m_Nodes)
            {
                node.Reset();
            }

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

                    foreach (Point position in FindAdjacentNodes(current.X, current.Y))
                    {
                        if (closed.Contains(position)) continue;
                        if (open.ContainsValue(position)) continue;

                        m_Nodes[position.X, position.Y].Cost = m_Nodes[current.X, current.Y].Cost + 1;
                        m_Nodes[position.X, position.Y].Heuristic = Math.Abs(endPoint.X - position.X) + Math.Abs(endPoint.Y - position.Y);
                        open.Add(m_Nodes[position.X, position.Y].Total, position);
                    }
                }
            }

            bool startFound = false;
            List<Point> path = new List<Point>();
            path.Add(endPoint);
            current = endPoint;
            
            while (!startFound)
            {
                List<Point> adjacentNodes = FindAdjacentNodes(current.X, current.Y);
                if (adjacentNodes.Count == 0) break;

                foreach (Point position in adjacentNodes)
                {
                    if (position.Equals(startPoint)) startFound = true;

                    if (closed.Contains(position) || open.ContainsValue(position))
                    {
                        if (m_Nodes[position.X, position.Y].Cost < m_Nodes[current.X, current.Y].Cost)
                        {
                            current = position;
                            path.Add(position);
                        }
                    }
                }
            }
            path.Reverse();

            Corridor corridor = new Corridor();
            for (int i = 0; i < path.Count - 1; i++)
            {
                CorridorSegment segment = corridor.AddSegment(new Point(path[i].X * Size + Size / 2, path[i].Y * Size + Size / 2), 
                    new Point(path[i + 1].X * Size + Size / 2, path[i + 1].Y * Size + Size / 2), Size);

                if (i == 0) segment.AddExtension(startExit.Direction.Invert());
            }
            corridor.AddCorners();

            return corridor;
        }

        //---------------------------------------------------------------------------

        private List<Point> FindAdjacentNodes(int x, int y)
        {
            List<Point> nodes = new List<Point>();

            int _x = x / 1;
            int _y = y / 1;

            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return nodes;

            if (_x > 0 && m_Map[_x - 1, _y] < 255)
                nodes.Add(new Point(_x - 1, _y));
            if (_x < Width - 1 && m_Map[_x + 1, _y] < 255)
                nodes.Add(new Point(_x + 1, _y));
            if (_y > 0 && m_Map[_x, _y - 1] < 255)
                nodes.Add(new Point(_x, _y - 1));
            if (_y < Height - 1 && m_Map[_x, _y + 1] < 255)
                nodes.Add(new Point(_x, _y + 1));

            return nodes;
        }
    }
}
