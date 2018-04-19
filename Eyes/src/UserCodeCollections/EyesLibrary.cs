using System;
using System.Diagnostics;
using Ranorex.Controls;
using Ranorex.Core.Testing;

namespace Ranorex.Eyes
{
    /// <summary>
    /// Ranorex user code collection for Applitools Eyes.
    /// </summary>
    [UserCodeCollection]
    internal static class EyesIntegrationCollection
    {
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
            EyesWrapper.SetBatch(batchName, batchId);
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
            if (!System.IO.File.Exists(fileOrFolderPath))
            {
                fileOrFolderPath = System.IO.Directory.GetCurrentDirectory() + @"\" + fileOrFolderPath;
            }

            EyesWrapper.CheckFolder(fileOrFolderPath, TestSuite.Current.CurrentTestContainer.Name);
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
        /// <param name="stepDescription">Optional: Description of the element</param>///        
        [UserCodeMethod]
        public static void VisualCheckpoint(Adapter adapter, string stepDescription)
        {
            var testCaseName = string.Empty;
            if (TestSuite.Current != null)
            {
                testCaseName = TestSuite.Current.CurrentTestContainer.Name;
            }

            if (string.IsNullOrWhiteSpace(stepDescription))
            {
                stepDescription = "Unnamed test step";
            }

            EyesWrapper.StartOrContinueTest(stepDescription, testCaseName);
            Report.Info(string.Format("Applitools 'CheckImage' called with screenshot from repository item '{0}'.", adapter));

            try
            {
                ProgressForm.SetOpacity(0);

                if (adapter.Element.HasCapability("webdocument"))
                {
                    adapter.As<WebDocument>().Browser.Resize(EyesWrapper.ViewPortWidth, EyesWrapper.ViewPortHeight);
                    adapter.As<WebDocument>().WaitForDocumentLoaded();
                }

                var image = adapter.Element.HasCapability("webdocument")
                    ? adapter.As<WebDocument>().CaptureFullPageScreenshot(WebDocument.CapturePageFlags.DisableCssTransitions)
                    : Imaging.CaptureImage(adapter.Element);

                EyesWrapper.CheckImage(image, stepDescription);
            }
            finally
            {
                ProgressForm.SetOpacity(100);
            }
        }
    }
}
