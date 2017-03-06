using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public interface ITickableComponent
    {
        void PreTick(float deltaTime);
        void Tick(float deltaTime);
        void PostTick(float deltaTime);
    }
}
