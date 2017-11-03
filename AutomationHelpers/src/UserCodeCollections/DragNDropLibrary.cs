/*
 * Created by Ranorex
 * User: sknopper
 * Date: 9/19/2017
 * Time: 12:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Drawing;

using Ranorex;

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
        /// Picks the source element, drags it to the target element and drops it there.
        /// <param name="source">The element which should be dragged.</param>
        /// <param name="target">The element where the source elemente will be dropped.</param>
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
