using System;
using System.Collections.Generic;
using System.Text;

namespace Stormancer
{
    /// <summary>
    /// Provides methods to serialize and deserialize json documents into objects.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Deserializes a json document into an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);


        /// <summary>
        /// Serializes an object into a json document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize<T>(T value);

    }
}
