using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentAttribute : Attribute
    {
        public Type[] Types { get; private set; }

        //---------------------------------------------------------------------------

        public RequireComponentAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
