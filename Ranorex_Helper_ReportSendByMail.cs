using System;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationSupplements
{

    public class Ranorex_Helper_ReportSendByMail
    {
        private string fromAddress;
        private string fromPassword;
        private string toAddress;
        private string subject;
        private string body;
        private int smtpPort;
        private string smtpHost;

        private System.Net.Mail.MailAddress _fromAddress;
        private System.Net.Mail.MailAddress _toAddress;
        private System.Net.Mail.MailMessage _message;
        private System.Net.Mail.SmtpClient _client;
        private System.Net.NetworkCredential _basicCredential;

        public Ranorex_Helper_ReportSendByMail(string FromAddress, string FromPassword, string ToAddress, string Subject, string Body, int SMTPPort, string SMTPHost)
        {
            fromAddress = FromAddress;
            fromPassword = FromPassword;
            toAddress = ToAddress;
            subject = Subject;
            body = Body;
            smtpHost = SMTPHost;
            smtpPort = SMTPPort;
        }

        public void Init()
        {
            _fromAddress = new System.Net.Mail.MailAddress(fromAddress);
            _toAddress = new System.Net.Mail.MailAddress(toAddress);
            _message = new System.Net.Mail.MailMessage(_fromAddress, _toAddress);
            _basicCredential = new System.Net.NetworkCredential(fromAddress, fromPassword);

            _message.Subject = subject;
            _message.Body = body;

            _client = new System.Net.Mail.SmtpClient();

            _client.Host = smtpHost;
            _client.Port = smtpPort; ;
            _client.EnableSsl = true;
            _client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            _client.UseDefaultCredentials = false;
            _client.Credentials = _basicCredential;
        }

        public void AttachFile(string filename)
        {
            string _file = filename;
            if (System.IO.File.Exists(_file))
            {
                System.Net.Mail.Attachment _attachement = new System.Net.Mail.Attachment(_file);
                _message.Attachments.Add(_attachement);
            }
        }
        public void AttachZippedReport()
        {
            string _filename = TestReport.ReportEnvironment.ReportName;
            string _directory = TestReport.ReportEnvironment.ReportFileDirectory;
            string _zippedFilename = _filename + ".rxzlog";

            AttachFile(_zippedFilename);
        }

        public void SendMail()
        {
            _client.Send(_message);
        }
    }
}