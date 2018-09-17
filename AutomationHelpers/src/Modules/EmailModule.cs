//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using Ranorex.AutomationHelpers.UserCodeCollections;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    /// Used to send emails from a testsuite.
    /// </summary>
    [TestModule("D8198CC7-82F5-46B2-81E7-3ED789544877", ModuleType.UserCode)]
    public sealed class EmailModule : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public EmailModule()
        {
            this.From = "";
            this.Body = "";
            this.Password = "";

            this.ServerHostname = "";
            this.ServerPort = "";
            this.Username = "";

            this.SendEmailOnFailure = true;
            this.SendEmailOnSuccess = true;
        }

        /// <summary>
        /// Gets or sets the value of the email subject.
        /// </summary>
        [TestVariable("279773ef-ef45-414a-a555-48cb80bfc115")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the value of the email body.
        /// </summary>
        [TestVariable("ef2dc4ee-14a8-483f-92ad-f2c6bd6d67db")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the value of the email recipients. Separate recipients by comma or semicolon.
        /// </summary>
        [TestVariable("12fd536f-980f-4e5a-a921-acaf0c6247e5")]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the value of the email sender.
        /// </summary>
        [TestVariable("718a0466-d3a5-4d93-93a0-91ccd5f1a19e")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the value of the email server hostname.
        /// </summary>
        [TestVariable("2436c39d-0705-4ec3-85fb-5e9835a1ab18")]
        public string ServerHostname { get; set; }

        /// <summary>
        /// Gets or sets the value of the email server port.
        /// </summary>
        [TestVariable("e255daf0-00ef-4b39-a594-273d574839a1")]
        public string ServerPort { get; set; }

        /// <summary>
        /// Gets or sets whether SSL is used or not (true or false) to connect to the email server.
        /// </summary>
        [TestVariable("c898ee67-ee7f-4258-ab69-d855b0d92274")]
        public bool UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the value of the user name to use when connecting to the email server.
        /// </summary>
        [TestVariable("38abb172-e49e-4d92-a24f-145709bdd7e3")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the value of the user password when connecting to the email server.
        /// </summary>
        [TestVariable("b97a0c41-28c8-44ed-b089-a8ca4b6af9d7")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the value to send the mail when the test suite completes with a failure (<c>true</c> or <c>false</c>).
        /// </summary>
        /// <remarks>If <c>true</c> Sends an email when the test suite finishes with a failure.</remarks>
        [TestVariable("07580bd9-744c-4666-83a6-ba22c0c8d909")]
        public bool SendEmailOnFailure { get; set; }

        /// <summary>
        /// Gets or sets the value to send the mail when the test suite completes successfully (<c>true</c> or <c>false</c>).
        /// </summary>
        /// <remarks>If <c>true</c> Sends an email when the test suite finishes with a success.</remarks>
        [TestVariable("670a378a-e239-43e7-8325-c216fd11f190")]
        public bool SendEmailOnSuccess { get; set; }

        /// <summary>
        /// Gets or sets whether the email should send the report as an attachement when the test suite finishes.
        /// </summary>
        [TestVariable("154c39b9-9dd8-4f75-934e-973ef4c5de5b")]
        public bool SendZippedReportOnComplete { get; set; }

        /// <summary>
        /// Gets or sets whether the email should send the pdf version of the report as an attachement when the test suite finishes.
        /// </summary>
        [TestVariable("7038d9db-9189-4fe3-9fb3-13f3cacde5a0")]
        public bool SendPdfReportOnComplete { get; set; }

        /// <summary>
        /// If <see cref="SendPdfReportOnComplete"/> is set to true, this can be used to set a custom stylesheet.
        /// Don't set or set to an empty string to use the default stylesheet.
        /// See https://www.ranorex.com/blog/ranorex-report-to-pdf-conversion#stylesheet
        /// </summary>
        [TestVariable("B56D72A4-8257-4EC5-A054-455D9094B56A")]
        public string PdfReportCustomStylesheet { get; set; }

        /// <summary>
        /// Sends the report via email after the test run has completed. Use this module in
        /// the TearDown of your test case to ensure that it is executed even on failing test runs.
        /// </summary>
        /// <remarks>You should not call this method directly. Instead, pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            //Will be executed at the very end of the TestSuite
            TestSuite.TestSuiteCompleted += this.OnTestSuiteCompletedSendResult;
        }

        private void OnTestSuiteCompletedSendResult(object sender, EventArgs e)
        {
            var currentTestSuiteStatus = TestReport.CurrentTestSuiteActivity;
            var reportFile = CreateReports();

            if (this.SendEmailOnFailure && currentTestSuiteStatus.Status == ActivityStatus.Failed
                || this.SendEmailOnSuccess && currentTestSuiteStatus.Status == ActivityStatus.Success
                || this.SendZippedReportOnComplete && !this.SendEmailOnFailure && !this.SendEmailOnSuccess
                || this.SendPdfReportOnComplete && !this.SendEmailOnFailure && !this.SendEmailOnSuccess)
            {
                EmailLibrary.SendEmail(
                    this.Subject,
                    this.To,
                    this.From,
                    this.Body,
                    reportFile,
                    this.ServerHostname,
                    int.Parse(this.ServerPort),
                    this.UseSSL,
                    this.Username,
                    this.Password);
            }
        }

        private string CreateCompressedReport()
        {
            if (this.SendZippedReportOnComplete)
            {
                //Necessary to end the Ranorex Report in order to update the duration and finalize the status
                FinishTestReport();

                // zip the current report
                var zippedReportFileDirectory = TestReport.ReportEnvironment.ReportFileDirectory;
                var name = TestReport.ReportEnvironment.ReportName;

                Report.Zip(TestReport.ReportEnvironment, zippedReportFileDirectory, name);
                Report.Info("Zipped report has been generated.");
                return string.Format("{0}\\{1}.rxzlog", zippedReportFileDirectory, TestReport.ReportEnvironment.ReportName);
            }

            return string.Empty;
        }

        private void FinishTestReport()
        {
            if (ActivityStack.Current is TestSuiteActivity)
            {
                TestReport.EndTestModule();
                Report.End();
            }

            TestReport.SaveReport();
        }

        private string CreatePdfReport()
        {
            if (this.SendPdfReportOnComplete)
            {
                ReportToPDFModule pdfModule = new ReportToPDFModule();
                pdfModule.PdfName = TestReport.ReportEnvironment.ReportName + ".pdf";
                pdfModule.Xml = this.PdfReportCustomStylesheet ?? "";
                pdfModule.Details = "all";
                return pdfModule.CreatePDF();
            }

            return string.Empty;
        }

        private string[] CreateReports()
        {
            return new string[] { CreateCompressedReport(), CreatePdfReport() };
        }
    }
}
