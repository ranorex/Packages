//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using System.Net;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of methods to help automate web and network-based tasks.
    /// </summary>
    [UserCodeCollection]
    public static class WebLibrary
    {
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
                        while (e != null)
                        {
                            message = string.Concat(message, Environment.NewLine, e.Message);
                            e = e.InnerException;
                        }

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
    }
}
