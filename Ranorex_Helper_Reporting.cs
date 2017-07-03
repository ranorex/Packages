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
    /// Description of UserCodeModule1.
    /// </summary>
    [TestModule("385FA58E-94E7-4068-BC38-36B1D8B5C4BF", ModuleType.UserCode, 1), UserCodeCollection]
    public class Ranorex_Helper_Reporting : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Ranorex_Helper_Reporting()
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
		/// Creates a Pdf version of the report. 
		/// </summary>
        [UserCodeMethod]
		public static void Report2Pdf() {
		}

       	/// <summary>
		/// Sends an email to recipient and adds the generated report (rxlog file) as an attachement to it. 
		/// </summary>
        [UserCodeMethod]
		public static void SendReport() {
		}

       	/// <summary>
		/// Logs some system information to the report 
		/// </summary>
        [UserCodeMethod]
		public static void ReportSystemInformation() {
		}

        [UserCodeMethod]
		public static void CreateLogFile() {
		}
        
        [UserCodeMethod]
        public static void sendEmailWhenDone() {
        	
        }
    }
}
