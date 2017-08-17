using EvershockGame.Code.Components;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(string name, Guid parent) : base(name, parent)
        {
            PhysicsComponent physics = AddComponent<PhysicsComponent>();
            physics.Init(BodyType.Kinematic, 0, 0, true);
            physics.IsGravityAffected = false;
            physics.Body.Enabled = false;

            SpriteComponent weaponsprite = AddComponent<SpriteComponent>();
            weaponsprite.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.YellowOrb));

        }

        //---------------------------------------------------------------------------

        public override void TryAttack()
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            PhysicsComponent physics = GetComponent<PhysicsComponent>();

            RectColliderComponent collider = AddComponent<RectColliderComponent>();
            collider.Init(50, 50, new Vector2(50, 0), 0, ECollisionCategory.Player);
            collider.SetSensor(true);

            if (physics != null)
            {
                physics.ResetLocation(Vector3.Zero);
                physics.Body.Enabled = true;
                IsAttacking = true;
                physics.Body.LinearVelocity = new Vector2(25,0);
                // i think this doesnt do anything, because the bodies (melee weapon and the player entity are glued together)
            }

            if (collider != null)
            {
                collider.Enable();
            }

            ScheduleOnAttackEnded(1.0f);
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
                physics.Body.LinearVelocity = Vector2.Zero;
            }
            IsAttacking = false;

            RectColliderComponent collider = GetComponent<RectColliderComponent>();
            if (collider != null)
            {
                collider.Disable();
                //remove component?
            }
        }
    }
}
