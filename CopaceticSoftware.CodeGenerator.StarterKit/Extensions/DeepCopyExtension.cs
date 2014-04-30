//----------------------------------------------------------------------- 
// <copyright file="DeepCopyExtension.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 5:48:10 PM</date> 
// Licensed under the Apache License, Version 2.0,
// you may not use this file except in compliance with this License.
//  
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright> 
//-----------------------------------------------------------------------

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
