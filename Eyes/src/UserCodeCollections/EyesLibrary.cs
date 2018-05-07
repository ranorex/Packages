using System;
using System.Drawing;
using Ranorex.Controls;
using Ranorex.Core.Testing;

namespace Ranorex.Eyes
{
    /// <summary>
    /// Ranorex user code collection for Applitools Eyes.
    /// </summary>
    [UserCodeCollection]
    public static class EyesLibrary
    {
        private static WebDocument.CapturePageFlags screenshotCaptureFlag = WebDocument.CapturePageFlags.DisableCssTransitions;

        /// <summary>
        /// Set the AppName for comparisons between the test and the baseline (default: Test suite name)
        /// </summary>
        /// <param name="newAppName">The name of the application under test.</param>
        [UserCodeMethod]
        public static void ChangeAppName(string newAppName)
        {
            EyesWrapper.SetAppName(newAppName);
        }

        /// <summary>
        /// Set the MatchLevel for comparisons between the test and the baseline (default: Strict)
        /// </summary>
        /// <param name="matchLevel">Allowed levels: Exact, Strict, Content, Layout.</param>
        [UserCodeMethod]
        public static void ChangeMatchLevel(string matchLevel)
        {
            EyesWrapper.SetMatchLevel(matchLevel);
        }

        /// <summary>
        /// Set the information for the current batch of comparisons
        /// </summary>
        /// <param name="batchName">The name of the patch.</param>
        /// <param name="batchId">The identifier of the patch (optional).</param>
        [UserCodeMethod]
        public static void SetBatch(string batchName, string batchId)
        {
            EyesWrapper.SetBatchName(batchName);
            if (!string.IsNullOrEmpty(batchId))
            {
                EyesWrapper.SetBatchId(batchId);
            }
        }

        /// <summary>
        /// Set the behavior of the screenshot capture operation (default: DisableCssTransitions) for elements of a
        /// Webdocument for the remaining test run.
        /// </summary>
        /// <param name="flagName">The name of the <see cref="WebDocument.CapturePageFlags"/>: None, DisableCssTransitions or HidePositionFixed</param>
        [UserCodeMethod]
        public static void ChangeScreenshotCaptureBehaviour(string flagName)
        {
            WebDocument.CapturePageFlags newFlag;
            if (!Enum.TryParse(flagName, out newFlag))
            {
                throw new ArgumentException(
                    string.Format(
                        "Not a valid option '{0}'. Please use: {1}",
                        flagName,
                        string.Join(", ", Enum.GetNames(typeof(WebDocument.CapturePageFlags)))));
            }

            Report.Info(string.Format("Changing screenshot capture from '{0}' to '{1}'", screenshotCaptureFlag, newFlag));
            screenshotCaptureFlag = newFlag;
        }

        /// <summary>
        /// Perform a visual check (utilizing Applitools eyes) on the passed document or folder.
        /// </summary>
        /// <param name="fileOrFolderPath">Folder of image-file (Folder: will be processed recursiveley; relative and absolute paths are allowed)</param>
        [UserCodeMethod]
        public static void VisualCheckpoint(string fileOrFolderPath)
        {
            // Handling of filename / folder
            fileOrFolderPath = fileOrFolderPath.Trim();
            if (!System.IO.File.Exists(fileOrFolderPath) && !System.IO.Directory.Exists(fileOrFolderPath))
            {
                fileOrFolderPath = System.IO.Directory.GetCurrentDirectory() + @"\" + fileOrFolderPath;
            }

            EyesWrapper.CheckFolder(fileOrFolderPath);
        }

        /// <summary>
        /// Perform a visual check (utilizing Applitools eyes) on the passed element.
        /// </summary>
        /// <param name="adapter">The element for the visual checkpoint (from any type).</param>
        [UserCodeMethod]
        public static void VisualCheckpoint(Adapter adapter)
        {
            VisualCheckpoint(adapter, string.Empty);
        }

        /// <summary>
        /// Perform a visual check (utilizing Applitools eyes) on the passed element. The parameter stepDescription is optional.
        /// </summary>
        /// <param name="adapter">The element for the visual checkpoint (from any type)</param>///
        /// <param name="stepDescription">Description of the test step</param>///
        [UserCodeMethod]
        public static void VisualCheckpoint(Adapter adapter, string stepDescription)
        {
        	if (adapter == null)
        	{
        		throw new ArgumentNullException("adapter");
        	}
        	
            var testCaseName = string.Empty;
            if (TestSuite.Current != null)
            {
                testCaseName = TestSuite.Current.CurrentTestContainer.Name;
            }

            EyesWrapper.StartOrContinueTest(testCaseName);
            Report.Info(string.Format("Applitools 'CheckImage' called with screenshot from repository item '{0}'.", adapter));

            try
            {
                ProgressForm.SetOpacity(0);

                Bitmap image;
                if (adapter.Element.HasCapability("webdocument"))
                {
                    var webDocument = adapter.As<WebDocument>();
                    EyesWrapper.SetBrowserName(webDocument.BrowserName);
                    if (EyesWrapper.ViewPortWidth > 0 && EyesWrapper.ViewPortHeight > 0)
                    {
                        webDocument.Browser.Resize(EyesWrapper.ViewPortWidth, EyesWrapper.ViewPortHeight);
                    }

                    webDocument.WaitForDocumentLoaded();
                    image = webDocument.CaptureFullPageScreenshot(screenshotCaptureFlag);
                }
                else
                {
                    var browserName = string.Empty;
                    if (adapter.Element.HasCapability("webelement"))
                    {
                        var parent = adapter.Parent;
                        while (parent.As<WebDocument>() == null)
                        {
                            parent = parent.Parent;
                        }

                        browserName = parent.As<WebDocument>().BrowserName;
                    }

                    EyesWrapper.SetBrowserName(browserName);
                    image = Imaging.CaptureImage(adapter.Element);
                }

                EyesWrapper.CheckImage(image, stepDescription);
            }
            finally
            {
                ProgressForm.SetOpacity(100);
            }
        }
    }
}
