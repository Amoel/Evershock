using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    /* Provides additional info for the serialization process of any manager if attached to its type */
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeManagerAttribute : Attribute
    {
        public static readonly bool     USE_BINARY_SERIALIZATION_DEFAULT    = false;
        public static readonly string   FILE_EXTENSION_DEFAULT              = "json";
        public static readonly string   RELATIVE_DIRECTORY_DEFAULT          = "Managers";

        //---------------------------------------------------------------------------

        public bool                     UseBinarySerialization  { get; private set; }
        public string                   FileExtension           { get; private set; }
        public string                   RelativeDirectory       { get; private set; }

        //---------------------------------------------------------------------------

        public SerializeManagerAttribute(bool useBinarySerialization, string fileExtension) :
            this(useBinarySerialization, fileExtension, RELATIVE_DIRECTORY_DEFAULT)
        { }

        //---------------------------------------------------------------------------

        public SerializeManagerAttribute(string relativeDirectory) :
            this(USE_BINARY_SERIALIZATION_DEFAULT, FILE_EXTENSION_DEFAULT, relativeDirectory)
        { }

        //---------------------------------------------------------------------------

        public SerializeManagerAttribute(bool useBinarySerialization, string fileExtension, string relativeDirectory)
        {
            UseBinarySerialization = useBinarySerialization;
            FileExtension = fileExtension;
            RelativeDirectory = relativeDirectory;
        }
    }

    //---------------------------------------------------------------------------

    internal struct SerializeManagerData
    {
        public bool     UseBinarySerialization  { get; private set; }
        public string   FileExtension           { get; private set; }
        public string   RelativeDirectory       { get; private set; }
        public string   RootDirectory           { get; private set; }
        public string   AbsoluteDirectory       { get; private set; }

        //---------------------------------------------------------------------------

        private SerializeManagerData(bool useBinarySerialization, string fileExtension, string relativeDirectory)
        {
            UseBinarySerialization  = useBinarySerialization;
            FileExtension           = fileExtension;
            RelativeDirectory       = relativeDirectory;
            RootDirectory           = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            AbsoluteDirectory       = Path.Combine(RootDirectory, RelativeDirectory);
        }

        //---------------------------------------------------------------------------

        /* Extract relevant serialization data from SerializeManagerAttribute */
        public static SerializeManagerData FromAttribute(SerializeManagerAttribute attribute)
        {
            if (attribute != null)
            {
                return new SerializeManagerData(
                    attribute.UseBinarySerialization,
                    attribute.FileExtension,
                    attribute.RelativeDirectory);
            }
            else
            {
                return new SerializeManagerData(
                    SerializeManagerAttribute.USE_BINARY_SERIALIZATION_DEFAULT,
                    SerializeManagerAttribute.FILE_EXTENSION_DEFAULT,
                    SerializeManagerAttribute.RELATIVE_DIRECTORY_DEFAULT);
            }
        }
    }
}
