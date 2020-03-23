using System;
using System.Linq;
using MimeKit;
using MimeEntity = Pirina.Kernel.Mime.MimeEntity;

namespace Pirina.Providers.Mime.MimeKit.Extensions
{
    public static class MimeEntityExtensions
    {
        public static string GetFileName(this MimeEntity mimeEntity)
        {
            var contentDispositionHeader = mimeEntity.Headers.FirstOrDefault(x =>
                x.Field.Equals("Content-Disposition", StringComparison.InvariantCultureIgnoreCase));

            if (contentDispositionHeader != null &&
                ContentDisposition.TryParse(contentDispositionHeader.Value, out var contentDisposition))
            {
                if(!string.IsNullOrEmpty(contentDisposition.FileName))
                    return contentDisposition.FileName;
            }

            var contentTypeHeader = mimeEntity.Headers.FirstOrDefault(x =>
                x.Field.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase));

            if (contentTypeHeader == null) return null;

            if (!ContentType.TryParse(contentTypeHeader.Value, out var contentType)) 
                return null;

            return !string.IsNullOrEmpty(contentType.Name) 
                ? contentType.Name 
                : null;
        }
    }
}
