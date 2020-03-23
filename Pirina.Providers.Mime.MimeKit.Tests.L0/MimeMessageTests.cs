using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glasswall.Kernel.Extensions;
using Glasswall.Kernel.Mime;
using Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers;
using Moq;
using NUnit.Framework;

namespace Glasswall.Providers.Mime.MimeKit.Tests.L0
{
    [TestFixture]
    [Category("L0")]
    public class MimeMessageTests
    {
        [TestFixture]
        public class Constructor : MimeMessageTests
        {
            [Test]
            public void Can_Construct()
            {
                // Arrange
                var mimeMessage = new global::MimeKit.MimeMessage();

                // Act
                var result = new MimeMessage(mimeMessage);

                // Assert
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void Null_MimeMessage_Parameter_Throws_ArgumentNullException()
            {
                // Act
                MimeMessage TestDelegate() => new MimeMessage(null);

                // Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("mimeMessage"));
            }
        }

        [TestFixture]
        public class HeadersProperty : MimeMessageTests
        {
            [Test]
            public void Is_Correctly_Populated_After_Construction()
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMessage);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.Headers.ToList();

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(7));

                for (var i = 0; i < result.Count; i++)
                {
                    Assert.That(result[i].Field, Is.EqualTo(message.Headers[i].Field));
                    Assert.That(result[i].Value, Is.EqualTo(message.Headers[i].Value));
                }
            }

