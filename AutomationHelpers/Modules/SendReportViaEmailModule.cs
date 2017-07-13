// TODO: Add Header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Ranorex.AutomationHelpers.UserCodeCollections;
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
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SendReportViaEmailModule()
            : base()
        {
            // Do not delete - a parameterless constructor is required!
        }

        public new void Run()
        {
            EmailLibrary.SendReportViaMail(
                subject: this.Subject,
                body: this.Body,
                to: this.To,
                from: this.From,
                serverHostname: this.ServerHostname,
                serverPort: int.Parse(this.ServerPort),
                useSSL: bool.Parse(this.UseSSL),
                username: this.Username,
                password: this.Password);
        }
    }
}
