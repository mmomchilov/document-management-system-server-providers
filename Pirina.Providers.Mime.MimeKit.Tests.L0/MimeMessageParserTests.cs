using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Glasswall.Kernel.Extensions;
using Glasswall.Kernel.Mime;
using Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers;
using NUnit.Framework;

namespace Glasswall.Providers.Mime.MimeKit.Tests.L0
{
    [TestFixture]
    [Category("L0")]
    public class MimeMessageParserTests
    {
        [TestFixture]
        public class ParseMimeMessageMethod : MimeMessageParserTests
        {
            [Test]
            public void Can_Parse_Mime_Message_From_String()
            {
                // Arrange
                var message = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMessage);
                var mimeMessageParser = new MimeMessageParser();
                
                // Act
                var result = mimeMessageParser.ParseMimeMessage(message);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers.Count, Is.EqualTo(7));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(1));
                Assert.That(result.IsEncrypted, Is.False);
                Assert.That(result.Size, Is.EqualTo(1712));
            }

            [Test]
            public void Throws_Argument_Null_Exception_If_String_Content_Parameter_Is_Null()
            {
                // Arrange
                string message = null;
                var mimeMessageParser = new MimeMessageParser();
                
                // Act
                IMimeMessage TestDelegate() => mimeMessageParser.ParseMimeMessage(message);

                // Assert
                Assert.That(TestDelegate, 
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("content"));
            }

            [Test]
            public void Can_Parse_Mime_Message_From_Stream()
            {
                // Arrange
                var message = TestMessageHelper.GetTestMessage(TestMessageType.MultiPartMessage);
                var stream = message.ToStream(Encoding.UTF8);

                var mimeMessageParser = new MimeMessageParser();
                
                // Act
                var result = mimeMessageParser.ParseMimeMessage(stream);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Headers.Count, Is.EqualTo(7));
                Assert.That(result.BodyParts.Count(), Is.EqualTo(2));
                Assert.That(result.Attachments.Count(), Is.EqualTo(1));
                Assert.That(result.IsEncrypted, Is.False);
                Assert.That(result.Size, Is.EqualTo(1712));
            }

            [Test]
            public void Throws_Argument_Null_Exception_If_Stream_Content_Parameter_Is_Null()
            {
                // Arrange
                Stream message = null;
                var mimeMessageParser = new MimeMessageParser();
                
                // Act
                IMimeMessage TestDelegate() => mimeMessageParser.ParseMimeMessage(message);

                // Assert
                Assert.That(TestDelegate, 
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("content"));
            }
        }
    }
}
