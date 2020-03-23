using System;
using System.IO;
using System.Text;
using Pirina.Kernel.Extensions;
using Pirina.Kernel.Mime;

namespace Pirina.Providers.Mime.MimeKit
{
    public class MimeMessageParser : IMimeMessageParser
    {
        public IMimeMessage ParseMimeMessage(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            using (var contentStream = content.ToStream(Encoding.UTF8))
            {
                return ParseMimeMessage(contentStream);
            }
        }

        public IMimeMessage ParseMimeMessage(Stream content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var parsedMimeMessage = global::MimeKit.MimeMessage.Load(content);
            return CreateMimeMessage(parsedMimeMessage);
        }

        private IMimeMessage CreateMimeMessage(global::MimeKit.MimeMessage parsedMimeMessage)
        {
            return new MimeMessage(parsedMimeMessage);
        }
    }
}
