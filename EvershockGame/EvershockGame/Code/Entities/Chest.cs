using EntityComponent;
using EntityComponent.Components;
using EntityComponent.Factory;
using EntityComponent.Manager;
using EvershockGame.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EvershockGame.Code
{
    public class Chest : Entity
    {
        bool closed = true;

        public Chest(string name) : base(name)
        {
            AddComponent<TransformComponent>();
            AddComponent<PhysicsComponent>();
            AddComponent<PickupSpawnerComponent>();

            AddComponent<SpriteComponent>().Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.ChestClosed1), Vector2.Zero, new Vector2(2, 2));
            //AddComponent<ShadowComponent>().Init(AssetManager.Get().Find<Texture2D>("ChestClosed1"), new Vector2(2, 2), new Vector2(0, 3));

            AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>(ELightAssets.CircleLight), Vector2.Zero, new Vector2(2, 2), Color.White, 0.8f);

            RectColliderComponent chestCollider = AddComponent<RectColliderComponent>();
            chestCollider.Init(AssetManager.Get().Find<Texture2D>(ESpriteAssets.ChestClosed1).Width, AssetManager.Get().Find<Texture2D>(ESpriteAssets.ChestClosed1).Height);

            chestCollider.Enter += (source, target) =>
            {
                if (closed)
                {
                    if (source.HasComponent<SpriteComponent>())
                    {
                        source.GetComponent<SpriteComponent>().Texture = AssetManager.Get().Find<Texture2D>(ESpriteAssets.ChestOpened1);
                    }

                    if (source.HasComponent<PickupSpawnerComponent>())
                    {
                        GetComponent<PickupSpawnerComponent>().Spawn();
                    }

                    closed = false;
                }
            };
            //AddComponent<LightingComponent>().Init(AssetManager.Get().Find<Texture2D>("CircleLight"));
        }

        //---------------------------------------------------------------------------

        public static string SpawnChest(int x, int y)
        {
            EntityFactory.Create<Chest>("SpawnChest").Init(new Vector2(x, y));
            return string.Format("Spawned chest at {0}/{1}", x, y);
        }

        //---------------------------------------------------------------------------

        public static string SpawnChest(int cameraIndex)
        {
            List<Camera> cams = EntityManager.Get().Find<Camera>();
            if (cameraIndex >= 0 && cameraIndex < cams.Count)
            {
                TransformComponent transform = cams[cameraIndex].GetComponent<TransformComponent>();
                if (transform != null)
                {
                    EntityFactory.Create<Chest>("SpawnChest").Init(transform.Location.To2D());
                    return string.Format("Spawned chest at in the center of camera {0}", cameraIndex);
                }
            }
            return "Could not spawn chest.";
        }

        //---------------------------------------------------------------------------

        public void Init(Vector2 location)
        {
            TransformComponent transform = GetComponent<TransformComponent>();
            if (transform != null)
            {
                //transform.Init(location.To3D());
                transform.MoveTo(location.To3D());

                PhysicsComponent physics = GetComponent<PhysicsComponent>();
                if (physics != null)
                {
                    physics.ResetLocation();
                }
            }
        }
        
    }
}
