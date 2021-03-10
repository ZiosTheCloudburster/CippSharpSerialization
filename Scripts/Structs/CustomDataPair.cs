
using System;
using System.Collections.Generic;
using CippSharp.Core;
using CippSharp.Core.Extensions;
using UnityEngine;

namespace CippSharp.Experimental
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
            string writtenKey = 
                Key.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);

            string writtenValue =
                Value.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);;
            
            return $"\"{writtenKey}\"{DataSeparatorChar}\"{writtenValue}\"";
        }

        /// <summary>
        /// The representation of this data pair as line for a file
        /// </summary>
        /// <returns></returns>
        public string ToLine()
        {
            string writtenKey = 
                Key.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);

            string writtenValue =
                Value.Replace(DataSeparatorChar.ToString(), EqualSymbol)
                    .Replace(QuotationMarkChar.ToString(), QuotationMarkSymbol)
                    .ReplaceEmptyLine(NewLineSymbol)
                    .ReplaceNewLine(NewLineSymbol);;
            
            return $"{writtenKey}{DataSeparatorChar}{writtenValue}";
        }
        
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
            return this;
        }
    }
    
    [Serializable]
    public class CustomDataPairList : ListContainer<CustomDataPair>
    {
               
    }
}