using Ranorex.AutomationHelpers.Modules;
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
        /// <param name="message">Email message</param>
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
            string message,
            string to,
            string from,
            string serverHostname,
            string serverPort,
            bool useSSL = false,
            string username = "",
            string password = "")
        {
            var SM = new EmailModule
            {
                From = from,
                Message = message,
                Password = password,
                ServerHostname = serverHostname,
                ServerPort = serverPort,
                Subject = subject,
                To = to,
                UseSSL = useSSL.ToString(),
                Username = username
            };
            SM.Run();
        }
    }
}