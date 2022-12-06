using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace WebAPI.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
       /// <summary>
       /// 
       /// </summary>
       /// <param name="contentType"></param>
       /// <param name="lengthlimit"></param>
       /// <returns></returns>
       /// <exception cref="InvalidDataException"></exception>
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
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && 
                contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        /// <summary>
        /// 
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
        /// 
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
