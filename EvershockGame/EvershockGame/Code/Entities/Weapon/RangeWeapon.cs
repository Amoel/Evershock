using EvershockGame.Code.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code
{
    public class RangeWeapon : Weapon
    {
        public RangeWeapon(string name, Guid parent) : base(name, parent)
        {
            BulletSpawnerComponent spawner = AddComponent<BulletSpawnerComponent>();
            spawner.Description = BulletDesc.Default;
        }

        //---------------------------------------------------------------------------

        public override void TryAttack()
        {
            BulletSpawnerComponent spawner = GetComponent<BulletSpawnerComponent>();
            if (spawner != null)
            {
                spawner.Spawn();
            }
        }

        //---------------------------------------------------------------------------

        protected override void OnAttackEnded() { }
    }
}
