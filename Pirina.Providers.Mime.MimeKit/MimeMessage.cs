using System;
using System.Collections.Generic;
using System.Linq;
using Pirina.Kernel.Extensions;
using Pirina.Kernel.Mime;
using MimeKit;
using MimeKit.Cryptography;
using MimeKit.IO;
using MimeEntity = Pirina.Kernel.Mime.MimeEntity;

namespace Pirina.Providers.Mime.MimeKit
{
    public class MimeMessage : IMimeMessage
    {
        private readonly global::MimeKit.MimeMessage _mimeMessage;

        internal MimeMessage(global::MimeKit.MimeMessage mimeMessage)
        {
            _mimeMessage = mimeMessage ?? throw new ArgumentNullException(nameof(mimeMessage));
            Headers = _mimeMessage.Headers.Select(x => new MimeHeader(x.Field, x.Value));
            BodyParts = GetBodyParts();
            Attachments = GetAttachments();
            Size = CalculateMessageSize();
            IsEncrypted = CheckIfEncrypted();
        }

        public IEnumerable<MimeHeader> Headers { get; }

        public IEnumerable<MimeEntity> BodyParts { get; }

        public IEnumerable<MimeEntity> Attachments { get; }

        public bool IsEncrypted { get; }

        public long Size { get; }

        public override string ToString()
        {
            return _mimeMessage == null ? string.Empty : _mimeMessage.ToString();
        }

        private IEnumerable<MimeHeader> GetHeaders(global::MimeKit.MimeEntity mimeEntity)
        {
            return mimeEntity.Headers.Select(h => new MimeHeader(h.Field, h.Value));
        }

        private IEnumerable<MimeEntity> GetBodyParts()
        {
            if (TryFindFirstMultipart<MultipartAlternative>(_mimeMessage.Body, out var firstMultiPartAlternative))
                return GetTextParts(firstMultiPartAlternative);

            if (TryFindFirstMultipart<MultipartRelated>(_mimeMessage.Body, out var firstMultiPartRelated))
                return GetTextParts(firstMultiPartRelated);

            return _mimeMessage.BodyParts.OfType<TextPart>()
                .Where(x => !x.IsAttachment && x.Content?.Stream != null)
                .Select(x => new MimeEntity(GetHeaders(x), x.Content.Stream));
        }

        private IEnumerable<MimeEntity> GetAttachments()
        {
           return _mimeMessage.BodyParts.OfType<MimePart>()
                .Where(x => x.Content?.Stream != null)
                .Select(x => new MimeEntity(GetHeaders(x), x.Content.Stream))
                .Where(x => !BodyParts.Contains(x));
        }

        private long CalculateMessageSize()
        {
            using (var stream = new MeasuringStream())
            {
                _mimeMessage.WriteTo(stream);
                return stream.Length;
            }
        }

        private bool CheckIfEncrypted()
        {
            return _mimeMessage.BodyParts.Any(x => x is ApplicationPkcs7Mime)
                || _mimeMessage.BodyParts.Any(x => x is ApplicationPgpEncrypted);
        }

        private bool TryFindFirstMultipart<T>(global::MimeKit.MimeEntity entity, out Multipart bodyMultipart)
            where T : Multipart
        {
            switch (entity)
            {
                case null:
                    bodyMultipart = null;
                    return false;

                case Multipart multipart when multipart.GetType() == typeof(T):
                    bodyMultipart = multipart;
                    return true;

                case Multipart multipart:
                    foreach (var subpart in multipart)
                        return TryFindFirstMultipart<T>(subpart, out bodyMultipart);
                    break;
            }

            bodyMultipart = null;
            return false;
        }

        private IEnumerable<MimeEntity> GetTextParts(Multipart multipart)
        {
            return multipart.OfType<TextPart>()
                .Where(x => !x.IsAttachment && x.Content?.Stream != null)
                .Select(x => new MimeEntity(GetHeaders(x), x.Content.Stream));
        }
    }
}