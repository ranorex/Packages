//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using Ranorex.AutomationHelpers.UserCodeCollections;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    ///     Used to send emails from a testsuite.
    /// </summary>
    [TestModule("D8198CC7-82F5-46B2-81E7-3ED789544877", ModuleType.UserCode)]
    public sealed class EmailModule : ITestModule
    {
        private bool sendResultOnFailure;
        private bool sendResultOnSuccess;
        private bool sendZippedReportOnComplete;

        /// <summary>
        ///     Constructs a new instance.
        /// </summary>
        public EmailModule()
        {
            this.From = "";
            this.Body = "";
            this.Password = "";
            this.sendResultOnSuccess = false;

            this.ServerHostname = "";
            this.ServerPort = "";
            this.UseSSL = "false";
            this.Username = "";
        }

        /// <summary>
        ///     Gets or sets the value of the Email sender.
        /// </summary>
        [TestVariable("718a0466-d3a5-4d93-93a0-91ccd5f1a19e")]
        public string From { get; set; }

        /// <summary>
        ///     Gets or sets the value of the Email body.
        /// </summary>
        [TestVariable("ef2dc4ee-14a8-483f-92ad-f2c6bd6d67db")]
        public string Body { get; set; }

        /// <summary>
        ///     Gets or sets the value of the user password when connecting to the email-server.
        /// </summary>
        [TestVariable("b97a0c41-28c8-44ed-b089-a8ca4b6af9d7")]
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the value to send the mail when the test suite completes with a failure (<c>true</c> or <c>false</c>).
        /// </summary>
        /// <remarks>If <c>true</c> Sends an email when the testsuite finishes (at the end of the testsuite run) with a failure.</remarks>
        [TestVariable("07580bd9-744c-4666-83a6-ba22c0c8d909")]
        public string SendResultOnFailure
        {
            get { return this.sendResultOnFailure.ToString(); }
            set
            {
                TestSuite.TestSuiteCompleted -= this.OnTestSuiteCompletedSendResultOnFailure;
                this.sendResultOnFailure = bool.Parse(value);

                if (this.sendResultOnFailure)
                {
                    TestSuite.TestSuiteCompleted += this.OnTestSuiteCompletedSendResultOnFailure;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value to send the mail when the test suite completes successfully (<c>true</c> or <c>false</c>).
        /// </summary>
        /// <remarks>If <c>true</c> Sends an email when the testsuite finishes (at the end of the testsuite run) with a success.</remarks>
        [TestVariable("670a378a-e239-43e7-8325-c216fd11f190")]
        public string SendResultOnSuccess
        {
            get { return this.sendResultOnSuccess.ToString(); }
            set
            {
                TestSuite.TestSuiteCompleted -= this.OnTestSuiteCompletedSendResultOnSuccess;
                this.sendResultOnSuccess = bool.Parse(value);

                if (this.sendResultOnSuccess)
                {
                    TestSuite.TestSuiteCompleted += this.OnTestSuiteCompletedSendResultOnSuccess;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value of the email-server hostname.
        /// </summary>
        [TestVariable("2436c39d-0705-4ec3-85fb-5e9835a1ab18")]
        public string ServerHostname { get; set; }

        /// <summary>
        ///     Gets or sets the value of the email-server port.
        /// </summary>
        [TestVariable("e255daf0-00ef-4b39-a594-273d574839a1")]
        public string ServerPort { get; set; }

        /// <summary>
        ///     Gets or sets the value of the email subject.
        /// </summary>
        [TestVariable("279773ef-ef45-414a-a555-48cb80bfc115")]
        public string Subject { get; set; }

        /// <summary>
        ///     Gets or sets the value of the email recipient.
        /// </summary>
        [TestVariable("12fd536f-980f-4e5a-a921-acaf0c6247e5")]
        public string To { get; set; }

        /// <summary>
        ///     Gets or sets the value of the user name to use when connecting to the email-server.
        /// </summary>
        [TestVariable("38abb172-e49e-4d92-a24f-145709bdd7e3")]
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets whether SSL is used or not (true or false) to connect to the email-server.
        /// </summary>
        [TestVariable("c898ee67-ee7f-4258-ab69-d855b0d92274")]
        public string UseSSL { get; set; }

        /// <summary>
        ///     Gets or sets whether the email should send the report in an attachement, when the TestSuite finishes.
        /// </summary>
        [TestVariable("154c39b9-9dd8-4f75-934e-973ef4c5de5b")]
        public string SendZippedReportOnComplete
        {
            get { return this.sendZippedReportOnComplete.ToString(); }
            set
            {
                TestSuite.TestSuiteCompleted -= this.OnTestSuiteCompletedSendZippedReport;
                this.sendZippedReportOnComplete = bool.Parse(value);

                if (this.sendZippedReportOnComplete)
                {
                    TestSuite.TestSuiteCompleted += this.OnTestSuiteCompletedSendZippedReport;
                }
            }
        }

        public void Run()
        {
            if (!this.sendResultOnFailure && !this.sendResultOnSuccess)
            {
                this.DoSendMail();
            }
        }

        private void DoSendMail()
        {
            EmailLibrary.SendEmail(
                this.Subject,
                this.Body,
                this.To,
                this.From,
                this.ServerHostname,
                this.ServerPort,
                bool.Parse(this.UseSSL),
                this.Username,
                this.Password);
        }

        private void OnTestSuiteCompletedSendResultOnFailure(object sender, EventArgs e)
        {
            if (TestReport.CurrentTestSuiteActivity.Status == ActivityStatus.Failed)
            {
                this.DoSendMail();
            }
        }

        private void OnTestSuiteCompletedSendResultOnSuccess(object sender, EventArgs e)
        {
            if (TestReport.CurrentTestSuiteActivity.Status == ActivityStatus.Success)
            {
                this.DoSendMail();
            }
        }

        private void OnTestSuiteCompletedSendZippedReport(object sender, EventArgs e)
        {
            //Necessary to end the testreport in order to update the duration
            TestReport.EndTestModule();

            // zip the current report
            var zippedReportFileDirectory = TestReport.ReportEnvironment.ReportFileDirectory;
            var name = TestReport.ReportEnvironment.ReportName;

            TestReport.SaveReport();
            Report.Zip(TestReport.ReportEnvironment, zippedReportFileDirectory, name);

            // send ziped report
            EmailLibrary.SendReportViaEmail(
                subject: this.Subject,
                body: this.Body,
                to: this.To,
                from: this.From,
                serverHostname: this.ServerHostname,
                serverPort: int.Parse(this.ServerPort),
                useSSL: bool.Parse(this.UseSSL),
                username: this.Username,
                password: this.Password);
        }
    }
}