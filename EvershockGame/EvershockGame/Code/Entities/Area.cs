﻿using EvershockGame.Code.Components;
using EvershockGame.Code.Manager;
using EvershockGame.Components;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public delegate void AreaEnterEventHandler(IEntity entity);
    public delegate void AreaLeaveEventHandler(IEntity entity);

    //---------------------------------------------------------------------------

    public class Area : Entity
    {
        public event AreaEnterEventHandler AreaEnter;
        public event AreaLeaveEventHandler AreaLeave;

        //---------------------------------------------------------------------------

        public AreaColliderComponent Collider { get { return GetComponent<AreaColliderComponent>(); } }
        public int Within { get; private set; }

        public List<Guid> Entities { get; private set; }

        //---------------------------------------------------------------------------

        public Area(string name, Guid parent) : base(name, parent)
        {
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AreaColliderComponent collider = AddComponent<AreaColliderComponent>();
            if (collider != null)
            {
                collider.SetCollisionCategory(ECollisionCategory.None);
                collider.SetCollidesWith(ECollisionCategory.Player);
                collider.Enter += OnEnter;
                collider.Leave += OnLeave;
            }

            Entities = new List<Guid>();

            AreaManager.Get().Register(this);
        }

        //---------------------------------------------------------------------------

        private void OnEnter(IEntity source, IEntity target)
        {
            if (target != null)
            {
                Console.WriteLine(string.Format("{0} entered {1}.", target.Name, source.Name));
                Within++;
                Entities.Add(target.GUID);

                AreaEnter?.Invoke(target);
            }
        }

        //---------------------------------------------------------------------------

        private void OnLeave(IEntity source, IEntity target)
        {
            if (target != null)
            {
                Console.WriteLine(string.Format("{0} left {1}.", target.Name, source.Name));
                Within--;
                Entities.Remove(target.GUID);

                AreaLeave?.Invoke(target);
            }
        }
    }
}
