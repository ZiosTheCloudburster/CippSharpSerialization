using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CippSharp.Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CippSharp.Serialization
{
    public static class SerializationUtils
    {
        #region Serialize
        
        /// <summary>
        /// Tries to take bytes from any object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bytes"></param>
        public static bool Serialize(object target, out List<byte> bytes)
        {
            return SerializeInternal(target, out bytes, null, false);
        }

        /// <summary>
        /// Tries to take bytes from any object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bytes"></param>
        /// <param name="debugContext"></param>
        public static bool Serialize(object target, out List<byte> bytes, Object debugContext)
        {
            return SerializeInternal(target, out bytes, debugContext, true);
        }

        /// <summary>
        /// Tries to take bytes from any object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bytes"></param>
        /// <param name="debugContext"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool SerializeInternal(object target, out List<byte> bytes, Object debugContext, bool debug)
        {
            bytes = new List<byte>();
            if (target == null) 
            {
                return false;
            }

            string logName = (debug) ? StringUtils.LogName(debugContext) : string.Empty;
            
            try
            {
                bytes.Clear();
                var formatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    formatter.Serialize(memoryStream, target);
                    bytes.AddRange(memoryStream.ToArray());
                }

                return true;
            }
            catch (Exception e)
            {
                if (debug)
                {
                    Debug.LogError(logName + $"Failed to serialize object, error: {e.Message}.", debugContext);
                }
                return false;
            }
        }
        
        #endregion

        #region Deserialize

        /// <summary>
        /// Tries to convert stored bytes in T.
        /// </summary>
        /// <typeparam name="T">T full type should match the stored full type.
        /// and/or it should be assignable from T</typeparam>
        public static bool Deserialize<T>(byte[] bytes, out T result)
        {
            return DeserializeInternal<T>(bytes, out result, null, false);
        }
        
        /// <summary>
        /// Tries to convert stored bytes in T.
        /// </summary>
        /// <typeparam name="T">T full type should match the stored full type.
        /// and/or it should be assignable from T</typeparam>
        public static bool Deserialize<T>(byte[] bytes, out T result, Object debugContext)
        {
            return DeserializeInternal<T>(bytes, out result, debugContext, true);
        }
        
        /// <summary>
        /// Tries to convert stored bytes in T.
        /// </summary>
        /// <typeparam name="T">T full type should match the stored full type.
        /// and/or it should be assignable from T</typeparam>
        private static bool DeserializeInternal<T>(byte[] bytes, out T result, Object debugContext, bool debug)
        {
            result = default(T);
            string logName = (debug) ? StringUtils.LogName(debugContext) : string.Empty;
          
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var bytesArray = bytes;
                    var formatter = new BinaryFormatter();
                    memoryStream.Write(bytesArray, 0, bytesArray.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    result = (T)formatter.Deserialize(memoryStream);
                }

                return true;
            }
            catch (Exception e)
            {
                if (debug)
                {
                    Debug.LogError(logName+$"Failed to deserialize object, error: {e.Message}.", debugContext);
                }
                
                return false;
            }
        }

        #endregion
    }
}
