using Managers;
using System.Diagnostics;

namespace EvershockGame.Manager
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

        public bool Show(bool condition, string message)
        {
            if (!HideAsserts)
            {
                Debug.Assert(condition, message);
            }
            return !condition;
        }
    }
}
