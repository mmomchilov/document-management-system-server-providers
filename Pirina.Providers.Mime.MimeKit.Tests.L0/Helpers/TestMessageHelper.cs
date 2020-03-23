using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers
{
    public static class TestMessageHelper
    {
        public static string GetTestMessage(TestMessageType testMessageType)
        {
            var testMessageMapper = new Dictionary<TestMessageType, string>
            {
                {
                    TestMessageType.MultiPartMessage,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartMessage.txt"
                },
                {
                    TestMessageType.MultiPartMessageNoAlternative,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartMessageNoAlternative.txt"
                },
                {
                    TestMessageType.MultiPartMessageRelated,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartMessageRelated.txt"
                },
                {
                    TestMessageType.SinglePartMimeAttachment,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.SinglePartMimeAttachment.txt"
                },
                {
                    TestMessageType.SinglePartMimeHtml,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.SinglePartMimeHtml.txt"
                },
                {
                    TestMessageType.SinglePartMimeText,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.SinglePartMimeText.txt"
                },
                {
                    TestMessageType.HeadersOnly,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.HeadersOnly.txt"
                },
                {
                    TestMessageType.BodyPartInAlternativeAndMixed,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.BodyPartInAlternativeAndMixed.txt"
                },
                {
                    TestMessageType.MultiPartMixedWithinMultiPartRelated,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartMixedWithinMultiPartRelated.txt"
                },
                {
                    TestMessageType.MultipleBodyPartsInAlternative,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultipleBodyPartsInAlternative.txt"
                },
                {
                    TestMessageType.AttachmentWithinAlternative,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.AttachmentWithinAlternative.txt"
                },
                {
                    TestMessageType.InlineAttachmentWithinAlternative,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.InlineAttachmentWithinAlternative.txt"
                },
                {
                    TestMessageType.MultipleBodyPartsInMixed,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultipleBodyPartsInMixed.txt"
                },
                {
                    TestMessageType.MultiPartAlternativeWithEmptyBodies,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartAlternativeWithEmptyBodies.txt"
                },
                {
                    TestMessageType.MultiPartAlternativeWithBlankBodies,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartAlternativeWithBlankBodies.txt"
                },
                {
                    TestMessageType.EncryptedMessage,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.EncryptedMessage.txt"
                },
                {
                    TestMessageType.MultiPartAttachmentOnlyMessage,
                    "Glasswall.Providers.Mime.MimeKit.Tests.L0.Helpers.MultiPartAttachmentOnlyMessage.txt"
                }
            };
            
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(testMessageMapper[testMessageType]);

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public enum TestMessageType
    {
        MultiPartMessage,
        MultiPartMessageNoAlternative,
        MultiPartMessageRelated,
        SinglePartMimeAttachment,
        SinglePartMimeHtml,
        SinglePartMimeText,
        HeadersOnly,
        BodyPartInAlternativeAndMixed,
        MultiPartMixedWithinMultiPartRelated,
        MultipleBodyPartsInAlternative,
        AttachmentWithinAlternative,
        InlineAttachmentWithinAlternative,
        MultipleBodyPartsInMixed,
        MultiPartAlternativeWithEmptyBodies,
        MultiPartAlternativeWithBlankBodies,
        MultiPartAttachmentOnlyMessage,
        EncryptedMessage
    }
}