            [Test]
            public void Is_Correctly_Populated_If_Message_Has_No_Headers()
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMessage);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                message.Headers.Clear();
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.Headers.ToList();

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class IsEncryptedProperty : MimeMessageTests
        {
            [Test]
            public void Returns_True_If_Message_Is_Encrypted()
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(TestMessageType.EncryptedMessage);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.IsEncrypted;

                // Assert
                Assert.That(result, Is.True);
            }

            [Test]
            public void Returns_False_If_Message_Is_Not_Encrypted()
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMessage);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.IsEncrypted;

                // Assert
                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class BodyPartsProperty : MimeMessageTests
        {
            [TestCase(TestMessageType.MultiPartMessage, 2)]
            [TestCase(TestMessageType.MultiPartMessageNoAlternative, 1)]
            [TestCase(TestMessageType.MultiPartMessageRelated, 1)]
            [TestCase(TestMessageType.SinglePartMimeAttachment, 0)]
            [TestCase(TestMessageType.SinglePartMimeHtml, 1)]
            [TestCase(TestMessageType.SinglePartMimeText, 1)]
            [TestCase(TestMessageType.HeadersOnly, 0)]
            [TestCase(TestMessageType.BodyPartInAlternativeAndMixed, 2)]
            [TestCase(TestMessageType.MultiPartMixedWithinMultiPartRelated, 2)]
            [TestCase(TestMessageType.MultipleBodyPartsInAlternative, 4)]
            [TestCase(TestMessageType.AttachmentWithinAlternative, 2)]
            [TestCase(TestMessageType.InlineAttachmentWithinAlternative, 2)]
            [TestCase(TestMessageType.MultipleBodyPartsInMixed, 4)]
            [TestCase(TestMessageType.MultiPartAlternativeWithEmptyBodies, 0)]
            [TestCase(TestMessageType.MultiPartAlternativeWithBlankBodies, 2)]
            [TestCase(TestMessageType.MultiPartAttachmentOnlyMessage, 0)]
            [TestCase(TestMessageType.EncryptedMessage, 0)]
            public void Can_Return_All_Message_Body_Parts(TestMessageType testMessageType, int expectedBodyPartCount)
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(testMessageType);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.BodyParts;

                // Assert
                Assert.That(result.Count, Is.EqualTo(expectedBodyPartCount));
            }
        }

        [TestFixture]
        public class AttachmentsProperty : MimeMessageTests
        {
            [TestCase(TestMessageType.MultiPartMessage, 1)]
            [TestCase(TestMessageType.MultiPartMessageNoAlternative, 1)]
            [TestCase(TestMessageType.MultiPartMessageRelated, 1)]
            [TestCase(TestMessageType.SinglePartMimeAttachment, 1)]
            [TestCase(TestMessageType.SinglePartMimeHtml, 0)]
            [TestCase(TestMessageType.SinglePartMimeText, 0)]
            [TestCase(TestMessageType.HeadersOnly, 0)]
            [TestCase(TestMessageType.BodyPartInAlternativeAndMixed, 2)]
            [TestCase(TestMessageType.MultiPartMixedWithinMultiPartRelated, 3)]
            [TestCase(TestMessageType.MultipleBodyPartsInAlternative, 1)]
            [TestCase(TestMessageType.AttachmentWithinAlternative, 1)]
            [TestCase(TestMessageType.InlineAttachmentWithinAlternative, 1)]
            [TestCase(TestMessageType.MultipleBodyPartsInMixed, 1)]
            [TestCase(TestMessageType.MultiPartAlternativeWithEmptyBodies, 1)]
            [TestCase(TestMessageType.MultiPartAlternativeWithBlankBodies, 1)]
            [TestCase(TestMessageType.MultiPartAttachmentOnlyMessage, 1)]
            [TestCase(TestMessageType.EncryptedMessage, 1)]
            public void Can_Return_All_Message_Attachments(TestMessageType testMessageType, int expectedAttachmentCount)
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(testMessageType);
                var message = global::MimeKit.MimeMessage.Load(testMessage.ToStream(Encoding.UTF8));
                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.Attachments;

                // Assert
                Assert.That(result.Count, Is.EqualTo(expectedAttachmentCount));
            }
        }

        [TestFixture]
        public class SizeProperty : MimeMessageTests
        {
            [Test]
            public void Returns_Correct_Value()
            {
                // Arrange
                var testMessage = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMixedWithinMultiPartRelated);
                var stream = testMessage.ToStream(Encoding.UTF8);
                var message = global::MimeKit.MimeMessage.Load(stream);

                var mimeMessage = new MimeMessage(message);

                // MimeKit adds a carriage return to the end of the message when loading it
                // This results in an extra two bytes
                var expectedSize = stream.Length + 2;

                // Act
                var result = mimeMessage.Size;

                // Assert
                Assert.That(result, Is.EqualTo(expectedSize));
            }

            [Test]
            public void Returns_Correct_Value_For_An_Empty_Message()
            {
                // Arrange
                var message = new global::MimeKit.MimeMessage();
                message.Headers.Clear();

                var mimeMessage = new MimeMessage(message);

                // MimeKit adds a carriage return to the end of the message when loading it
                // This results in an extra two bytes
                var expectedSize = 0 + 2;

                // Act
                var result = mimeMessage.Size;

                // Assert
                Assert.That(result, Is.EqualTo(expectedSize));
            }
        }

        [TestFixture]
        public class ToStringMethod : MimeMessageTests
        {
            [Test]
            public void ToString_Returns_Correct_Value()
            {
                // Arrange
                var expectedMessage = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMixedWithinMultiPartRelated);
                var stream = expectedMessage.ToStream(Encoding.UTF8);
                var message = global::MimeKit.MimeMessage.Load(stream);

                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.ToString();

                // Assert
                // MimeKit adds a carriage return to the end of the message when loading it
                Assert.That(result, Is.EqualTo(expectedMessage + Environment.NewLine));
            }

            [Test]
            public void ToString_Returns_Correct_Value_For_An_Empty_Message()
            {
                // Arrange
                var expectedMessage = "";

                var message = new global::MimeKit.MimeMessage();
                message.Headers.Clear();

                var mimeMessage = new MimeMessage(message);

                // Act
                var result = mimeMessage.ToString();

                // Assert
                // MimeKit adds a carriage return to the end of the message when loading it
                Assert.That(result, Is.EqualTo(expectedMessage + Environment.NewLine));
            }
        }
    }
}
