using System;
using System.Drawing;
using Applitools;
using Applitools.Images;
using Ranorex.Core.Testing;


namespace Ranorex.Eyes
{
    internal static class EyesWrapper
    {
        internal static readonly log4net.LogManager _mgr = null; // explicit ref

        private static readonly ClassicRunner _runner = new ClassicRunner();
        private static readonly Applitools.Images.Eyes _eyes = new Applitools.Images.Eyes(_runner);
        private static readonly Configuration _suitConfiguration = new Configuration();
        private static BatchInfo _batch = new BatchInfo();

        private static string _appName;
        private static string _currentTestName = string.Empty;
        private static string _currentBrowserName = string.Empty;
        private static bool _testRunning;

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
            _suitConfiguration
                .SetApiKey(apiKey)
                .SetAppName(appName)
                .SetHideCaret(true)
                .SetHideScrollbars(true)
                .SetHostOS(Host.Local.OSEdition);

            SetAppName(appName);
            SetMatchLevel(matchLevel);

            if (!string.IsNullOrWhiteSpace(serverURL))
            {
                _suitConfiguration.SetServerUrl(serverURL);
            }

            if (TestSuite.Current != null)
            {
                SetBatchName(TestSuite.Current.Name);
            }

            if (!string.IsNullOrWhiteSpace(batchId))
            {
                SetBatchId(batchId);
            }

            ViewPortWidth = portWidth;
            ViewPortHeight = portHeight;
            _suitConfiguration.SetViewportSize(ViewPortWidth, ViewPortHeight);

            _eyes.SetConfiguration(_suitConfiguration);
        }

        public static void CheckImage(Bitmap image, string tag)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            _eyes.CheckImage(image, tag);
        }

        public static void CheckFolder(string fileOrFolderPath)
        {
            var viewPort = Size.Empty;
            if (ViewPortWidth > 0 && ViewPortHeight > 0)
            {
                viewPort = new Size(ViewPortWidth, ViewPortHeight);
            }

            var builder = new Applitools.ImageTester.SuiteBuilder(fileOrFolderPath, _appName, viewPort);

            var suite = builder.Build();
            if (suite == null)
            {
                Console.WriteLine("Nothing to test!");
                return;
            }

            Report.Info("Visual Checkpoint - file comparison (PDF/Images).");
            suite.Run(_eyes);
        }

        [Obsolete("Parameter throwException is no longer used, please use CloseTest() instead.", false)]
        public static void CloseTest(bool throwException)
        {
            CloseTest();
        }

        public static void CloseTest()
        {
            TestResults results;
            if (_testRunning)
            {
                _eyes.CloseAsync(false);
                var allTestResults = _runner.GetAllTestResults(false);

                foreach (var testResults in allTestResults)
                {
                    results = testResults.TestResults;
                    if (results.IsNew)
                    {
                        Report.LogHtml(ReportLevel.Warn, "Visual Testing", string.Format("New baseline created; please approve here: <a href='{0}'>Applitools backend</a>", results.Url));
                    }
                    if (results.IsPassed)
                    {
                        Report.LogHtml(ReportLevel.Info, "Visual Testing", string.Format("Visual test passed; check results here: <a href='{0}'>Applitools backend</a>", results.Url));
                    }
                    else
                    {
                        Report.LogHtml(ReportLevel.Failure, "Visual Testing", string.Format("Visual test failed; please check results here: <a href='{0}'>Applitools backend</a>", results.Url));
                    }
                }

                _testRunning = false;
                _eyes.AbortIfNotClosed();
            }
        }

        public static void StartOrContinueTest(string testName)
        {
            if (!_testRunning)
            {
                _eyes.Open(_appName, testName);
                _currentTestName = testName;
                _testRunning = true;
            }
            else
            {
                if (!testName.Equals(_currentTestName))
                {
                    CloseTest();
                    StartOrContinueTest(testName);
                }
            }
        }

        public static void SetAppName(string newAppName)
        {
            _appName = newAppName;
        }

        public static void SetMatchLevel(string matchLevel)
        {
            Report.Debug(String.Format("Setting Match-Level to: {0}", matchLevel));

            if (!string.IsNullOrEmpty(matchLevel))
            {
                if (matchLevel.ToUpper().Contains("LAYOUT"))
                {
                    _suitConfiguration.MatchLevel = MatchLevel.Layout;
                }
                else if (matchLevel.ToUpper().Contains("EXACT"))
                {
                    _suitConfiguration.MatchLevel = MatchLevel.Exact;
                }
                else if (matchLevel.ToUpper().Contains("IGNORECOLORS"))
                {
                    _suitConfiguration.MatchLevel = MatchLevel.IgnoreColors;
                }
                else if (matchLevel.ToUpper().Contains("STRICT"))
                {
                    _suitConfiguration.MatchLevel = MatchLevel.Strict;
                }
                else
                {
                    Report.Warn(string.Format("MatchLevel {0} is not valid; fallback to default (strict)", matchLevel));
                    _suitConfiguration.MatchLevel = MatchLevel.Strict; // Default
                }
            }
            else
            {
                _suitConfiguration.MatchLevel = MatchLevel.Strict; // Default
            }
        }

        public static void SetBatchId(string batchId)
        {
            if (_suitConfiguration.Batch != null) 
            {
                _batch = _suitConfiguration.Batch;
            }

            _batch.Id = batchId;
            _suitConfiguration.Batch = _batch;
        }

        public static void SetBatchName(string batchName)
        {
            if (_suitConfiguration.Batch != null)
            {
                _batch = _suitConfiguration.Batch;
            }

            _batch.Name = batchName;
            _suitConfiguration.Batch = _batch;
        }

        internal static void SetBrowserName(string browserName)
        {
            if (_currentBrowserName.Equals(browserName))
            {
                return;
            }

            _currentBrowserName = browserName;
            var configuration = _eyes.GetConfiguration();
            configuration.SetHostApp(browserName);
            _eyes.SetConfiguration(configuration);
            _eyes.SetAppEnvironment(Host.Local.OSEdition, browserName);
        }
    }
}
