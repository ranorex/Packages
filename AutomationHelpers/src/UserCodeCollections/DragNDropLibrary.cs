/*
 * Created by Ranorex
 * User: sknopper
 * Date: 9/19/2017
 * Time: 12:22 PM
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

namespace CheckAutomationHelperIntegration.UserCodeCollections
{
    /// <summary>
    /// Ranorex User Code collection. A collection is used to publish User Code methods to the User Code library.
    /// </summary>
    [UserCodeCollection]
    public class DragNDropLibrary
    {
    	
    	/// <summary>
    	/// This is a placeholder text. Please describe the purpose of the
    	/// user code method here. The method is published to the User Code library
    	/// within a User Code collection.
    	/// </summary>
    	[UserCodeMethod]
    	public static void DragAndDrop(Adapter source, Adapter target)
    	{
            source.Focus();
            source.MoveTo();
            Mouse.ButtonDown(System.Windows.Forms.MouseButtons.Left);

            // fix issue if moving to an inactive window.
            Point currentPoint = Mouse.Position;
            currentPoint.X += 5;
            Mouse.MoveTo(currentPoint);

            target.Focus();
            target.MoveTo();
            Mouse.ButtonUp(System.Windows.Forms.MouseButtons.Left);
        }
    }
}
