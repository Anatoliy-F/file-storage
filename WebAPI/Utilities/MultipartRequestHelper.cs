using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace WebAPI.Utilities
{
    /// <summary>
    /// methods to process multipart request
    /// </summary>
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        /// <summary>
        /// Parsing boundary from header
        /// </summary>
        /// <param name="contentType"><see cref="MediaTypeHeaderValue"/></param>
        /// <param name="lengthlimit">Length limits</param>
        /// <returns>Boundary</returns>
        /// <exception cref="InvalidDataException">Throws if boundary length hegher than length limit or boundary value empty</exception>
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthlimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }
            if (boundary.Length > lengthlimit)
            {
                throw new InvalidDataException($"Multipart bounday length limit {lengthlimit} exceeded.");
            }
            return boundary;
        }

        /// <summary>
        /// Check is contentType "multipart/"
        /// </summary>
        /// <param name="contentType">Content-type of request</param>
        /// <returns>TRUE if contentType "multipart/", FALSE otherwise</returns>
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && 
                contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check content diposition
        /// </summary>
        /// <param name="contentDisposition"><see cref="ContentDispositionHeaderValue"/></param>
        /// <returns>TRUE if </returns>
        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        /// <summary>
        /// Check content diposition  for file data
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }

        /// <summary>
        /// Check mediaType encoding
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in most cases.
            if (!hasMediaTypeHeader || mediaType == null || mediaType.Encoding == null ||
                (mediaType.Encoding != null && Encoding.UTF7.Equals(mediaType.Encoding)))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding ?? Encoding.UTF8;
        }
    }
}
