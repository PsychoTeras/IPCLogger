using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;

namespace IPCLogger.Core.Loggers.LMail
{
    public sealed class LMail : QueueableLogger<LMailSettings>
    {

#region Private fields

        private SmtpClient _smtpClient;
        private MailAddress _mailAddressFrom;
        private MailAddress _mailAddressTo;
        private MailAddress _mailAddressCC;
        private MailAddress _mailAddressBcc;

        private List<MailData> _pendingMails;
        private HashSet<MailData> _mailsForDeleting;

#endregion

#region Properties

        protected override bool ShouldFlushQueue
        {
            get { return false; }
        }

#endregion

#region Ctor

        public LMail(bool threadSafetyIsGuaranteed)
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override bool InitializeQueue()
        {
            _pendingMails = new List<MailData>();
            _mailsForDeleting = new HashSet<MailData>();
            ConfigureSmtpClient();
            return true;
        }

        protected override bool DeinitializeQueue()
        {
            return true;
        }

        protected override void WriteQueue(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine)
        {
            _pendingMails.Add(new MailData(callerType, eventType, text));
        }

        protected override void FlushQueue()
        {
            if (_pendingMails.Count == 0) return;

            foreach (MailData mail in _pendingMails)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.Priority = Settings.Priority;
                mailMessage.From = _mailAddressFrom;
                mailMessage.To.Add(_mailAddressTo);
                if (_mailAddressCC != null)
                {
                    mailMessage.CC.Add(_mailAddressCC);
                }
                if (_mailAddressBcc != null)
                {
                    mailMessage.Bcc.Add(_mailAddressBcc);
                }

                mailMessage.Subject = SFactory.Process(Settings.Subject, Patterns);
                mailMessage.Body = mail.Message;

                _smtpClient.Send(mailMessage);

                _mailsForDeleting.Add(mail);
            }

            _pendingMails.RemoveAll(m => _mailsForDeleting.Contains(m));
        }

#endregion

#region Class methods

        private void ConfigureSmtpClient()
        {
            _smtpClient = new SmtpClient(Settings.SmtpServer, Settings.SmtpPort)
            {
                EnableSsl = Settings.UseSSL
            };
            if (!string.IsNullOrEmpty(Settings.UserName))
            {
                _smtpClient.Credentials = new NetworkCredential(Settings.UserName, Settings.Password);
            }
            else
            {
                _smtpClient.UseDefaultCredentials = true;
            }
            _mailAddressFrom = new MailAddress(Settings.From);
            _mailAddressTo = new MailAddress(Settings.To);
            _mailAddressCC = !string.IsNullOrEmpty(Settings.CC)
                ? new MailAddress(Settings.CC)
                : null;
            _mailAddressBcc = !string.IsNullOrEmpty(Settings.Bcc)
                ? new MailAddress(Settings.Bcc)
                : null;
        }

#endregion

    }
}
