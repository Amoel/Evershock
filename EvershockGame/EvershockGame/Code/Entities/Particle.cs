using EvershockGame;
using EvershockGame.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class Particle : Entity
    {
        public Particle(string name) : base(name)
        {
            AddComponent<TransformComponent>();
        }
    }
}
