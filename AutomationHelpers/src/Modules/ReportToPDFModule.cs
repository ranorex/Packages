//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    /// Allows conversion of the default Ranorex report to PDF.
    /// </summary>
    [TestModule("FFA0759D-37D2-4ABB-89A7-411F0FCF2DFE", ModuleType.UserCode, 1)]
    public sealed class ReportToPDFModule : ITestModule
    {
        //Variables

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReportToPDFModule()
        {
            //Init variables
            this.registered = false;
            this.PdfName = "";
            this.Xml = "";

            //Possible values: none | failed | all
            this.Details = "all";

        }

        private bool registered { get; set; }

        private DirectoryInfo ZippedReportFileDirectoryInfo { get; set; }

        [TestVariable("1d09c6b5-db7a-4ed0-a13c-ee2add5d3d37")]
        public string PdfName { get; set; }

        [TestVariable("536f8a69-f246-4f2b-ae9e-1f62babb2ba9")]
        public string PdfDirectoryPath { get; set; }

        [TestVariable("c15f25ea-9409-4e1a-b76e-cd208bae6c56")]
        public string Xml { get; set; }

        [TestVariable("b9993b89-d8cb-45fe-829b-42b0f8dd8a00")]
        public string Details { get; set; }

#if !RX72 && !RX80 //this requires Ranorex 8.1+
        [TestVariable("7f788c18-962c-41ab-b591-9c3122512c5e")]
        public bool DeleteRanorexReport { get; set; }
#endif

        /// <summary>
        /// Converts the Ranorex Report into PDF after the test run completed. Use this module in
        /// the TearDown of your TestCase to ensure that it is executed even on failing test runs.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            //Delegate must be registered only once
            if (!this.registered)
            {
                System.DateTime testSuiteCompleted = new System.DateTime();

                //PDF will be generated at the very end of the TestSuite
                TestSuite.TestSuiteCompleted += delegate
                {
                    testSuiteCompleted = System.DateTime.Now;
                    CreatePDF();
                };

#if !RX72 && !RX80 //this requires Ranorex 8.1+
                if (this.DeleteRanorexReport)
                {
                    TestSuiteRunner.TestRunCompleted += delegate
                    {
                        var cleaner = new CleanupRanorexReport(testSuiteCompleted);
                        cleaner.Cleanup();
                    };
                }
#endif

                this.registered = true;
            }
        }

        public string CreatePDF()
        {
            //Specify Ranorex Report name if not already set
            if (String.IsNullOrEmpty(this.PdfName))
            {
                this.PdfName = CreatePDFName();
            }

            var pdfReportFilePath = "";

            try
            {
                pdfReportFilePath = ConvertReportToPDF(this.PdfName, this.PdfDirectoryPath, this.Xml, this.Details);
                Report.LogHtml(
                    ReportLevel.Success,
                    "PDFReport",
                    string.Format(
                        "Successfully created PDF: <a href='{0}' target='_blank'>Open PDF</a>",
                        pdfReportFilePath));
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ReportToPDF: " + e.Message);
                Console.ResetColor();
            }

            //Delete *.rxzlog if not enabled within test suite settings
            Cleanup();

            //Update error value
            UpdateError();

            return pdfReportFilePath;
        }

        private string ConvertReportToPDF(string pdfName, string pdfDirectoryPath, string xml, string details)
        {
        	var zippedReportFileDirectory = CreateTempReportFileDirectory();
            var reportFileDirectory = string.IsNullOrEmpty(pdfDirectoryPath)
                ? TestReport.ReportEnvironment.ReportFileDirectory
                : pdfDirectoryPath;
            var name = TestReport.ReportEnvironment.ReportName;

            var input = String.Format(@"{0}\{1}.rxzlog", zippedReportFileDirectory, name);
            var PDFReportFilePath = String.Format(@"{0}\{1}", reportFileDirectory, AddPdfExtension(pdfName));

            FinishReport();

            Report.Zip(TestReport.ReportEnvironment, zippedReportFileDirectory, name);

            Ranorex.PDF.Creator.CreatePDF(input, PDFReportFilePath, xml, details);

            return PDFReportFilePath;
        }

        private static string AddPdfExtension(string pdfName)
        {
            return pdfName.EndsWith(".pdf") ? pdfName : pdfName + ".pdf";
        }

        private void FinishReport() {
            Activity activity = ActivityStack.Current ;

            //Necessary to end the Ranorex Report in order to update the duration and finalize the status
            if (activity.GetType().Name.Equals("TestSuiteActivity"))
            {
                TestReport.EndTestModule();
                Report.End();
            }

            TestReport.SaveReport();
        }

        private string CreateTempReportFileDirectory()
        {
            //Create new temp directory for zipped Report
            try
            {
                this.ZippedReportFileDirectoryInfo = Directory.CreateDirectory(
                    string.Format(@"{0}\temp", TestReport.ReportEnvironment.ReportFileDirectory));
                return this.ZippedReportFileDirectoryInfo.FullName;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create temp folder: " + ex.Message);
            }
        }

        private void Cleanup()
        {
            try
            {
                this.ZippedReportFileDirectoryInfo.Delete(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to recursively delete zipped report file directory: " + ex.Message);
            }
        }

        private static string CreatePDFName()
        {
            //Report status is not part of the Report name at this stage of the test
            var name = TestReport.ReportEnvironment.ReportName;

            //Get status from TestSuite
            var testsuite = (TestSuite)TestSuite.Current;

            if (testsuite.ReportSettings.ReportFormatString.Contains("%X"))
            {
                name += "_" + GetTestSuiteStatus();
            }

            return name;
        }

        private static void UpdateError()
        {
            if (GetTestSuiteStatus().Contains("Failed"))
            {
                Report.Failure("Rethrow Exception within PDF Module (Necessary for correct error value)");
            }
        }

        private static string GetTestSuiteStatus()
        {
            string status = "";

            var rootChildren = ActivityStack.Instance.RootActivity.Children;

            if (rootChildren.Count > 1)
            {
                Console.WriteLine("Multiple TestSuiteActivites, status taken from first entry");
            }

            var testSuiteAct = rootChildren[0] as TestSuiteActivity;

            if (testSuiteAct != null)
            {
                status = testSuiteAct.Status.ToString();
            }

            return status;
        }
    }

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

                if (imageFolderDirectoryInfo.Exists)
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

                if (imageFolderDirectoryInfo.Exists)
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
