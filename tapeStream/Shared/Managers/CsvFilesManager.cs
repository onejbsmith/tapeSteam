using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace tapeStream.Shared.Managers
{
    /// <summary>
    /// Creates, appends and reads CSV files into new List<class>()
    /// </summary>
    /// <remarks>
    /// Uses CSVHelper for the hard parts
    /// 1. Creating 
    /// </remarks>
    public class CsvFilesManager
    {


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
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    using (CsvWriter cw = new CsvWriter(sw, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        cw.WriteHeader<T>();
                        cw.NextRecord();
                        foreach (T item in data)
                        {
                            cw.WriteRecord<T>(item);
                            cw.NextRecord();
                        }
                        String result = System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                        //StreamReader reader = new StreamReader(ms);
                        //string text = reader.ReadToEnd();
                        return result;
                    }
                }

            }

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

    }


}