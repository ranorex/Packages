//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using System.IO;
using System.Net;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// Collection of methods which helps to automate web and network based tasks.
    /// </summary>
    [UserCodeCollection]
    public static class WebLibrary
    {
        /// <summary>
        /// Download a file and stores it locally.
        /// </summary>
        /// <param name="uri">The uri of the file to download</param>
        /// <param name="targetLocation">Local location where the file should be stored</param>
        /// <param name="overwrite">true / false if existing file should be overwritten</param>
        [UserCodeMethod]
        public static void DownloadFile(
            string uri, string localPath, string overwriteExisting = "true")
        {
            if (!File.Exists(localPath) || bool.Parse(overwriteExisting))
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
                            "Downloading a file from: {0} failed for the following reason: {1}",
                            uri,
                            e.Message);
                        Report.Log(ReportLevel.Failure, message);
                    }
                }
            }
            else
            {
                string message = string.Format(
                    "The file {0} already exists in the local file, the download will be skipped",
                    new FileInfo(localPath).FullName);
                Report.Log(ReportLevel.Debug, message);
            }
        }
    }
}
