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

namespace Ranorex.AutomationHelpers
{
    /// <summary>
    /// Description of Ranorex_Helper_Image.
    /// </summary>
    [TestModule("687F1AD2-C912-4A78-8047-BFA9BB799B3E", ModuleType.UserCode, 1)]
    public class Ranorex_Helper_Image : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Ranorex_Helper_Image()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;
        }
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void compareImages() {
		}
        
        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        [UserCodeMethod]
		public static void imageContains() {
		}
        
    }
}
