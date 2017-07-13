using System;
using System.Net;
using System.Net.Mail;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    ///     Used to send emails from a recording or module.
    /// </summary>
    [UserCodeCollection]
    public class EmailLibrary
    {
        /// <summary>
        ///     Send an email.
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email message</param>
        /// <param name="to">Email recipient</param>
        /// <param name="from">Email sender</param>
        /// <param name="serverHostname">Server hostname</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="useSSL">Defines whether SSL is used or not (true or false)</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        [UserCodeMethod]
        public static void SendMail(
            string subject,
            string body,
            string to,
            string from,
            string serverHostname,
            string serverPort,
            bool useSSL = false,
            string username = "",
            string password = "")
        {
            try
            {
                var smtp = new SmtpClient(serverHostname, int.Parse(serverPort))
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = useSSL
                };

                smtp.Send(from, to, subject, body);

                Report.Success("Email has been sent to '" + to + "'.");
            }
            catch (Exception ex)
            {
                Report.Failure("Mail Error: " + ex);
            }
        }

        /// <summary>
        ///     Send current zipped pdf report via email.
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email message</param>
        /// <param name="to">Email recipient</param>
        /// <param name="from">Email sender</param>
        /// <param name="serverHostname">Server hostname</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="useSSL">Defines whether SSL is used or not (true or false)</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        [UserCodeMethod]
        public static void SendReportViaMail(
            string subject,
            string body,
            string to,
            string from,
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
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body
                };

                AddZippedReportToMessageAttachments(message);

                client.Send(message);

                Report.Success("Report has been sent to '" + to + "' via email.");
            }
            catch (Exception ex)
            {
                Report.Failure("Mail Error: " + ex);
            }
        }

        private static void AddZippedReportToMessageAttachments(MailMessage message)
        {
            string zippedFilename = TestReport.ReportEnvironment.ReportName + ".rxzlog";

            if (System.IO.File.Exists(zippedFilename))
            {
                System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(zippedFilename);
                message.Attachments.Add(attachement);
            }
            else
            {
                Report.Warn($"The zipped report '{zippedFilename}' does not exist. Please make sure the path is correct.");
            }
        }
    }
}