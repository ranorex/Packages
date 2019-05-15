//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// Used to send emails from a recording or module.
    /// </summary>
    [UserCodeCollection]
    public static class EmailLibrary
    {
        private const string libraryName = "EmailLibrary";

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="to">Email recipients, separated by comma or semicolon</param>
        /// <param name="from">Email sender</param>
        /// <param name="body">Email message</param>
        /// <param name="attachment">Path of a file to attach</param>
        /// <param name="serverHostname">Server hostname</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="useSSL">Defines whether SSL is used or not (true or false)</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        [UserCodeMethod]
        public static bool SendEmail(
            string subject,
            string to,
            string from,
            string body,
            string attachment,
            string serverHostname,
            int serverPort,
            bool useSSL = false,
            string username = "",
            string password = "")
        {
            return EmailLibrary.SendEmail(subject, to, from, body, new string[] { attachment }, serverHostname, serverPort, useSSL, username, password);
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="to">Email recipients, separated by comma or semicolon</param>
        /// <param name="from">Email sender</param>
        /// <param name="body">Email message</param>
        /// <param name="attachments">Paths of files to attach</param>
        /// <param name="serverHostname">Server hostname</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="useSSL">Defines whether SSL is used or not (true or false)</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        [UserCodeMethod]
            public static bool SendEmail(
                string subject,
                string to,
                string from,
                string body,
                string[] attachments,
                string serverHostname,
                int serverPort,
                bool useSSL = false,
                string username = "",
                string password = "")
            {
                try
                {
                    var client = new SmtpClient(serverHostname, serverPort)
                    {
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = useSSL,
                    };

                    to = to.Replace(";", ",");
                    var message = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = body
                    };

                    if (attachments != null)
                    {

                        foreach (string attachment in attachments)
                        {
                            if (File.Exists(attachment))
                            {
                                message.Attachments.Add(new Attachment(attachment));
                            }
                            else if (!string.Empty.Equals(attachment))
                            {
                                Report.Warn(string.Format("The file '{0}' does not exist. Please make sure the path '{1}' is correct.",
                                    attachment, TestReport.ReportEnvironment));
                            }
                        }
                    }

                    client.Send(message);

                    Report.Success(string.Format("Email has been sent to '{0}'", to));
                    return true;
                }
                catch (Exception ex)
                {
                    Utils.ReportException(ex, libraryName);
                    return false;
                }
            }
        }
}
