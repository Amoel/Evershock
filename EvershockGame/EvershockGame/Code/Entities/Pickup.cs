using EntityComponent;
using EvershockGame.Code.Factories;

namespace EvershockGame.Code
{
    public class Pickup : Entity
    {
        public Pickup (string name) : base (name) { }

        
        //---------------------------------------------------------------------------

        public void Init(params EPickups[] types)
        {
            double[] randoms = new double[types.Length];
            double combinedRandoms = 0;

            for (int i = 0; i < types.Length; i++)
            {
                //randoms[i] = m_rand.NextDouble();
                combinedRandoms += randoms[i];
            }
            //This will be needed, to figure out, which Pickups to actually spawn
            //foreach (EAttributes key in Enum.GetValues(typeof(EAttributes))) { }
        }

        public void SpawnPickups(int amount = 1, params EPickups[] types)
        {
            Init(types);

            for (int i = 0; i < amount; i++)
            {

            }
        }
    }
}
