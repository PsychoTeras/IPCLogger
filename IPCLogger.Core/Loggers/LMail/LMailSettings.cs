using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Net.Mail;

namespace IPCLogger.Core.Loggers.LMail
{
    public sealed class LMailSettings : QueueableSettings
    {

#region Constants

        private const int SMTP_PORT = 25;
        private const MailPriority MAIL_PRIORITY = MailPriority.Normal;

#endregion

#region Defaults

        protected override int DefQueueSize { get { return 10; } }

#endregion

#region Properties

        [RequiredSetting]
        public string SmtpServer { get; set; }

        [RequiredSetting]
        public int SmtpPort { get; set; }

        [RequiredSetting]
        public string UserName { get; set; }

        [RequiredSetting]
        public string Password { get; set; }

        public bool UseSSL { get; set; }

        [RequiredSetting]
        public string From { get; set; }

        [RequiredSetting]
        public string To { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public MailPriority Priority { get; set; }

#endregion

#region Ctor

        public LMailSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges)
        {
            SmtpPort = SMTP_PORT;
            Priority = MAIL_PRIORITY;
        }

#endregion

    }
}