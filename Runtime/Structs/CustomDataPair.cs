
using System;
using System.Collections.Generic;
using CippSharp.Core.Containers;
using CippSharp.Core.Extensions;
using UnityEngine;


namespace CippSharp.Serialization
{
    [Serializable]
    public struct CustomDataPair : ISimplePair<string, string>
    {
        /// <summary>
        /// Data separator between key and value
        /// </summary>
        public const char DataSeparatorChar = '=';
        public const char QuotationMarkChar = '"';
        public static readonly string NewLineValueChars = Environment.NewLine;
        public const string EqualSymbol = "<EqualSymbol>";
        public const string QuotationMarkSymbol = "<QuotationMark>";
        public const string NewLineSymbol = "<NewLine>";
        
        [SerializeField] private string key;

        public string Key
        {
            get => this.key;
            set => this.key = value;
        }

        [SerializeField] private string value;

        public string Value
        {
            get => this.value;
            set => this.value = value;
        }

        #region Constructor
    
        public CustomDataPair(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public CustomDataPair(KeyValuePair<string, string> pair)
        {
            this.key = pair.Key;
            this.value = pair.Value;
        }
        
        #endregion
        
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// The representation of this data pair as line for a file
        /// </summary>
        /// <returns></returns>
        public string ToLineWithQuotations()
        {
            return WriteLine(key, value, true);
        }

        /// <summary>
        /// The representation of this data pair as line for a file
        /// </summary>
        /// <returns></returns>
        public string ToLine()
        {
            return WriteLine(key, value, false);
        }

        #region Write
    
        /// <summary>
        /// The representation of this data pair as line for a file
        /// </summary>
        /// <returns></returns>
        public static string WriteLine(CustomDataPair pair)
        {
            return WriteLine(pair.Key, pair.Value);
        }

        /// <summary>
        /// The representation of this data pair as line for a file
        /// </summary>
        /// <returns></returns>
        private static string WriteLine(string key, string value, bool withQuotationMarks = false)
        {
            string writtenKey = 
                key.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);

            string writtenValue =
                value.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);;

            return withQuotationMarks ? 
                $"\"{writtenKey}\"{DataSeparatorChar}\"{writtenValue}\"" : 
                $"{writtenKey}{DataSeparatorChar}{writtenValue}";
        }
        
        #endregion
        
        /// <summary>
        /// Parse a line and converts it to CustomDataPair
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static CustomDataPair Parse(string line)
        {
            string[] split = line.Split(DataSeparatorChar);
            //first and last characters are \" from ToLine method.
            string key = 
                split[0].TrimStart(new []{'\"'}).TrimEnd(new []{'\"'})
                    .Replace(EqualSymbol, DataSeparatorChar.ToString())
                    .Replace(QuotationMarkSymbol, QuotationMarkChar.ToString())
                    .Replace(NewLineSymbol, NewLineValueChars);
            string value = 
                split[1].TrimStart(new []{'\"'}).TrimEnd(new []{'\"'})
                    .Replace(EqualSymbol, DataSeparatorChar.ToString())
                    .Replace(QuotationMarkSymbol, QuotationMarkChar.ToString())
                    .Replace(NewLineSymbol, NewLineValueChars);
            return new CustomDataPair(key, value);
        }

        /// <summary>
        /// Try to parse a line and to converts it to CustomDataPair
        /// </summary>
        /// <param name="line"></param>
        /// <param name="dataPair"></param>
        /// <returns></returns>
        public static bool TryParse(string line, out CustomDataPair dataPair)
        {
            try
            {
                dataPair = Parse(line);
                return true;
            }
            catch
            {
                //ignored
                dataPair = new CustomDataPair();
                return false;
            }
        }

        public void EditValue(string newValue)
        {
            this.value = newValue;
        }

        /// <summary>
        /// Operator to convert to KeyValuePair
        /// </summary>
        /// <param name="dataPair"></param>
        /// <returns></returns>
        public static implicit operator KeyValuePair<string, string>(CustomDataPair dataPair)
        {
            return new KeyValuePair<string, string>(dataPair.Key, dataPair.Value);
        }

        /// <summary>
        /// Operator to convert to CustomDataPair
        /// </summary>
        /// <param name="valuePair"></param>
        /// <returns></returns>
        public static implicit operator CustomDataPair(KeyValuePair<string, string> valuePair)
        {
            return new CustomDataPair(valuePair.Key, valuePair.Value);
        }
        
        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new KeyValuePair<string, string>(Key, Value);
        }
    }
}