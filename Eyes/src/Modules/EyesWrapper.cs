using System;
using System.Drawing;
using Ranorex.Core.Testing;
using Applitools;
using Applitools.ImageTester;

namespace Ranorex.Eyes
{
    internal static class EyesWrapper
    {
        internal static readonly log4net.LogManager mgr = null; // explicit ref

        private static readonly Applitools.Images.Eyes eyes = new Applitools.Images.Eyes();
        private static readonly BatchInfo batch = new BatchInfo();

        private static string appName;
        private static string currentTestName = string.Empty;
        private static string currentBrowserName = string.Empty;
        private static bool testRunning;

        public static int ViewPortHeight { get; set; }
        public static int ViewPortWidth { get; set; }

        public static void Initialize(
            string apiKey,
            string appName,
            string serverURL,
            string batchId,
            int portWidth,
            int portHeight,
            string matchLevel)
        {
            eyes.SetAppEnvironment(Host.Local.OSEdition, currentBrowserName);
            eyes.ApiKey = apiKey;

            SetAppName(appName);

            if (!string.IsNullOrWhiteSpace(serverURL))
            {
                eyes.ServerUrl = serverURL;
            }

            SetMatchLevel(matchLevel);
            SetBatchName(TestSuite.Current.Name);
            if (!string.IsNullOrWhiteSpace(batchId))
            {
                SetBatchId(batchId);
            }

            ViewPortWidth = portWidth;
            ViewPortHeight = portHeight;
        }

        public static void CheckImage(Bitmap image, string tag)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            eyes.CheckImage(image, tag);
        }

        public static void CheckFolder(string fileOrFolderPath, string appName)
        {
            var viewPort = Size.Empty;
            if (ViewPortWidth > 0 && ViewPortHeight > 0)
            {
                viewPort = new Size(ViewPortWidth, ViewPortHeight);
            }

            var builder = new SuiteBuilder(fileOrFolderPath, appName, viewPort);

            var suite = builder.Build();
            if (suite == null)
            {
                Console.WriteLine("Nothing to test!");
                return;
            }

            Report.Info("Visual Checkpoint - file comparison (PDF/Images).");

            suite.Run(eyes);
        }

        public static void CloseTest(bool throwException)
        {
            if (testRunning)
            {
                try
                {
                    eyes.Close(throwException);
                }
                catch (NewTestException e)
                {
                    Report.LogHtml(ReportLevel.Warn, "Visual Testing", string.Format("New baseline created; please approve here: <a href='{0}'>Applitools backend</a>", e.TestResults.Url));
                }
                catch (TestFailedException e)
                {
                    Report.LogHtml(ReportLevel.Failure, "Visual Testing", string.Format("Visual test failed; please check results here: <a href='{0}'>Applitools backend</a>", e.TestResults.Url));
                }
                finally
                {
                    testRunning = false;
                }
            }
        }

        public static void StartOrContinueTest(string testName)
        {
            if (!testRunning)
            {
                eyes.Open(appName, testName, new Size(ViewPortWidth, ViewPortHeight));
                currentTestName = testName;
                testRunning = true;
            }
            else
            {
                if (!testName.Equals(currentTestName))
                {
                    CloseTest(true);
                    StartOrContinueTest(testName);
                }
            }
        }

        public static void SetAppName(string newAppName)
        {
            appName = newAppName;
        }

        public static void SetMatchLevel(string matchLevel)
        {
            Report.Debug(String.Format("Setting Match-Level to: {0}", matchLevel));

            if (!string.IsNullOrEmpty(matchLevel))
            {
                if (matchLevel.ToUpper().Contains("LAYOUT"))
                {
                    eyes.DefaultMatchSettings.MatchLevel = MatchLevel.Layout;
                }
                else if (matchLevel.ToUpper().Contains("EXACT"))
                {
                    eyes.DefaultMatchSettings.MatchLevel = MatchLevel.Exact;
                }
                else if (matchLevel.ToUpper().Contains("CONTENT"))
                {
                    eyes.DefaultMatchSettings.MatchLevel = MatchLevel.Content;
                }
                else
                {
                    Report.Warn(string.Format("MatchLevel {0} is not valid; fallback to default (strict)", matchLevel));
                    eyes.DefaultMatchSettings.MatchLevel = MatchLevel.Strict; // Default
                }
            }
            else
            {
                eyes.DefaultMatchSettings.MatchLevel = MatchLevel.Strict; // Default
            }
        }

        public static void SetBatchId(string batchId)
        {
            batch.Id = batchId;
            eyes.Batch = batch;
        }

        public static void SetBatchName(string batchName)
        {
            batch.Name = batchName;
            eyes.Batch = batch;
        }

        internal static void SetBrowserName(string browserName)
        {
            if (currentBrowserName.Equals(browserName))
            {
                return;
            }

            currentBrowserName = browserName;
            eyes.SetAppEnvironment(Host.Local.OSEdition, browserName);
        }
    }
}
