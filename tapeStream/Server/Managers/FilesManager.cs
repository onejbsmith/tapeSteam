﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace tapeStream.Server.Data
{
    /// <summary>
    /// Creates, appends and reads CSV files into new List<class>()
    /// </summary>
    /// <remarks>
    /// Uses CSVHelper for the hard parts
    /// 1. Creating 
    /// </remarks>
    public class FilesManager
    {

        /// <summary>
        /// Create CSV file for T in the Files/{dataType}/{dataDate}/{symbol}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataType"></param>
        /// <param name="symbol"></param>
        public static void CreateCSVFile<T>(string dataType, string symbol)
        { }

        //public static AppendCSVFile
        public static string GetCSVHeader<T>(T anObject)
        {
            try
            {
                // Get the type handle of a specified class.
                Type myType = typeof(T);

                // Get the properties of the specified class.
                PropertyInfo[] myProps = myType.GetProperties();
                var sHeader = string.Join(',', myProps.Select(field => field.Name));
                // Create CSV string from field names
                return sHeader.Replace(",Item", "");
            }

            catch (Exception e)
            {
                //Console.WriteLine("Exception : {0} ", e.Message);
                return "";
            }
        }


        public static string GetFileNameForToday(string fileType)
        {
            try
            {
                return $"FEED {DateTime.Now.ToString("MMM dd yyyy")}.json";
            }
            catch
            {
                return Directory.GetFiles(Directory.GetCurrentDirectory(), $"{fileType}*.json").Where(f => !f.ToLower().Contains("copy")).Last();

            }

        }

        internal static async Task SendToMessageQueue(string svcName, DateTime svcDateTime, string svcFieldedJson)
        {
            string fileName = svcName + svcDateTime.ToString(".yyMMdd.HHmm.ss.ff");
            string filePath = $"D:\\MessageQs\\Inputs\\{svcName}\\{fileName}.json";
            System.IO.File.WriteAllText(filePath, svcFieldedJson);
            //if (svcName == "QUOTE")
            //{
            //    filePath = filePath = $"D:\\MessageQs\\Inputs\\{CONSTANTS.TIMESALE_EQUITY}\\{fileName}.json";
            //    System.IO.File.WriteAllText(filePath, svcFieldedJson);
            //}
            await Task.CompletedTask;
        }

        public static List<string> GetChartEntries(int nCloses)
        {
            var entries = new List<string>();

            string filePath = $"D:\\MessageQs\\Inputs\\CHART_EQUITY";

            var lstFiles = Directory.GetFiles(filePath);
            if (lstFiles.Length < nCloses) nCloses = 0;

            var ourFiles = lstFiles.ToList().Skip(lstFiles.Length - nCloses);
            foreach (var fileName in ourFiles)
            {
                var text = File.ReadAllText(fileName);
                entries.Add(text);
            }
            return entries;

        }
    }


}