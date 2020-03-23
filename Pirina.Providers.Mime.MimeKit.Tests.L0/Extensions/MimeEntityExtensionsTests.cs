using System.IO;
using System.Linq;
using Pirina.Kernel.Mime;
using Pirina.Providers.Mime.MimeKit.Extensions;
using NUnit.Framework;

namespace Pirina.Providers.Mime.MimeKit.Tests.L0.Extensions
{
    [TestFixture]
    [Category("L0")]
    public class MimeEntityExtensionsTests
    {
        [TestFixture]
        public class GetFileNameMethod : MimeEntityExtensionsTests
        {
          [Test]
            public void Filename_From_Content_Disposition_Header_Is_Returned_If_Present()
            {
                // Arrange
                const string expectedFilename = "Clean.png";
                var headers = new[] {new MimeHeader("Content-Disposition", $"attachment; filename=\"{expectedFilename}\"")};
                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Filename_From_Content_Type_Header_Is_Returned_If_Present()
            {
                // Arrange
                const string expectedFilename = "contenttype.png";
                var headers = new[] {new MimeHeader("Content-Type", $"image/png; name=\"{expectedFilename}\"")};
                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Filename_From_Content_Disposition_Header_Is_Returned_If_Content_Type_Header_Has_No_Name_Parameter()
            {
                // Arrange
                const string expectedFilename = "contentdisposition.png";
                var headers = new[]
                {
                    new MimeHeader("Content-Disposition", $"attachment; filename=\"{expectedFilename}\""),
                    new MimeHeader("Content-Type", "image/png;"), 
                };

                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Filename_From_Content_Type_Header_Is_Returned_If_Content_Disposition_Header_Has_No_FileName_Parameter()
            {
                // Arrange
                const string expectedFilename = "contenttype.png";
                var headers = new[]
                    {
                        new MimeHeader("Content-Disposition", "attachment;"),
                        new MimeHeader("Content-Type", $"image/png; name=\"{expectedFilename}\""), 
                    };

                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Filename_From_Content_Dispositon_Header_Is_Returned_If_Both_Headers_Exist()
            {
                // Arrange
                const string expectedFilename = "contentdisposition.png";
                var headers = new[]
                {
                    new MimeHeader("Content-Disposition", $"attachment; filename=\"{expectedFilename}\""),
                    new MimeHeader("Content-Type", "image/png; name=\"contenttype.png\""), 
                };

                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Filename_From_Content_Type_Header_Is_Returned_If_Content_Disposition_Header_Is_Invalid()
            {
                // Arrange
                const string expectedFilename = "contenttype.png";
                var headers = new[]
                {
                    new MimeHeader("Content-Disposition", "somegarbage"),
                    new MimeHeader("Content-Type", $"image/png; name=\"{expectedFilename}\""), 
                };

                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.EqualTo(expectedFilename));
            }

            [Test]
            public void Returns_Null_If_Both_Headers_Are_Invalid()
            {
                // Arrange
                var headers = new[]
                {
                    new MimeHeader("Content-Disposition", "somegarbage"),
                    new MimeHeader("Content-Type", "moregarbage"), 
                };

                var mimeEntity = new MimeEntity(headers, Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.Null);
            }

            [Test]
            public void Returns_Null_If_No_Headers_Present()
            {
                // Arrange
                var mimeEntity = new MimeEntity(Enumerable.Empty<MimeHeader>(), Stream.Null);

                // Act
                var result = mimeEntity.GetFileName();

                // Assert
                Assert.That(result, Is.Null);
            }
        }
    }
}
