using CsvHelper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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


        /// <summary>
        /// Needs to read filenames from 3 folders and return one list in time order
        /// </summary>
        /// <param name="TDAStreamerData.simulatorSettings"></param>
        /// <returns></returns>
        internal static Dictionary<DateTime, string> GetFeedFileNames(string symbol,SimulatorSettings simulatorSettings)
        {
            /// Get all file times, names into one dictionary
            var dictAllFileNames = new Dictionary<DateTime, string>();
            var folderPath = $"D:\\MessageQs\\Inputs\\CHART_EQUITY\\{symbol}\\{TDAStreamerData.simulatorSettings.runDate}\\";
            FeedAddFiles(folderPath, dictAllFileNames, TDAStreamerData.simulatorSettings);

            folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\{symbol}\\{TDAStreamerData.simulatorSettings.runDate}\\";
            FeedAddFiles(folderPath, dictAllFileNames, TDAStreamerData.simulatorSettings);

            folderPath = $"D:\\MessageQs\\Inputs\\NASDAQ_BOOK\\{symbol}\\{TDAStreamerData.simulatorSettings.runDate}\\";
            FeedAddFiles(folderPath, dictAllFileNames, TDAStreamerData.simulatorSettings);



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
                if (fileDate.Date == TDAStreamerData.simulatorSettings.runDateDate.Date)
                    if (fileDate.TimeOfDay >= TDAStreamerData.simulatorSettings.startTime.TimeOfDay && fileDate.TimeOfDay <= TDAStreamerData.simulatorSettings.endTime.TimeOfDay)
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

        internal static List<string> GetFeedDates(string symbol)
        {
            var folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\{symbol}\\";
            var folderNames = Directory.GetDirectories(folderPath).ToList();
            var lstFileDates = new List<string>();
            foreach (var folderName in folderNames)
            {
                var filesCount = Directory.GetFiles(folderName).Count().ToString("n0");
                var fileDate = $"{ Path.GetFileName(folderName)} ({filesCount})";
                if (!lstFileDates.Contains(fileDate))
                    lstFileDates.Add(fileDate);
            }
            return lstFileDates;
        }
        internal static List<string> GetFeedTimes(string symbol, string runDate)
        {
            var runDateDate = Convert.ToDateTime(runDate);
            var folderPath = $"D:\\MessageQs\\Inputs\\TIMESALE_EQUITY\\{symbol}\\{runDate}";
            var fileNames = Directory.GetFiles(folderPath).ToList();
            var lstFileTimes = new List<string>();
            foreach (var fileName in fileNames)
            {
                var fileDate = File.GetCreationTime(fileName);
                if (fileDate.Date == runDateDate)
                {
                    var fileTime = fileDate.ToShortTimeString();
                    if (!lstFileTimes.Contains(fileTime))
                        lstFileTimes.Add(fileTime);
                }
            }
            return lstFileTimes;
        }


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

        public static void WriteNewCsvFile<T>(string path, List<T> data)
        {
            using (StreamWriter sw = new StreamWriter(path, false, new System.Text.UTF8Encoding(true)))
            using (CsvWriter cw = new CsvWriter(sw, System.Globalization.CultureInfo.InvariantCulture))
            {
                cw.WriteHeader<T>();
                cw.NextRecord();
                foreach (T item in data)
                {
                    cw.WriteRecord<T>(item);
                    cw.NextRecord();
                }
            }
        }

        public static string GetNewCsvString<T>(List<T> data)
        {
            var ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms))
            using (CsvWriter cw = new CsvWriter(sw, System.Globalization.CultureInfo.InvariantCulture))
            {
                cw.WriteHeader<T>();
                cw.NextRecord();
                foreach (T item in data)
                {
                    cw.WriteRecord<T>(item);
                    cw.NextRecord();
                }
            }
            StreamReader reader = new StreamReader(ms);
            string text = reader.ReadToEnd();
            return text;
        }

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
        internal static async Task SendToMessageQueue(string symbol, string svcName, DateTime svcDateTime, string svcFieldedJson)
        {
            string fileName = svcName + svcDateTime.ToString(".yyMMdd.HHmm.ss.ff");
            string svcDate = svcDateTime.ToString("MMMM dd, yyyy");
            string folderPath = $"D:\\MessageQs\\Inputs\\{svcName}\\{symbol}\\{svcDate}\\";
            string filePath = $"{folderPath}{fileName}.json";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

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
            string svcDate = svcDateTime.ToString("MMMM d, yyyy");
            string folderPath = $"D:\\MessageQs\\Inputs\\{svcName}\\{svcDate}\\";

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

        public static void MoveQFilesToDatedFolders()
        {
            /// Walk thru 3 folders and move files to Date Folders ( create Date folders as needed)
            /// The 3 folders are:
            /// 
            var tsPath = $"D:\\MessageQs\\Inputs\\BookedTimeSales\\";
            var nbPath = $"D:\\MessageQs\\Inputs\\NasdaqBook\\";
            //var cePath = $"D:\\MessageQs\\Inputs\\CHART_EQUITY\\";
            var paths = new List<string>() {  nbPath, tsPath };
            foreach (var path in paths)
            {
                /// Get the filenames in the path
                var files = Directory.GetFiles(path);
                /// Read each file in path
                foreach (var file in files)
                {
                    /// Get it's creation date
                    var date = File.GetCreationTime(file);
                    string svcDate = date.ToString("MMMM dd, yyyy");
                    var dateFolder = path + svcDate;
                    /// Create a folder in the path for that date if needed
                    if (!Directory.Exists(dateFolder)) Directory.CreateDirectory(dateFolder);

                    /// Move the file to the Date folder
                    var newFileName = Path.Combine(dateFolder, Path.GetFileName(file).Replace(svcDate, ""));
                    File.Move(file, newFileName);
                }
            }

        }

        public static List<string> GetChartEntries(string symbol, int nCloses)
        {
            var entries = new List<string>();
            string svcDate = TDAStreamerData.runDate;

            string filePath = $"D:\\MessageQs\\Inputs\\CHART_EQUITY\\{symbol}\\{svcDate}\\";

            /// Get's all the files in this day's CHART_EQUITY folder
            /// which works in real time, giving you the latest candles...
            /// For the simulator we need to return files before the run time
            /// So what is the runtime -- need to add to simulator settings and pass to 
            var lstFiles = Directory.GetFiles(filePath);

            if (TDAStreamerData.simulatorSettings.isSimulated)
            {
                lstFiles = lstFiles.Where(file => File.GetCreationTime(file) <= TDAStreamerData.simulatorSettings.currentSimulatedTime).ToArray();
            }

            if (lstFiles.Length < nCloses) nCloses = 0;

            var ourFiles = lstFiles.ToList().Skip(lstFiles.Length - nCloses);
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.simulatorSettings, "TDAStreamerData.simulatorSettings");
            foreach (var fileName in ourFiles)
            {
                var text = File.ReadAllText(fileName);
                entries.Add(text);
            }
            return entries;

        }

    }


}