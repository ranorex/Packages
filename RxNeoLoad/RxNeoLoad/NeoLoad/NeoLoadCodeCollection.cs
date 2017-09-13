/*
 * Created by Ranorex
 * User: phaiden
 * Date: 06.07.2017
 * Time: 16:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
using Ranorex.Core.Repository;

namespace Ranorex.NeoLoad
{
    /// <summary>
    /// Ranorex User Code collection. A collection is used to publish User Code methods to the User Code library.
    /// </summary>
    [UserCodeCollection]
    public class NeoLoadCodeCollection
    {
    	// You c
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void SendTimingValues(Ranorex.Core.Repository.RepoItemInfo domNode, string transactionName)
    	{
    		var api = NeoloadApi.Instance;
    		var doc = domNode.CreateAdapter<WebDocument>(true);
    		var timings = NavigationTimingWrapper.ReadTimingValuesFromPage(doc);
    		api.SendNavTiming(
    			timings, 
    			transactionName, 
    			doc.PageUrl,
    			string.Format("{0} - {1}", doc.BrowserName, doc.BrowserVersion),
    			TestSuite.CurrentTestContainer.Name);
    	}
    }
}
