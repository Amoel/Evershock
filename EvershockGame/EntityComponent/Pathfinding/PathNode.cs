using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Pathfinding
{
    public struct PathNode
    {
        public int Cost { get; set; }
        public int Heuristic { get; set; }

        public int Total { get { return Cost + Heuristic; } }

        //---------------------------------------------------------------------------

        public void Reset()
        {
            Cost = 0;
            Heuristic = 0;
        }
    }
}
