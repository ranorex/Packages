using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Net.Mail;
using System.Net;
using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers
{
    /// <summary>
    /// Description of RanorexEmailHelper.
    /// </summary>
    [TestModule("D8198CC7-82F5-46B2-81E7-3ED789544877", ModuleType.UserCode, 1), UserCodeCollection]
    public class Ranorex_Helper_Email : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Ranorex_Helper_Email()
        {
            // Do not delete - a parameterless constructor is required!
        }

		#region Variables

		public static string From = "";
		public static string ServerHostname = "";
		public static string ServerPort = "";
		public static string Message = "";
		public static bool SSL = true;
		public static string Username = "";
		public static string Password = "";
		
        
		#endregion
        
        		
		[UserCodeMethod]
		public static void SendMail(string Subject, string Message, string To, string From, string ServerHostname, string ServerPort, bool SSL = false, string Username = "", string Password = "")
		{			
			SetServerConfig(From, ServerHostname, ServerPort, SSL, Username, Password);
			
			Ranorex_Helper_Email SM = new Ranorex_Helper_Email();
			SM.DoSendMail(Subject, Message, To);
		}
		
		/// <summary>
		/// Sets the mail server config.
		/// </summary>
		/// <param name="From">Sender email</param>
		/// <param name="ServerHostname">Server hostname</param>
		/// <param name="ServerPort">Server port</param>
		/// <param name="SSL">Defines whether SSL is used or not (true or false)</param>
		/// <param name="Username">Username</param>
		/// <param name="Password">Password</param>
		[UserCodeMethod]
		public static void SetServerConfig(string From, string ServerHostname, string ServerPort, bool SSL = false, string Username = "", string Password = "")
		{
				
			Ranorex_Helper_Email.From = From;
			Ranorex_Helper_Email.ServerHostname = ServerHostname;
			Ranorex_Helper_Email.ServerPort = ServerPort;
			Ranorex_Helper_Email.SSL = SSL;
			Ranorex_Helper_Email.Username = Username;
			Ranorex_Helper_Email.Password = Password;			
		}
		
		/// <summary>
		/// Sends an email.
		/// </summary>
		/// <param name="Subject">Email subject</param>
		/// <param name="Message">Email message</param>
		/// <param name="To">Email recipient</param>
		[UserCodeMethod]
		public static void SendEmail(string Subject, string Message, string To)
		{		
			Ranorex_Helper_Email SM = new Ranorex_Helper_Email();
			SM.DoSendMail(Subject, Message, To);
		}
		
		
		/// <summary>
		/// Sends an email if the test suite finishes with success.
		/// </summary>
		/// <param name="Subject">Email subject</param>
		/// <param name="Message">Email message</param>
		/// <param name="To">Email recipient</param>
		[UserCodeMethod]
		public static void SendResultOnSuccess(string Subject, string Message, string To)
		{
			// Delegate must be registered only once
			// Email will be sent at the very end of the testsuite
			TestSuite.TestSuiteCompleted += delegate {
				if (Ranorex.Core.Reporting.TestReport.CurrentTestSuiteActivity.Status == ActivityStatus.Success)
				{
					SendEmail(Subject, Message, To);
				}
			};
		}

		/// <summary>
		/// Sends an email if the test suite finishes with a failure.
		/// </summary>
		/// <param name="Subject">Email subject</param>
		/// <param name="Message">Email message</param>
		/// <param name="To">Email recipient</param>
		[UserCodeMethod]
		public static void SendResultOnFailure(string Subject, string Message, string To)			
		{
			// Delegate must be registered only once
			// Email will be sent at the very end of the testsuite
			TestSuite.TestSuiteCompleted += delegate {
				if (Ranorex.Core.Reporting.TestReport.CurrentTestSuiteActivity.Status == ActivityStatus.Failed)
				{
					SendEmail(Subject, Message, To);
				}
			};
		}
		
		public void DoSendMail(string Subject, string Message, string To)			
		{
			try
			{
				SmtpClient smtp = new SmtpClient(ServerHostname, int.Parse(ServerPort))
            	{
	                Credentials = new NetworkCredential(Username, Password),
	                EnableSsl = true
            	};
				
				smtp.Send(From, To, Subject, Message);
				
				Ranorex.Report.Success("Email has been sent to '" + To + "'.");
			}
			catch(Exception ex)
			{
				Ranorex.Report.Failure("Mail Error: " + ex.ToString());
			}
			
		}
    }
}
