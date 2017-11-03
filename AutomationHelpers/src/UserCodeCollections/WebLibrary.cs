/*
 * Created by Ranorex
 * User: sknopper
 * Date: 9/21/2017
 * Time: 12:17 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// Collection of methods which helps to automate web and network based tasks.
    /// </summary>
    [UserCodeCollection]
    public class WebLibrary
    {
    	
    	/// <summary>
        /// Download a file and stores it locally.
        /// </summary>
        /// <param name="uri">The uri of the file to download</param>
        /// <param name="targetLocation">Local location where the file should be stored</param>
        /// <param name="overwrite">true / false if existing file should be overwritten</param>
    	[UserCodeMethod]
    	public static void DownloadFile(string uri, string localPath, string overwriteExisting = "true")
    	{
    		if (!File.Exists(localPath) || bool.Parse(overwriteExisting)){
    			using (WebClient client = new WebClient()) 
    			{
    				try
    				{
    					client.DownloadFile(uri, localPath);
    					string message = String.Format("File successfully downloaded to {0}", new FileInfo(localPath).FullName);
    					Report.Log(ReportLevel.Success, message);
    				} catch (Exception e)
    				{
    					string message = String.Format("Downloading a file from: {0} failed for the following reason: {1}", uri, e.Message);
    					Report.Log(ReportLevel.Failure, message);
    				}
    				
    			}
    		} else {
                string message = String.Format("The file {0} already exists in the local file, the download will be skipped", new FileInfo(localPath).FullName);
                Report.Log(ReportLevel.Debug, message);
            }
    	}
    }
}
