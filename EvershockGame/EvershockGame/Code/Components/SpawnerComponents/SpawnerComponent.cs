using EntityComponent;
using EntityComponent.Components;
using System;

namespace EvershockGame.Code.Components
{
    [RequireComponent(typeof(TransformComponent))]
    public class SpawnerComponent : Component, IComponent
    {
        protected Random m_Rand;

        public SpawnerComponent (Guid entity) : base (entity) { m_Rand = new Random(); }

        
    }
}
