﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using tapeStream.Server.Data.classes;

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

        public static async Task WriteToMongoDb(string[] args)
        {
            var connectionString = "mongodb://localhost";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("test");

            string text = System.IO.File.ReadAllText(@"records.JSON");

            var document = BsonSerializer.Deserialize<BsonDocument>(text);
            var collection = database.GetCollection<BsonDocument>("test_collection");
            await collection.InsertOneAsync(document);

        }

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

        /// <summary>
        /// TODO: /// 1. Add Date Folders beneath each
        /// TODO: /// 2. Write seed code to move existing into Date folders
        /// TODO: /// 3. Update Sim code to use new date stuff (s/b mucho faster! to start)
        /// </summary>
        /// <param name="svcName"></param>
        /// <param name="svcDateTime"></param>
        /// <param name="svcFieldedJson"></param>
        /// <returns></returns>
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

        internal static async Task SendToMessageQueueDated(string svcName, DateTime svcDateTime, string svcFieldedJson)
        {
            string fileName = svcName + svcDateTime.ToString(".yyMMdd.HHmm.ss.ff");
            string svcDate = svcDateTime.ToString("dddd MMMM d, yyyy");
            string folderPath = $"D:\\MessageQs\\Dated\\{svcDate}\\Inputs\\{svcName}\\";

            string filePath = $"{folderPath}{fileName}.json";
            /// Add folder for date if not already there
            /// 
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

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

        /// <summary>
        /// Needs to read filenames from 3 folders and return one list in time order
        /// </summary>
        /// <param name="simulatorSettings"></param>
        /// <returns></returns>
        internal static Dictionary<DateTime, string> GetFeedFileNames(SimulatorSettings simulatorSettings)
        {
            /// Get all file times, names into one dictionary
            var dictAllFileNames = new Dictionary<DateTime, string>();
            var folderPath = $"D:\\MessageQs\\Inputs\\CHART_EQUITY\\";
            FeedAddFiles(folderPath, dictAllFileNames, simulatorSettings);

            folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\";
            FeedAddFiles(folderPath, dictAllFileNames, simulatorSettings);

            folderPath = $"D:\\MessageQs\\Inputs\\NASDAQ_BOOK\\";
            FeedAddFiles(folderPath, dictAllFileNames, simulatorSettings);



            /// Get all times into a sorted list
            var lstAllTimes = new List<DateTime>();
            lstAllTimes = dictAllFileNames.Keys.ToList();
            lstAllTimes.Sort();


            /// Create sorted dictionary
            var dictFinalFilesNames = new Dictionary<DateTime, string>();
            foreach (var time in lstAllTimes)
            {
                dictFinalFilesNames.Add(time, dictAllFileNames[time]);
            }

            return dictFinalFilesNames;
        }

        private static void FeedAddFiles(string folderPath, Dictionary<DateTime, string> dictAllFileNames, SimulatorSettings simulatorSettings)
        {
            var fileNames = Directory.GetFiles(folderPath).ToList();
            var lstFileDates = new List<string>();
            foreach (var fileName in fileNames)
            {
                var fileDate = File.GetCreationTime(fileName);
                if (fileDate.Date == simulatorSettings.runDate.Date)
                    if (fileDate.TimeOfDay >= simulatorSettings.startTime.TimeOfDay && fileDate.TimeOfDay <= simulatorSettings.endTime.TimeOfDay)
                    {
                        /// Since GetLastAccessTime is only to the second, add millis to make fileDate unique
                        while (dictAllFileNames.ContainsKey(fileDate))
                            fileDate = fileDate.AddMilliseconds(1);

                        dictAllFileNames.Add(fileDate, fileName);
                    }
            }
        }

        internal static string GetFeedFile(string feedFile)
        {
            var fileText = File.ReadAllText(feedFile);
            /// Replace times with current time
            return fileText;
        }

        internal static List<string> GetFeedDates()
        {
            var folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\";
            var fileNames = Directory.GetFiles(folderPath).ToList();
            var lstFileDates = new List<string>();
            foreach (var fileName in fileNames)
            {
                var fileDate = File.GetLastAccessTime(fileName).ToLongDateString();
                if (!lstFileDates.Contains(fileDate))
                    lstFileDates.Add(fileDate);
            }
            return lstFileDates;
        }
        internal static List<string> GetFeedTimes(DateTime runDate)
        {
            var folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\";
            var fileNames = Directory.GetFiles(folderPath).ToList();
            var lstFileTimes = new List<string>();
            foreach (var fileName in fileNames)
            {
                var fileDate = File.GetLastAccessTime(fileName);
                if (fileDate.Date == runDate)
                {
                    var fileTime = fileDate.ToShortTimeString();
                    if (!lstFileTimes.Contains(fileTime))
                        lstFileTimes.Add(fileTime);
                }
            }
            return lstFileTimes;
        }

    }


}