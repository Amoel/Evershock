using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    public class DuplicateKeyComparer<T> : IComparer<T> where T : IComparable
    {
        public int Compare(T x, T y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else
                return result;
        }
    }
}
