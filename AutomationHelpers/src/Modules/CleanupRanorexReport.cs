//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using Ranorex.Core.Reporting;

namespace Ranorex.AutomationHelpers.Modules
{
    internal sealed class CleanupRanorexReport
    {
        private readonly System.DateTime testSuiteCompleted;

        internal CleanupRanorexReport(System.DateTime testSuiteCompleted)
        {
            this.testSuiteCompleted = testSuiteCompleted;
        }

        /// <summary>
        /// Used to cleanup and delete all Ranorex report related files from current testrun
        /// </summary>
        internal void Cleanup()
        {
            try
            {
                var reportFileDirectory = TestReport.ReportEnvironment.ReportFileDirectory;
                var name = TestReport.ReportEnvironment.ReportName;

                var reportDataFile = TestReport.ReportEnvironment.ReportDataFilePath;
                var reportFile = string.Format(@"{0}\{1}.{2}", reportFileDirectory, name, GetReportExtension());

                DeleteReportImages();

                File.Delete(reportDataFile);
                File.Delete(reportFile);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete Ranorex report files: " + ex.Message);
            }
        }

        private void DeleteReportImages()
        {
            //Check if images are stored within a subdirectory - default
            if (TestReport.ReportEnvironment.UseScreenshotFolder)
            {
                var imageFolderDirectoryInfo = new DirectoryInfo(TestReport.ReportEnvironment.ReportScreenshotFolderPath);

                if(imageFolderDirectoryInfo.Exists)
                {
                    imageFolderDirectoryInfo.Delete(true);
                }
            }

            //Delete image files, which match the report short name and aren't older then currently created report
            else
            {
                const int MaxPrefixLen = 8;
                var shortName = TestReport.ReportEnvironment.ReportName.Substring(0, Math.Min(TestReport.ReportEnvironment.ReportName.Length, MaxPrefixLen));
                var imageFolderDirectoryInfo = new DirectoryInfo(TestReport.ReportEnvironment.ReportScreenshotFolderPath);

                if(imageFolderDirectoryInfo.Exists)
                {
                    foreach (var image in imageFolderDirectoryInfo.GetFiles(string.Format("*{0}*.jpg", shortName)))
                    {
                        var imageCreationTime = File.GetCreationTime(image.FullName);

                        if (testSuiteCompleted < imageCreationTime.AddSeconds(10)) //Added 10 secs to the imageCreationTime to ensure the images are actually created
                        {
                            image.Delete();
                        }
                    }
                }
            }
        }

        private string GetReportExtension()
        {
            var extension = TestReport.ReportEnvironment.ReportDataFilePath.Split('.');
            return extension[extension.Length - 2];
        }
    }
}
