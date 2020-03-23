using System;
using System.Collections.Generic;
using System.Text;
using Pirina.Kernel.Mime;
using Pirina.Providers.Mime.MimeKit.Extensions;
using MimeKit;
using NUnit.Framework;

namespace Pirina.Providers.Mime.MimeKit.Tests.L0.Extensions
{
    [TestFixture]
    [Category("L0")]
    public class MimeKitExtensionsTests
    {
        [TestFixture]
        public class ToMailboxAddressMethod
        {
            [Test]
            public void Returns_A_MailboxAddress()
            {
                // Arrange
                var mailbox = new Mailbox("hello@world.com") {DisplayName = "Hello"};

                // Act
                var result = mailbox.ToMailboxAddress();

                // Assert
                Assert.That(result, Is.TypeOf<MailboxAddress>());
                Assert.That(result.Address, Is.EqualTo(mailbox.AsAddress()));
                Assert.That(result.Name, Is.EqualTo(mailbox.DisplayName));
            }
        }

        [TestFixture]
        public class ToMailboxMethod
        {
            [Test]
            public void Returns_A_Mailbox()
            {
                // Arrange
                var mailboxAddress = new MailboxAddress("Hello", "hello@world.com");
                
                // Act
                var result = mailboxAddress.ToMailbox();

                // Assert
                Assert.That(result, Is.TypeOf<Mailbox>());
                Assert.That(result.AsAddress(), Is.EqualTo(mailboxAddress.Address));
                Assert.That(result.DisplayName, Is.EqualTo(mailboxAddress.Name));
            }
        }
    }
}
