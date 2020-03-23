using System.Text;
using Pirina.Kernel.Extensions;
using Pirina.Kernel.Mime;
using MimeKit;

namespace Pirina.Providers.Mime.MimeKit.Extensions
{
    public static class MimeKitExtensions
    {
        public static MailboxAddress ToMailboxAddress(this Mailbox mailbox)
        {
            return new MailboxAddress(mailbox.DisplayName, mailbox.AsAddress());
        }

        public static Mailbox ToMailbox(this MailboxAddress mailboxAddress)
        {
            var mailbox = new Mailbox(mailboxAddress.Address);
            
            if (!string.IsNullOrEmpty(mailboxAddress.Name))
            {
                mailbox.DisplayName = mailboxAddress.Name;
            }

            return mailbox;
        }
    }
}