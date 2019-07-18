//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.Drawing;
using System.IO;
using System.Net;
using Ranorex.Controls;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of methods to help automate web and network-based tasks.
    /// </summary>
    [UserCodeCollection]
    public static class WebLibrary
    {
        private const string libraryName = "WebLibrary";

        /// <summary>
        /// Downloads a file and stores it locally.
        /// </summary>
        /// <param name="uri">The uri of the file to download</param>
        /// <param name="localPath">Local location where the file should be stored</param>
        /// <param name="overwriteExisting">true / false if existing file should be overwritten</param>
        [UserCodeMethod]
        public static void DownloadFile(
            string uri, string localPath, bool overwriteExisting = true)
        {
            Uri result = null;
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out result) && localPath != null && !Path.HasExtension(localPath))
            {
                localPath = Path.Combine(localPath, Path.GetFileName(result.LocalPath));
            }

            if (!File.Exists(localPath) || overwriteExisting)
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(uri, localPath);
                        string message = string.Format(
                            "File successfully downloaded to {0}",
                            new FileInfo(localPath).FullName);
                        Report.Log(ReportLevel.Success, message);
                    }
                    catch (Exception e)
                    {
                        string message = string.Format(
                            "Downloading a file from: {0} failed for the following reason:",
                            uri);
                        message = string.Concat(message, Environment.NewLine, e.GetFullMessage(Environment.NewLine));

                        Report.Log(ReportLevel.Failure, message);
                    }
                }
            }
            else
            {
                string message = string.Format(
                    "The file {0} already exists in the local file, the download will be skipped",
                    new FileInfo(localPath).FullName);
                Report.Log(ReportLevel.Warn, message);
            }
        }

        /// <summary>
        /// Returns the HTTP status code from a URL.
        /// </summary>
        /// <param name="url">The URL to query for the status code.</param>
        [UserCodeMethod]
        public static string GetHttpStatusCode(string url)
        {
            HttpWebResponse resp = null;

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                return ((int)((HttpWebResponse) we.Response).StatusCode).ToString();
            }

            return ((int)resp.StatusCode).ToString();
        }

        /// <summary>
        /// Takes screenshot of entire web page and reports it.
        /// </summary>
        /// <param name="repoItemInfo">Repository item</param>
        [UserCodeMethod]
        public static void ReportFullPageScreenshot(RepoItemInfo repoItemInfo)
        {
            ProgressForm.Hide();

            try
            {
                Utils.CheckArgumentNotNull(repoItemInfo, "repoItemInfo");

                var webDocument = repoItemInfo.CreateAdapter<WebDocument>(false);

                if (webDocument == null)
                {
                    Report.Error("Repository item '" + repoItemInfo.FullName + "' is not a web document. " +
                                 "Screenshot can be taken only for web documents.");
                    return;
                }

                var screenshotFilePath = Path.GetTempFileName();

                var screenshot = webDocument.CaptureFullPageScreenshot();
                screenshot.Save(screenshotFilePath);

                Report.LogData(ReportLevel.Info, "Screenshot", screenshot);

                if (File.Exists(screenshotFilePath))
                {
                    try
                    {
                        File.Delete(screenshotFilePath);
                    }
                    catch
                    {
                        // No need to handle exception.
                        // Temp files are deleted only to prevent piling up of unnecessary files.
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ReportException(ex, libraryName);
            }

            ProgressForm.Show();
        }
    }
}
