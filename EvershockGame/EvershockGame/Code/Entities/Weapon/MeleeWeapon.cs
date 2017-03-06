using EvershockGame.Code.Components;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(string name, Guid parent) : base(name, parent)
        {
            PhysicsComponent physics = AddComponent<PhysicsComponent>();
            physics.Init(BodyType.Dynamic, 0, 0, true);
            physics.IsGravityAffected = false;
            physics.Body.Enabled = false;

            RectColliderComponent collider = AddComponent<RectColliderComponent>();
            collider.Init(35, 35, new Vector2(40, -30));
            collider.SetSensor(true);
        }

        //---------------------------------------------------------------------------

        public override void TryAttack()
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            PhysicsComponent physics = GetComponent<PhysicsComponent>();

            if (transform != null && physics != null)
            {
                physics.ResetLocation(Vector3.Zero);
                physics.Body.Enabled = true;
                IsAttacking = true;
                ScheduleOnAttackEnded(1.0f);
            }
        }

        //---------------------------------------------------------------------------

        private async void ScheduleOnAttackEnded(float time)
        {
            await Task.Delay(TimeSpan.FromSeconds(time));
            OnAttackEnded();
        }

        //---------------------------------------------------------------------------

        protected override void OnAttackEnded()
        {
            PhysicsComponent physics = GetComponent<PhysicsComponent>();
            if (physics != null)
            {
                physics.Body.Enabled = false;
            }
            IsAttacking = false;
        }
    }
}
