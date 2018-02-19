/*
 * User: RobinHood42
 * Date: 19/02/2018
 * Time: 10:36
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using Ranorex.Core.Reporting;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    /// Functionality: Collects all error messages from given test container.
    /// Usage: Use module within a test container with enabled retry count
    /// </summary>
    [TestModule("E4E50BBC-20F8-461F-B39A-863B6F69DE89", ModuleType.UserCode, 1)]
    public class CollectErrorMessagesModule : ITestModule
    {
        public static List<string> builder = new List<string>();

        private static string lastTestContainerName = String.Empty;
        private static int retryIteration;
        private static bool registered = false;

        public CollectErrorMessagesModule()
        {
            // Do not delete - a parameterless constructor is required!
        }

        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;

            //Delegate must be registered only once
            if (!CollectErrorMessagesModule.registered)
            {
                //PDF will be generated at the very end of the TestSuite
                TestSuite.TestSuiteCompleted += delegate
                {
                    LogErrorMessages();
                };

                CollectErrorMessagesModule.registered = true;
            }

            //Store name of TestContainer and reset retryIteration in case of new TestContainer
            if (!String.IsNullOrEmpty(lastTestContainerName))
            {
                if (lastTestContainerName != TestReport.CurrentTestContainerActivity.FullDisplayName)
                {
                    CollectErrorMessagesModule.retryIteration = 1;
                }
                else
                {
                    CollectErrorMessagesModule.retryIteration++;
                }
            }

            if (String.IsNullOrEmpty(lastTestContainerName))
            {
                CollectErrorMessagesModule.lastTestContainerName = TestReport.CurrentTestContainerActivity.FullDisplayName;
                CollectErrorMessagesModule.retryIteration = 1;
            }

            //GetErrorMessages from TestContainer   
            var container = TestReport.CurrentTestContainerActivity;

            //Only collect error messages if the test run failed
            if (container.Status == ActivityStatus.Failed)
            {
                GetErrorMessages((TestContainerActivity)container);
            }
        }

        /// <summary>
        /// Recursively collects all error messages from given test container
        /// </summary>
        /// <param name="container"></param>
        private void GetErrorMessages(TestContainerActivity container)
        {
            foreach (var containerChild in container.Children)
            {
                var containerChildType = containerChild.GetType().Name;

                //Check if TestContainerActivity
                if (containerChildType == "TestContainerActivity")
                {
                    GetErrorMessages((TestContainerActivity)containerChild);
                }

                //Check if TestModuleActivity
                if (containerChildType == "TestModuleActivity")
                {
                    var castedContainerChild = containerChild as TestModuleActivity;

                    foreach (var testModuleChild in castedContainerChild.Children)
                    {
                        var testModuleChildType = testModuleChild.GetType().Name;

                        //Check if testModuleChild is ReportItem
                        if (testModuleChildType == "ReportItem")
                        {
                            var castedTestModuleChild = testModuleChild as ReportItem;

                            if (castedTestModuleChild.Level == ReportLevel.Failure || castedTestModuleChild.Level == ReportLevel.Error)
                            {
                                //Add error message to string builder
                                builder.Add(String.Format("Module: {0} | Parent (Iteration): {1}({2}) ReportLevel: {3} | Message: {4}", castedContainerChild.FullDisplayName, container.FullDisplayName, CollectErrorMessagesModule.retryIteration, castedTestModuleChild.Level, castedTestModuleChild.Message));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Logs all collected error messages to the After Test Suite section of the Ranorex report
        /// </summary>
        private void LogErrorMessages()
        {
            //End report 
            TestReport.EndTestModule();

            //Report collected error messages
            foreach (var message in CollectErrorMessagesModule.builder)
            {
                Report.Info(message);
            }
        }
    }
}
