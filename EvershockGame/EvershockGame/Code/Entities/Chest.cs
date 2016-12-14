using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvershockGame.Code
{
    public class Chest : Entity
    {
        bool closed = true;

        public Chest(string name) : base(name)
        {
            AddComponent<TransformComponent>().Init(new Vector3(300, 0, 0));
            AddComponent<AttributesComponent>();
            AddComponent<PhysicsComponent>();
            AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>("ChestClosed1"));
            RectColliderComponent chestCollider = AddComponent<RectColliderComponent>();
            chestCollider.Init(AssetManager.Get().Find<Texture2D>("ChestClosed1").Width, AssetManager.Get().Find<Texture2D>("ChestClosed1").Height);

            chestCollider.Enter += (source, target) =>
            {
                if (closed && target.HasComponent<AttributesComponent>())
                {
                    target.GetComponent<AttributesComponent>().TransmitMovementAddend(150);
                    closed = false;
                }

                if (source.HasComponent<SpriteComponent>())
                {
                    source.GetComponent<SpriteComponent>().Texture = AssetManager.Get().Find<Texture2D>("ChestOpened1");
                }
                    
            };

            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"));
        }

        public void Init(Vector2 location)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                transform.Init(location.To3D());
            }
        }
        
    }
}
