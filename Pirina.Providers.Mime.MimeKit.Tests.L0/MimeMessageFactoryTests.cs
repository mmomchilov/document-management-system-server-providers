using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pirina.Kernel.Mime;
using NUnit.Framework;

namespace Pirina.Providers.Mime.MimeKit.Tests.L0
{
    [TestFixture]
    [Category("L0")]
    public class MimeMessageFactoryTests
    {
        [TestFixture]
        public class CreateInstanceMethod : MimeMessageFactoryTests
        {
            [Test]
            public void Can_Create_A_Message_With_No_Body_Parts_Or_Attachments()
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, Enumerable.Empty<MimeEntity>(), Enumerable.Empty<MimeEntity>()).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(0));
                Assert.That(result.Attachments.Count(), Is.EqualTo(0));
            }

            [Test]
            public void Can_Create_A_Message_With_Body_Parts_Only()
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var bodyParts = CreateBodyParts();

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, bodyParts, Enumerable.Empty<MimeEntity>()).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(0));
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(10)]
            [TestCase(100)]
            public void Can_Create_A_Message_With_Attachments_Only(int noOfAttachments)
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var attachments = CreateAttachments(noOfAttachments);

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, Enumerable.Empty<MimeEntity>(), attachments).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(0));
                Assert.That(result.Attachments.Count(), Is.EqualTo(noOfAttachments));
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(10)]
            [TestCase(100)]
            public void Can_Create_A_Message_With_Body_Parts_And_Attachments(int noOfAttachments)
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var bodyParts = CreateBodyParts();
                var attachments = CreateAttachments(noOfAttachments);

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, bodyParts, attachments).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(noOfAttachments));
            }

            [Test]
            public void Can_Create_A_Message_With_Multiple_Body_Parts()
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var bodyParts = new List<MimeEntity>();
                bodyParts.AddRange(CreateBodyParts());
                bodyParts.AddRange(CreateBodyParts());

                var attachments = CreateAttachments(1);

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, bodyParts, attachments).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(4));
                Assert.That(result.Attachments.Count(), Is.EqualTo(1));
            }

            [Test]
            public void Can_Create_A_Message_With_Attachment_With_Missing_Content_Encoding_Header()
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var bodyParts = CreateBodyParts();
                var attachments = new List<MimeEntity>()
                {
                    new MimeEntity(Enumerable.Empty<MimeHeader>(), CreateContentStream())
                };

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, bodyParts, attachments).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(1));
            }

            [Test]
            public void Can_Create_A_Message_With_Attachment_With_Invalid_Content_Encoding_Header()
            {
                // Arrange
                var mimeMessageFactory = new MimeMessageFactory();
                var messageHeaders = CreateMessageHeaders();

                var bodyParts = CreateBodyParts();
                var attachments = new List<MimeEntity>()
                {
                    new MimeEntity(new List<MimeHeader>()
                    {
                        new MimeHeader("Content-Transfer-Encoding", "foo")
                    },
                    CreateContentStream())
                };

                // Act
                var result = mimeMessageFactory.CreateInstance(messageHeaders, bodyParts, attachments).Result;

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers, Is.EquivalentTo(messageHeaders));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(1));
            }
        }

        private List<MimeHeader> CreateMessageHeaders()
        {
            return new List<MimeHeader>()
            {
                new MimeHeader("MIME-Version", "1.0"),
                new MimeHeader("From", "\"test.user\" <test.user@example.com>"),
                new MimeHeader("To", "recipient@test.com"),
                new MimeHeader("Subject", "A Test Message")
            };
        }

        private IEnumerable<MimeEntity> CreateBodyParts()
        {
            return new List<MimeEntity>
            {
                new MimeEntity(new List<MimeHeader>()
                    {
                        new MimeHeader("Content-Type", "text/plain; charset=utf-8; format=flowed"),
                        new MimeHeader("Content-Transfer-Encoding", "7bit")
                    },
                    CreateContentStream()),
                new MimeEntity(new List<MimeHeader>()
                    {
                        new MimeHeader("Content-Type", "text/html; charset=utf-8;"),
                        new MimeHeader("Content-Transfer-Encoding", "7bit")
                    },
                    CreateContentStream())
            };
        }

        private List<MimeEntity> CreateAttachments(int noOfAttachments)
        {
            var mimeEntities = new List<MimeEntity>();

            for (var i = 0; i < noOfAttachments; i++)
            {
                mimeEntities.Add(new MimeEntity(CreateAttachmentHeaders(i), CreateContentStream()));
            }

            return mimeEntities;
        }

        private IEnumerable<MimeHeader> CreateAttachmentHeaders(int fileNameSuffix)
        {
            return new List<MimeHeader>()
            {
                new MimeHeader("Content-Type", $"image/png; name=\"image{fileNameSuffix}.png\";"),
                new MimeHeader("Content-Transfer-Encoding", "base64"),
                new MimeHeader("Content-Disposition", "attachment; filename=\"image{fileNameSuffix}.png\""),
            };
        }

        private Stream CreateContentStream()
        {
            var helloWorldbytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            return new MemoryStream(helloWorldbytes);
        }

    }
}
