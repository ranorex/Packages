using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers
{
    /// <summary>
    /// Description of UserCodeModule1.
    /// </summary>
    [UserCodeCollection]
    public class Ranorex_Helper_File
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Ranorex_Helper_File()
        {
            // Do not delete - a parameterless constructor is required!
        }
        
        
        /// <summary>
		/// Creates a log file in the output folder and saves two values in it.
		/// </summary>
		/// <param name="text>Text which is saved in the log file.</param>
		/// <param name="filenamePrefix">Prefix used for the log filename. </param>
		/// <param name="fileExtension">Sets the extension of the file.</param>
		[UserCodeMethod]
		public static void WriteToFile (string text, string filenamePrefix, string fileExtension)                    
		{
			System.DateTime now = System.DateTime.Now;
			string strTimestamp = now.ToString("yyyyMMdd_HHmmss");
			string filename = filenamePrefix + "_" + strTimestamp + "." + fileExtension;
			Report.Info(filename);
		
			try {
				//Create the File
				using (FileStream fs = File.Create(filename))
				{
					Byte[] info = new UTF8Encoding(true).GetBytes(text);			
					fs.Write(info,0,info.Length);
				}
			} 
			catch (Exception ex)
			{			
				Console.WriteLine(ex.ToString());	
			}
			filenamePrefix = filename;
		}
		
		
		/// <summary>
		/// Checks if files in a directory exist and t
		/// </summary>
		/// <param name="path">The relative or absolute where to search for the files.</param>
		/// <param name="pattern">The search string to match against the files in the path.</param>
		/// <param name="expectedCount">Number of expected files to be found.</param>
		/// <param name="timeout">Defines the timeout (seconds) to look for the searched files </param>
		[UserCodeMethod]
		public static void checkFilesExist(string path, string pattern, int expectedCount, int timeout)
		{
			string[] listofFiles = System.IO.Directory.GetFiles(path, pattern);
			System.DateTime start = System.DateTime.Now;
			
			while (listofFiles.Length !=expectedCount && System.DateTime.Now < start.AddSeconds(timeout)) {
				listofFiles = System.IO.Directory.GetFiles(path,pattern);
			}
			
			Ranorex.Report.Info("Check if '" + expectedCount + "' file(s) with pattern '"+ pattern + "' exist in the directory '" +path+ "'. Search time "+timeout+ " seconds.");
			Validate.AreEqual(listofFiles.Length, expectedCount);	
		}
		
		/// <summary>
		/// Deletes files with in a specific path and a certain filename pattern.
		/// </summary>
		/// <param name="path">The relative or absolute where to search for the files.</param>
		/// <param name="pattern">The search string to match against the files in the path.</param>
		[UserCodeMethod]
		public static void DeleteFiles(string path, string pattern)
		{
			string[] listofFiles = System.IO.Directory.GetFiles(path, pattern);

			if (listofFiles.Length == 0)
			{
				Ranorex.Report.Warn("No files have been found in '" + path + "' with the pattern '" + pattern+ "'.");
			}
			
			foreach(string file in listofFiles) {
				try {
					System.IO.File.Delete(file);					
					Ranorex.Report.Info("File has been deleted: " + file.ToString());
				} catch (Exception ex) {
					Ranorex.Report.Error(ex.Message);
				}
			}
		}

		
		/// <summary>
		/// Checks in an interval if files with a specific filename pattern exist.
		/// </summary>
		/// <param name="path">The relative or absolute where to search for the files.</param>
		/// <param name="pattern">The search string to match against the files in the path.</param>
		/// <param name="duration">The duration for how long to wait for the files in milliseconds.</param>
		/// <param name="intervall">Sets the interval in seconds for how often to check the files in milliseconds.</param>
		[UserCodeMethod]
		public static void WaitForFile(string path, string pattern, int duration, int intervall)
		{
			path = getPathForFile(path);
			bool bFound = Directory.GetFiles(path, pattern).Length > 0;
			System.DateTime start = System.DateTime.Now;
			
			while (!bFound && (System.DateTime.Now < start + new Duration(duration)))
			{
				
				bFound=Directory.GetFiles(path, pattern).Length > 0;
				
				if (bFound)
				{
					break;
				}
				
				Delay.Duration(intervall, false);
			}
			
			if (bFound)
			{
				Ranorex.Report.Success("Validation", "File with pattern '" + pattern + "' was found in directory '" + path + "'.");
			}
			else
			{
				Ranorex.Report.Failure("Validation", "File with pattern '" + pattern + "' wasn't found in directory '" + path + "'.");
			}
		}
        

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void WordCompare() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void PdfCompare() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void ReplaceFileContents() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void checkFileContent() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void checkFileContentForPattern() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void readRessourceFile() {
		}


        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void deleteFiles() {
		}
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void checkFileExists() {
		}
        
        private static string getPathForFile(string path)
        {
			if (path.StartsWith(".")) {
				return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path));
			}
			return path;
		}
    }
}
