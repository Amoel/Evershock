using Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class AssertManager : BaseManager<AssertManager>
    {
        public bool HideAsserts { get; set; }

        //---------------------------------------------------------------------------

        protected AssertManager() { }

        //---------------------------------------------------------------------------

        public void Show(string message)
        {
            if (!HideAsserts)
            {
                Debug.Assert(true, message);
            }
        }

        //---------------------------------------------------------------------------

        public void Show(bool condition, string message)
        {
            if (!HideAsserts)
            {
                Debug.Assert(condition, message);
            }
        }
    }
}
