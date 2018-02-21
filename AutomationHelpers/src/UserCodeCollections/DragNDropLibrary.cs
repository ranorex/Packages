//
// Copyright © 2018 Ranorex All rights reserved
//

using System.Drawing;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection providing an advanced DragAndDrop method.
    /// </summary>
    [UserCodeCollection]
    public static class DragNDropLibrary
    {
        /// <summary>
        /// Picks the source element, drags it to the target element and drops it there.
        /// <param name="source">The element which should be dragged.</param>
        /// <param name="target">The element where the source element will be dropped.</param>
        /// </summary>
        [UserCodeMethod]
        public static void DragAndDrop(Adapter source, Adapter target)
        {
            Report.Info(string.Format("Drag from {0}", source));
            source.Focus();
            source.MoveTo();
            Mouse.ButtonDown(System.Windows.Forms.MouseButtons.Left);

            // fix issue if moving to an inactive window.
            Point currentPoint = Mouse.Position;
            currentPoint.X += 5;
            Mouse.MoveTo(currentPoint);

            Report.Info(string.Format("Drop at {0}", target));
            target.Focus();
            target.MoveTo();
            Mouse.ButtonUp(System.Windows.Forms.MouseButtons.Left);
        }
    }
}
