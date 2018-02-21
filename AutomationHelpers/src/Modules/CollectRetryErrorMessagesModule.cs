//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.Collections.Generic;
using Ranorex.Core.Testing;
using Ranorex.Core.Reporting;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    /// When placed in a test container with a retry count >= 1, collects all error messages even if one of the retries
    /// was successful and adds them to the report.
    /// </summary>
    [TestModule("E4E50BBC-20F8-461F-B39A-863B6F69DE89", ModuleType.UserCode)]
    public class CollectRetryErrorMessagesModule : ITestModule
    {
        private static List<string> errorMessages = new List<string>();
        private static string lastTestContainerName;
        private static int retryIteration;
        private static bool registered;

        public CollectRetryErrorMessagesModule()
        {
            // Do not delete - a parameterless constructor is required!
        }

        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;

            Run();
        }

        private static void Run()
        {
            // Delegate must be registered only once
            if (!registered)
            {
                //Messages will be added to the report at the very end of the TestSuite
                TestSuite.TestSuiteCompleted += LogErrorMessages;
                registered = true;
            }

            // Store name of TestContainer and reset retryIteration in case of new TestContainer
            if (!string.IsNullOrEmpty(lastTestContainerName))
            {
                if (lastTestContainerName != TestReport.CurrentTestContainerActivity.FullDisplayName)
                {
                    retryIteration = 1;
                }
                else
                {
                    retryIteration++;
                }
            }

            if (string.IsNullOrEmpty(lastTestContainerName))
            {
                lastTestContainerName = TestReport.CurrentTestContainerActivity.FullDisplayName;
                retryIteration = 1;
            }

            // GetErrorMessages from TestContainer
            var container = TestReport.CurrentTestContainerActivity;
            // Only collect error messages if the test run failed
            if (container.Status == ActivityStatus.Failed)
            {
                GetErrorMessages(container);
            }
        }

        /// <summary>
        /// Recursively collects all error messages from given test container
        /// </summary>
        /// <param name="container"></param>
        private static void GetErrorMessages(ITestContainerActivity container)
        {
            foreach (var containerChild in container.Children)
            {
                // Check if TestContainerActivity
                var testContainerActivity = containerChild as ITestContainerActivity;
                if (testContainerActivity != null)
                {
                    GetErrorMessages(testContainerActivity);
                }

                // Check if TestModuleActivity
                var testModuleActivity = containerChild as ITestModuleActivity;
                if (testModuleActivity != null)
                {
                    AddErrorMessages(
                        testModuleActivity.TestModuleName,
                        container.FullDisplayName,
                        testModuleActivity.Children);
                }
            }
        }

        private static void AddErrorMessages(
            string testModuleActivityFullDisplayName,
            string containerFullDisplayName,
            IEnumerable<IReportItem> items)
        {
            foreach (var testModuleChild in items)
            {
                // Check if testModuleChild is ReportItem
                var reportItem = testModuleChild as ReportItem;
                if (reportItem != null
                    && (reportItem.Level == ReportLevel.Error || reportItem.Level == ReportLevel.Failure))
                {
                    // Store error message for report on test suite completed event
                    errorMessages.Add(
                        string.Format(
                            "Module: {0} | Parent (Iteration): {1}({2}) ReportLevel: {3} | Message: {4}",
                            testModuleActivityFullDisplayName,
                            containerFullDisplayName,
                            retryIteration,
                            reportItem.Level,
                            reportItem.Message));
                }
            }
        }

        /// <summary>
        /// Logs all collected error messages to the "After Test Suite" section of the Ranorex report
        /// </summary>
        private static void LogErrorMessages(object sender, EventArgs e)
        {
            // End report
            TestReport.EndTestModule();

            // Report collected error messages
            foreach (var message in errorMessages)
            {
                Report.Info("RetryError", message);
            }
        }
    }
}
