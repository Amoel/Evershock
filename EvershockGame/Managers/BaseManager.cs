using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    /* Singleton-based BaseClass for all managers. Access instance by calling the Get()-method */
    [Serializable]
    public abstract class BaseManager<T> : IManager where T : class
    {
        private static T s_Instance;

        //---------------------------------------------------------------------------

        public static string Name { get { return typeof(T).Name; } }

        //---------------------------------------------------------------------------

        protected BaseManager()
        {
            Init();
        }

        //---------------------------------------------------------------------------

        /* Creates and returns the singleton for this manager */
        public static T Get() { return s_Instance ?? (s_Instance = Deserialize()); }

        //---------------------------------------------------------------------------

        private static T CreateInstance()
        {
            BindingFlags flags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // Get the default constructor
            ConstructorInfo ctor = typeof(T).GetConstructor(flags, null, Type.EmptyTypes, null);
            if (ctor == null)
            {
                throw new Exception($"Type {Name} does not have a valid constructor.");
            }

            return (T)ctor.Invoke(null);
        }

        //---------------------------------------------------------------------------

        [OnDeserialized]
        private void OnManagerDeserialized(StreamingContext context)
        {
            InitAfterDeserialize();
        }

        //---------------------------------------------------------------------------
        // INITIALIZATION
        //---------------------------------------------------------------------------

        /* Used for initialization of manager specific components before serialization */
        protected virtual void Init() { }

        //---------------------------------------------------------------------------

        /* Used for initialization of manager specific components after serialization */
        protected virtual void InitAfterDeserialize() { }

        //---------------------------------------------------------------------------
        // SERIALIZATION
        //---------------------------------------------------------------------------

        public void Serialize()
        {
            Serialize(s_Instance);
        }
        //---------------------------------------------------------------------------

        /* Writes relevant data of this manager to a file in the rootdirectory */
        private static void Serialize(T instance)
        {
            if (instance != null)
            {
                // Retrieve additional info for serialization from SerializeManagerAttribute (if available)
                SerializeManagerData serializeData = ExtractSerializeManagerData();

                try
                {
                    string file = Path.Combine(serializeData.AbsoluteDirectory, $"{Name}.{serializeData.FileExtension}");

                    // Check if the directory for the file exists. If not, create it.
                    if (!Directory.Exists(serializeData.AbsoluteDirectory))
                    {
                        Directory.CreateDirectory(serializeData.AbsoluteDirectory);
                    }

                    using (FileStream stream = new FileStream(@file, FileMode.Create))
                    {
                        if (serializeData.UseBinarySerialization)
                        {
                            /* Use  more secure binary serialization */
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, instance);
                        }
                        else
                        {
                            /* Use more readable JSON serialization */
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                string serializedData = JsonConvert.SerializeObject(instance, Formatting.Indented);
                                writer.Write(serializedData);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // Catch and handle exceptions during serialization
                    Console.WriteLine($"Could not serialize data of {Name}. {e.Message}");
                }
            }
        }

        //---------------------------------------------------------------------------

        /* Reads relevant data of this manager from a file in the rootdirecty */
        private static T Deserialize()
        {
            T instance = null;

            // Retrieve additional info for serialization from SerializeManagerAttribute (if available)
            SerializeManagerData serializeData = ExtractSerializeManagerData();

            try
            {
                string file = Path.Combine(serializeData.AbsoluteDirectory, $"{Name}.{serializeData.FileExtension}");

                using (FileStream stream = new FileStream(@file, FileMode.Open))
                {
                    if (serializeData.UseBinarySerialization)
                    {
                        /* Use  more secure binary serialization */
                        BinaryFormatter formatter = new BinaryFormatter();
                        instance = formatter.Deserialize(stream) as T;
                    }
                    else
                    {
                        /* Use more readable JSON serialization */
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string serializedData = reader.ReadToEnd();
                            instance = JsonConvert.DeserializeObject<T>(serializedData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Catch and handle exceptions during deserialization
                Console.WriteLine($"Could not deserialize data of {Name}. {e.Message}");
            }

            if (instance == null)
            {
                instance = CreateInstance();
            }

            return instance;
        }

        //---------------------------------------------------------------------------

        private static SerializeManagerData ExtractSerializeManagerData()
        {
            //SerializeManagerAttribute attribute = typeof(T).GetCustomAttribute<SerializeManagerAttribute>();
            //return SerializeManagerData.FromAttribute(attribute);
            return new SerializeManagerData();
        }
    }
}
