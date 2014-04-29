using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class DeepCopyExtension
    {
        public class DeserializationBinderHelper<T> : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return typeof(T).Assembly.GetType(typeName);
            }
        }

        public static T DeepCopyLocal<T>(this T objectToCopy) where T : class
        {
            if (null == objectToCopy)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter {Binder = new DeserializationBinderHelper<T>()};

                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memoryStream) as T;
            }
        }
    }
}
