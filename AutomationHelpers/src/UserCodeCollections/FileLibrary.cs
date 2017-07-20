//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using System.IO;
using System.Text;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of useful file methods.
    /// </summary>
    [UserCodeCollection]
    public static class FileLibrary
    {
        /// <summary>
        /// Creates a log file containing a custom text in the output folder.
        /// </summary>
        /// <param name="text">Text that the log file should contain</param>
        /// <param name="filenamePrefix">Prefix used for the log filename</param>
        /// <param name="fileExtension">Extension of the log file</param>
        [UserCodeMethod]
        public static void WriteToFile(string text, string filenamePrefix, string fileExtension)
        {
            var now = System.DateTime.Now;
            var strTimestamp = now.ToString("yyyyMMdd_HHmmss");
            var filename = filenamePrefix + "_" + strTimestamp + "." + fileExtension;
            Report.Info(filename);

            try
            {
                //Create the File
                using (FileStream fs = File.Create(filename))
                {
                    var info = new UTF8Encoding(true).GetBytes(text);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Checks if files in a directory exist.
        /// </summary>
        /// <param name="path">The relative or absolute path to search for the files</param>
        /// <param name="pattern">The pattern to search for in the filename</param>
        /// <param name="expectedCount">Number of expected files to be found</param>
        /// <param name="timeout">Defines the search timeout in seconds</param>
        [UserCodeMethod]
        public static void CheckFilesExist(string path, string pattern, int expectedCount, int timeout)
        {
            var listofFiles = Directory.GetFiles(path, pattern);
            var start = System.DateTime.Now;

            while (listofFiles.Length != expectedCount && System.DateTime.Now < start.AddSeconds(timeout))
            {
                listofFiles = Directory.GetFiles(path, pattern);
            }

            Report.Info("Check if '" + expectedCount + "' file(s) with pattern '" + pattern + "' exist in the directory '" + path + "'. Search time " + timeout + " seconds.");
            Validate.AreEqual(listofFiles.Length, expectedCount);
        }

        /// <summary>
        /// Deletes files.
        /// </summary>
        /// <param name="path">The relative or absolute path to search for the files</param>
        /// <param name="pattern">The pattern to search for in the filename</param>
        [UserCodeMethod]
        public static void DeleteFiles(string path, string pattern)
        {
            var listofFiles = Directory.GetFiles(path, pattern);

            if (listofFiles.Length == 0)
            {
                Report.Warn("No files have been found in '" + path + "' with the pattern '" + pattern + "'.");
            }

            foreach (string file in listofFiles)
            {
                try
                {
                    File.Delete(file);
                    Report.Info("File has been deleted: " + file.ToString());
                }
                catch (Exception ex)
                {
                    Report.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// Repeatedly checks if files in a directory exist.
        /// </summary>
        /// <param name="path">The relative or absolute path to search for the files</param>
        /// <param name="pattern">The pattern to search for in the filename</param>
        /// <param name="duration">Defines the search timeout in milliseconds</param>
        /// <param name="interval">Sets the interval in milliseconds at which the files are checked for the pattern</param>
        [UserCodeMethod]
        public static void WaitForFile(string path, string pattern, int duration, int interval)
        {
            path = GetPathForFile(path);
            var bFound = Directory.GetFiles(path, pattern).Length > 0;
            var start = System.DateTime.Now;

            while (!bFound && (System.DateTime.Now < start + TimeSpan.FromMilliseconds(duration)))
            {

                bFound = Directory.GetFiles(path, pattern).Length > 0;

                if (bFound)
                {
                    break;
                }

                Delay.Duration(Duration.FromMilliseconds(interval), false);
            }

            if (bFound)
            {
                Report.Success("Validation", "File with pattern '" + pattern + "' was found in directory '" + path + "'.");
            }
            else
            {
                Report.Failure("Validation", "File with pattern '" + pattern + "' wasn't found in directory '" + path + "'.");
            }
        }

        private static string GetPathForFile(string path)
        {
            return path.StartsWith(".") ? Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path)) : path;
        }
    }
}
