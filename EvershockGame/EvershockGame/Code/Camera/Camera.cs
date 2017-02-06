using EvershockGame.Components;
using EvershockGame.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame
{
    public class Camera : Entity
    {
        public TransformComponent Transform { get { return GetComponent<TransformComponent>(); } }
        public CameraComponent Properties { get { return GetComponent<CameraComponent>(); } }

        //---------------------------------------------------------------------------

        public Camera(string name) : base(name)
        {
            AddComponent<TransformComponent>();
            AddComponent<CameraComponent>();

            CameraManager.Get().RegisterCamera(this);
        }
    }
}
