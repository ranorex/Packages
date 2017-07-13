// TODO: Add Header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.Modules
{
    /// <summary>
    ///     Used to send a report via email.
    /// </summary>
    [TestModule("bdd84fdb-489b-4f45-a31e-504ddeee15bf", ModuleType.Recording, 1)]
    public class SendReportViaEmailModule : EmailModule, ITestModule
    {

        public SendReportViaEmailModule()
            : base()
        {

        }

        public new void Run()
        {
            try
            {
                var client = new SmtpClient(this.ServerHostname, int.Parse(this.ServerPort))
                {
                    Credentials = new NetworkCredential(this.Username, this.Password),
                    EnableSsl = bool.Parse(this.UseSSL),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                var message = new MailMessage(this.From, this.To)
                {
                    Subject = this.Subject,
                    Body = this.Body
                };

                AddZippedReportToMessageAttachments(message);

                client.Send(message);

                Report.Success("Report has been sent to '" + this.To + "' via email.");
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
