namespace Game {

    using System.Collections;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.IO;
    using System;
    using UnityEngine;

    /// <summary>
    /// Manages object serialization
    /// </summary>
    public static class ObjectSerializer {
        /// <summary>
        /// Serializes an object
        /// </summary>
        public static string SerializeObject<T>(T toSerialize) {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static object XmlDeserializeFromString(string objectData, Type type) {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData)) {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
    }
}
