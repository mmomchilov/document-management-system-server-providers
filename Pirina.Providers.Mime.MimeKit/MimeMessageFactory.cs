using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Pirina.Kernel.Mime;
using MimeKit;
using MimeEntity = Pirina.Kernel.Mime.MimeEntity;

namespace Pirina.Providers.Mime.MimeKit
{
    public class MimeMessageFactory : IMimeMessageFactory
    {
        private readonly Dictionary<string, ContentEncoding> _validContentEncodings =
            new Dictionary<string, ContentEncoding>()
            {
                {"7bit", ContentEncoding.SevenBit},
                {"8bit", ContentEncoding.EightBit},
                {"quoted-printable", ContentEncoding.QuotedPrintable},
                {"base64", ContentEncoding.Base64},
                {"binary", ContentEncoding.Binary}
            };

        public Task<IMimeMessage> CreateInstance(IEnumerable<MimeHeader> headers,
            IEnumerable<MimeEntity> bodyParts,
            IEnumerable<MimeEntity> attachments)
        {
            var message = new global::MimeKit.MimeMessage();
            message.Headers.Clear();

            foreach (var header in headers)
                message.Headers.Add(header.Field, header.Value);

            var multipartMixed = new Multipart("mixed");
            var multipartAlternative = new MultipartAlternative();

            foreach (var bodyPart in bodyParts)
                multipartAlternative.Add(CreateMimePart<TextPart>(bodyPart));

            multipartMixed.Add(multipartAlternative);
        
            foreach (var attachment in attachments)
                multipartMixed.Add(CreateMimePart<MimePart>(attachment)); 
            
            message.Body = multipartMixed;

            return Task.FromResult((IMimeMessage)new MimeMessage(message));
        }

        private T CreateMimePart<T>(MimeEntity mimeEntity)
            where T : global::MimeKit.MimePart, new()
        {
            var mimePart = new T()
            {
                Content = new MimeContent(mimeEntity.Content, GetContentEncoding(mimeEntity.Headers))
            };

            mimePart.Headers.Clear();

            foreach (var header in mimeEntity.Headers)
                mimePart.Headers.Add(header.Field, header.Value);

            return mimePart;
        }

        private ContentEncoding GetContentEncoding(IEnumerable<MimeHeader> headers)
        {
            var contentEncodingHeader = headers.FirstOrDefault(x => x.Field == "Content-Transfer-Encoding");

            if (contentEncodingHeader == null || string.IsNullOrEmpty(contentEncodingHeader.Value))
                return ContentEncoding.Default;

            foreach (var encoding in _validContentEncodings)
            {
                if (!contentEncodingHeader.Value.Equals(encoding.Key, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                return encoding.Value;
            }

            return ContentEncoding.Default;
        }
    }
}
