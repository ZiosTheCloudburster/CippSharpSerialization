using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CippSharp.Experimental
{
    public static class CustomDataPairUtils
    {
        /// <summary>
        /// Tries to write a dictionary by converting it in custom data pairs
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static bool TryWrite(string fullPath, Dictionary<string, string> dataPairs)
        {
            return TryWrite(fullPath, dataPairs.Select(k => new CustomDataPair(k)).ToArray());
        }

        /// <summary>
        /// Tries to write custom data pairs at file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static bool TryWrite(string fullPath, CustomDataPair[] dataPairs)
        {
            try
            {
                string[] lines = new string[dataPairs.Length];
                for (int i = 0; i < dataPairs.Length; i++)
                {
                    lines[i] = (dataPairs[i].ToLine());
                }
                File.WriteAllLines(fullPath, lines.ToArray());
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write data. Error {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Tries to read a dictionary by converting custom data pairs
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static bool TryRead(string fullPath, out Dictionary<string, string> dataPairs)
        {
            CustomDataPair[] customDataPairs;
            if (TryRead(fullPath, out customDataPairs))
            {
                dataPairs = new Dictionary<string, string>();
                for (var i = 0; i < customDataPairs.Length; i++)
                {
                    CustomDataPair customDataPair = customDataPairs[i];
                    dataPairs[customDataPair.Key] = customDataPair.Value;
                }
                return true;
            }
            else
            {
                dataPairs = null;
                return false;
            }
        }
        
        /// <summary>
        /// Tries to read an array of custom data pairs from a file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static bool TryRead(string fullPath, out CustomDataPair[] dataPairs)
        {
            try
            {
                List<CustomDataPair> customDataPairs = new List<CustomDataPair>();
                string[] lines = File.ReadAllLines(fullPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    CustomDataPair parsed;
                    if (CustomDataPair.TryParse(lines[i], out parsed))
                    {
                        customDataPairs.Add(parsed);
                    }
                }
                dataPairs = customDataPairs.ToArray();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read data. Error {e.Message}");
                dataPairs = null;
                return false;
            }   
        }


        /// <summary>
        /// Tries to read an array of custom data pairs from a file
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static bool TryRead(string[] lines, out CustomDataPair[] dataPairs)
        {
            try
            {
                List<CustomDataPair> customDataPairs = new List<CustomDataPair>();
                for (int i = 0; i < lines.Length; i++)
                {
                    CustomDataPair parsed;
                    if (CustomDataPair.TryParse(lines[i], out parsed))
                    {
                        customDataPairs.Add(parsed);
                    }
                }
                dataPairs = customDataPairs.ToArray();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read data. Error {e.Message}");
                dataPairs = null;
                return false;
            }   
        }

        /// <summary>
        /// Converts an array of CustomDataPairs to Dictionary string, string
        /// </summary>
        /// <param name="customDataPairs"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(CustomDataPair[] customDataPairs)
        {
            if (customDataPairs == null)
            {
                return null;
            }

            Dictionary<string, string> dataPairs = new Dictionary<string, string>();
            for (var i = 0; i < customDataPairs.Length; i++)
            {
                CustomDataPair customDataPair = customDataPairs[i];
                dataPairs[customDataPair.Key] = customDataPair.Value;
            }
            return dataPairs;
        }

        /// <summary>
        /// Converts a Dictionary string, string to CustomDataPairs
        /// </summary>
        /// <param name="dataPairs"></param>
        /// <returns></returns>
        public static CustomDataPair[] ToCustomDataPairs(Dictionary<string, string> dataPairs)
        {
            if (dataPairs == null)
            {
                return null;
            }
            List<CustomDataPair> customDataPairs = new List<CustomDataPair>();
            KeyValuePair<string, string>[] dataPairsArray = dataPairs.ToArray();
            for (int i = 0; i < dataPairsArray.Length; i++)
            {
                KeyValuePair<string, string> valuePair = dataPairsArray[i];
                customDataPairs.Add((CustomDataPair)valuePair);
            }

            return customDataPairs.ToArray();
        }

        /// <summary>
        /// Converts CustomDataPairs array to a single string of text
        /// </summary>
        /// <returns></returns>
        public static string ToText(CustomDataPair[] customDataPairs)
        {
            if (customDataPairs == null)
            {
                return string.Empty;
            }

            string text = string.Empty;
            for (int i = 0; i < customDataPairs.Length; i++)
            {
                CustomDataPair customDataPair = customDataPairs[i];
                text += (i == customDataPairs.Length - 1)
                    ? $"{customDataPair.ToLine()}"
                    : $"{customDataPair.ToLine()}{Environment.NewLine}";
            }
            return text;
        }
    }
}

