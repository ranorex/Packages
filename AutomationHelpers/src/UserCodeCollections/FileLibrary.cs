//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of useful file methods.
    /// </summary>
    [UserCodeCollection]
    public static class FileLibrary
    {
        private const string libraryName = "FileLibrary";
        private const string newLineRegexPattern = "(\r\n)|(\n)|(\r)";

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
                Utils.ReportException(ex, libraryName);
            }
        }

        /// <summary>
        /// Opens an existing file and adds a new line of text at the end of it.
        /// </summary>
        /// <param name="text">The text to add to the file.</param>
        /// <param name="path">The Full qualified path to load the file including filename and extension.</param>
        /// <param name="addNewLine">If true, adds the text on a newline. Otherwise, adds it without a line break.</param>
        [UserCodeMethod]
        public static void AppendStringToExistingFile(string text, string path, bool addNewLine)
        {
        	Report.Info("Add new text to file: " + path);
            if (addNewLine)
            {
                File.AppendAllText(path, Environment.NewLine + text);
            }
            else
            {
                File.AppendAllText(path, text);
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
                    Utils.ReportException(ex, libraryName);
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

        /// <summary>
        /// Compares content of two binary files.
        /// </summary>
        /// <param name="filePath1">The relative or absolute path of the first file</param>
        /// <param name="filePath2">The relative or absolute path of the second file</param>
        [UserCodeMethod]
        public static void ValidateFilesBinaryEqual(string filePath1, string filePath2)
        {
            try
            {
                filePath1 = GetPathForFile(filePath1);
                filePath2 = GetPathForFile(filePath2);

                if (!FilesExist(filePath1, filePath2))
                {
                    return;
                }

                var fi1 = new FileInfo(filePath1);
                var fi2 = new FileInfo(filePath2);

                if (fi1.Length != fi2.Length)
                {
                    Report.Failure("Files '" + filePath1 + "' and '" + filePath2 + "' are not equal because they differ in size.");
                    return;
                }

                const int bufferSize = 65536;

                using (var fs1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
                using (var fs2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
                {
                    var tempBufferSize = sizeof(Int64);
                    var tempBuffer1 = new byte[tempBufferSize];
                    var tempBuffer2 = new byte[tempBufferSize];

                    for (var i = 0L; i < (fi1.Length / tempBufferSize) + 1; i++)
                    {
                        fs1.Read(tempBuffer1, 0, tempBufferSize);
                        fs2.Read(tempBuffer2, 0, tempBufferSize);

                        if (BitConverter.ToInt64(tempBuffer1, 0) != BitConverter.ToInt64(tempBuffer2, 0))
                        {
                            Report.Failure("Files '" + filePath1 + "' and '" + filePath2 + "' are not equal.");
                            return;
                        }
                    }
                }

                Report.Success("Files '" + filePath1 + "' and '" + filePath2 + "' are equal.");
            }
            catch (Exception ex)
            {
                Utils.ReportException(ex, libraryName);
            }
        }

        /// <summary>
        /// Compares content of two text files.
        /// </summary>
        /// <param name="filePath1">The relative or absolute path of the first file</param>
        /// <param name="filePath2">The relative or absolute path of the second file</param>
        [UserCodeMethod]
        public static void ValidateFilesTextEqual(string filePath1, string filePath2)
        {
            ValidateFilesTextEqual(filePath1, filePath2, true);
        }

        /// <summary>
        /// Compares content of two text files.
        /// </summary>
        /// <param name="filePath1">The relative or absolute path of the first file</param>
        /// <param name="filePath2">The relative or absolute path of the second file</param>
        /// <param name="normalizeLineEndings">If true, line endings will be normalized before comparison.
        /// Original files will not be changed.</param>
        [UserCodeMethod]
        public static void ValidateFilesTextEqual(string filePath1, string filePath2, bool normalizeLineEndings)
        {
            try
            {
                filePath1 = GetPathForFile(filePath1);
                filePath2 = GetPathForFile(filePath2);

                if (!FilesExist(filePath1, filePath2))
                {
                    return;
                }

                var fileContent1 = File.ReadAllText(filePath1);
                if (normalizeLineEndings)
                {
                    fileContent1 = Regex.Replace(fileContent1, newLineRegexPattern, "\r\n");
                }

                var fileContent2 = File.ReadAllText(filePath2);
                if (normalizeLineEndings)
                {
                    fileContent2 = Regex.Replace(fileContent2, newLineRegexPattern, "\r\n");
                }

                if (fileContent1 != fileContent2)
                {
                    Report.Failure("Files '" + filePath1 + "' and '" + filePath2 + "' are not equal.");
                    return;
                }

                Report.Success("Files '" + filePath1 + "' and '" + filePath2 + "' are equal.");
            }
            catch (Exception ex)
            {
                Utils.ReportException(ex, libraryName);
            }
        }

        /// <summary>
        /// Checks if file contains text specified.
        /// </summary>
        /// <param name="filePath">The relative or absolute path to the file</param>
        /// <param name="text">The text to search for</param>
        [UserCodeMethod]
        public static void ValidateFileContainsText(string filePath, string text)
        {
            ValidateFileContainsText(filePath, text, "utf-8");
        }

        /// <summary>
        /// Checks if file contains text specified.
        /// </summary>
        /// <param name="filePath">The relative or absolute path to the file</param>
        /// <param name="text">The text to search for</param>
        /// <param name="fileEncoding">Encoding of a file</param>
        [UserCodeMethod]
        public static void ValidateFileContainsText(string filePath, string text, string fileEncoding)
        {
            try
            {
                var encoding = Encoding.GetEncoding(fileEncoding);

                filePath = GetPathForFile(filePath);

                if (!FilesExist(filePath))
                {
                    return;
                }

                var textFound = false;

                if (Regex.IsMatch(text, newLineRegexPattern))
                {
                    if (File.ReadAllText(filePath, encoding).Contains(text))
                    {
                        Report.Success("Text '" + text + "' was found in file " + filePath + "'.");
                        textFound = true;
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(filePath, encoding))
                    {
                        var line = "";
                        var i = 1;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                Report.Success("Text '" + text + "' was found on line " + i + ": '" + line + "'.");
                                textFound = true;
                            }

                            ++i;
                        }
                    }
                }

                if (!textFound)
                {
                    Report.Failure("Text '" + text + "' was not found in file '" + filePath + "'.");
                }
            }
            catch (Exception ex)
            {
                Utils.ReportException(ex, libraryName);
            }
        }

        private static string GetPathForFile(string path)
        {
            return path.StartsWith(".") ? Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path)) : path;
        }

        private static bool FilesExist(params string[] filePaths)
        {
            bool filesExist = true;

            foreach (string filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    Report.Error("The file '" + filePath + "' does not exist.");
                    filesExist = false;
                }
            }

            return filesExist;
        }
    }
}
