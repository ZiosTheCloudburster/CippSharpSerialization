using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CippSharp.Core.Extensions;
using UnityEngine;

namespace CippSharp.Serialization
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
        /// Converts CustomDataPairs array or list to a single string of text
        /// </summary>
        /// <returns></returns>
        public static string ToText(CustomDataPair[] array)
        {
            if (array.IsNullOrEmpty())
            {
                return string.Empty;
            }
            
            string text = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                CustomDataPair pair = array[i];
                text += (i == array.Length - 1) ? pair.ToLine() : pair.ToLine() + Environment.NewLine;
            }
            return text;
        }

        /// <summary>
        /// Converts CustomDataPairs array or list to a single string of text
        /// </summary>
        /// <returns></returns>
        public static string ToText(List<CustomDataPair> list)
        {
            if (list.IsNullOrEmpty())
            {
                return string.Empty;
            }
            
            string text = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                CustomDataPair pair = list[i];
                text += (i == list.Count - 1) ? pair.ToLine() : pair.ToLine() + Environment.NewLine;
            }
            return text;
        }

        /// <summary>
        /// Converts plain text to CustomDataPairs list
        /// </summary>
        /// <param name="writtenDataPairs"></param>
        /// <returns></returns>
        public static IEnumerable<CustomDataPair> FromText(string writtenDataPairs)
        {
            List<CustomDataPair> customDataPairs = new List<CustomDataPair>();
            const string replace = "<S_NL>";
            string[] splitResult = writtenDataPairs.ReplaceNewLine(replace).Split(new[]{replace}, StringSplitOptions.RemoveEmptyEntries);
            if (!splitResult.IsNullOrEmpty())
            {
                foreach (var split in splitResult)
                {
                    if (CustomDataPair.TryParse(split, out CustomDataPair parsedPair))
                    {
                        customDataPairs.Add(parsedPair);
                    }
                }
            }
            return customDataPairs;
        }

        /// <summary>
        /// Converts an IEnumerable of custom data pair to an IEnumerable of KeyValuePair string string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(IEnumerable<CustomDataPair> data)
        {
            return data.Select(Selector);
            
            KeyValuePair<string, string> Selector(CustomDataPair d)
            {
                return (KeyValuePair<string, string>)d;
            }
        }

        /// <summary>
        /// Converts an IEnumerable of KeyValuePair string string to an IEnumerable of custom data pair
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IEnumerable<CustomDataPair> ToCustomDataPairs(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            return dictionary.Select(Selector);
            
            CustomDataPair Selector(KeyValuePair<string, string> keyPair)
            {
                return (CustomDataPair) keyPair;
            }
        }
    }
}

