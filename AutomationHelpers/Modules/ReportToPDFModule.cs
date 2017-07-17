using System;
using System.IO;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    /// Allows conversion of Ranorex report files to PDF
    /// </summary>
    [TestModule("FFA0759D-37D2-4ABB-89A7-411F0FCF2DFE", ModuleType.UserCode, 1)]
    public class ReportToPDFModule : ITestModule
    {
        //Variables

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReportToPDFModule()
        {
            //Init variables
            this.PdfName = "";
            this.Xml = "";
            this.Registered = false;

            //Possible values: none | failed | all
            this.Details = "all";

        }

        [TestVariable("64842eff-ddac-41d8-86db-e8f803b9a2a7")]
        public bool Registered { get; set; }

        [TestVariable("b9993b89-d8cb-45fe-829b-42b0f8dd8a00")]
        public string Details { get; set; }

        [TestVariable("c15f25ea-9409-4e1a-b76e-cd208bae6c56")]
        public string Xml { get; set; }

        [TestVariable("1d09c6b5-db7a-4ed0-a13c-ee2add5d3d37")]
        public string PdfName { get; set; }

        public DirectoryInfo ZippedReportFileDirectoryInfo { get; set; }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            //Delegate must be registered only once
            if (!this.Registered)
            {
                //PDF will be generated at the very end of the testsuite
                TestSuite.TestSuiteCompleted += delegate {

                    //Specify report name if not already set
                    if (String.IsNullOrEmpty(this.PdfName))
                    {
                        this.PdfName = CreatePDFName();
                    }

                    //Necessary to end the testreport in order to update the duration
                    TestReport.EndTestModule();

                    //Comment out if ConvertReportToPDF() is called directly
                    try
                    {
                        Report.LogHtml(ReportLevel.Success, "PDFReport", "Successfully created PDF: <a href='" + ConvertReportToPDF(this.PdfName, this.Xml, this.Details) + "' target='_blank'>Open PDF</a>");
                    }
                    catch (Exception e)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ReportToPDF: " + e.Message);
                        Console.ResetColor();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }

                    //Delete *.rxzlog if not enabled within test suite settings
                    Cleanup();

                    //Update error value
                    UpdateError();
                };

                this.Registered = true;
            }
        }

        private string ConvertReportToPDF(string pdfName, string xml, string details)
        {
            var zippedReportFileDirectory = CreateTempReportFileDirectory();
            var reportFileDirectory = TestReport.ReportEnvironment.ReportFileDirectory;
            var name = TestReport.ReportEnvironment.ReportName;

            var input = String.Format(@"{0}\{1}.rxzlog", zippedReportFileDirectory, name);
            var PDFReportFilePath = String.Format(@"{0}\{1}", reportFileDirectory, AddPdfExtension(pdfName));

            TestReport.SaveReport();
            Report.Zip(TestReport.ReportEnvironment, zippedReportFileDirectory, name);

            Ranorex.PDF.Creator.CreatePDF(input, PDFReportFilePath, xml, details);
            return PDFReportFilePath;
        }

        private static string AddPdfExtension(string pdfName)
        {
            return pdfName.EndsWith(".pdf") ? pdfName : pdfName + ".pdf";
        }

        private string CreateTempReportFileDirectory()
        {
            //Create new temp directory for zipped report
            try
            {
                this.ZippedReportFileDirectoryInfo = System.IO.Directory.CreateDirectory(String.Format(@"{0}\temp", TestReport.ReportEnvironment.ReportFileDirectory));
                return this.ZippedReportFileDirectoryInfo.FullName;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create temp folder: " + ex.Message);
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
                throw new Exception("Failed to recursively delete zipped report file directory: " + ex.Message);
            }
        }

        private static string CreatePDFName()
        {
            //Report Status is not part of the ReportName at this stage of the test
            var name = TestReport.ReportEnvironment.ReportName;

            //Get status from TestSuite
            var testsuite = (TestSuite)TestSuite.Current;

            if (testsuite.ReportSettings.ReportFormatString.Contains("%X"))
            {
                name = name += "_" + GetTestSuiteStatus();
            }

            return name;
        }

        private static void UpdateError()
        {
            var testsuite = (TestSuite)TestSuite.Current;

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
}
