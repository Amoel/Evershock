using EvershockGame.Code.Components;
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
    public class Camera : Entity
    {
        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public CameraComponent Properties { get { return GetComponent<CameraComponent>(); } }

        //---------------------------------------------------------------------------

        public Camera(string name, Guid parent) : base(name, parent)
        {
            AddComponent<TransformComponent>();
            AddComponent<CameraComponent>();

            CameraManager.Get().RegisterCamera(this);
        }
    }
}
