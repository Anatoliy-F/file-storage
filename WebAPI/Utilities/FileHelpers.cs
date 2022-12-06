﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;

namespace WebAPI.Utilities
{
    /// <summary>
    /// Validate file
    /// </summary>
    public static class FileHelpers
    {
        // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
        // and the official specifications for the file types you wish to add.
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new()
        {
             { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
             { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
             { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
             },
             { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
             },
             { ".zip", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                    new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                    new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
                }
            },
            { ".txt", new List<byte[]>
                {
                    new byte[]{ 0xEF, 0xBB, 0xBF },
                    new byte[]{ 0xFF, 0xEF },
                    new byte[]{ 0xFE, 0xFF },
                    new byte[]{ 0xFF, 0xFE, 0x00, 0x00 },
                    new byte[]{ 0x00, 0x00, 0xFE, 0xFF },
                    new byte[]{ 0x0E, 0xFE, 0xFF }
                }
            },
            { ".pdf", new List<byte[]>
                {
                    new byte[]{ 0x25, 0x50, 0x44, 0x46, 0x2D}
                }
            }
        };


        // **WARNING!**
        // In the following file processing methods, the file's content isn't scanned.
        // In most production scenarios, an anti-virus/anti-malware scanner API is
        // used on the file before making the file available to users or other
        // systems. For more information, see the topic that accompanies this sample
        // app.
        /// <summary>
        /// Not used in this implimentation((
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formFile"></param>
        /// <param name="modelState"></param>
        /// <param name="permittedExtensions"></param>
        /// <param name="sizeLimit"></param>
        /// <returns></returns>
        public static async Task<byte[]> ProcessFormFile<T>(IFormFile formFile,
            ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
        {
            var fieldDisplayName = string.Empty;

            // Use reflection to obtain the display name for the model
            // property associated with this IFormFile. If a display
            // name isn't found, error messages simply won't show
            // a display name.

            MemberInfo? property = typeof(T).GetProperty(
                formFile.Name[(formFile.Name.IndexOf(".", StringComparison.Ordinal) + 1)..]);

            if (property != null && (property.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute))
            {
                fieldDisplayName = $"{displayAttribute.Name}";
            }

            // Don't trust the file name sent by the client. To display
            // the file name, HTML-encode the value.
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(formFile.Name);

            // Check the file length. This check doesn't catch files that only have 
            // a BOM as their content.
            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.Name, $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty");

                return Array.Empty<byte>();
            }

            if (formFile.Length > sizeLimit)
            {
                var megabyteSizeLimit = sizeLimit / 1048576;
                modelState.AddModelError(formFile.Name,
                    $"{fieldDisplayName}({trustedFileNameForDisplay}) exceeds {megabyteSizeLimit:N1} MB");
                return Array.Empty<byte>();
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);

                // Check the content length in case the file's only
                // content was a BOM and the content is actually
                // empty after removing the BOM.
                if (memoryStream.Length == 0)
                {
                    modelState.AddModelError(formFile.Name, $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
                }

                if (!IsValidFileExtensionAndSignature(formFile.FileName, memoryStream, permittedExtensions))
                {
                    modelState.AddModelError(formFile.Name, $"{fieldDisplayName}({trustedFileNameForDisplay}) file " +
                         "type isn't permitted or the file's signature doesn't match the file's extension.");
                }
                else
                {
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                var message = $"{fieldDisplayName}({trustedFileNameForDisplay}) upload failed. " +
                    $"Please contact the Help Desk for support. Error: {ex.HResult}";
                modelState.AddModelError(formFile.Name, message);
                Serilog.Log.Error(ex, message);
            }

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Validate streamed file
        /// </summary>
        /// <param name="section">A multipart section <see cref="MultipartSection"/></param>
        /// <param name="contentDisposition">Value of content-diposition header <see cref="ContentDispositionHeaderValue"/></param>
        /// <param name="modelState">Model state for validation errors <see cref="ModelStateDictionary"/></param>
        /// <param name="permittedExtensions">Array of files extensions permitted by application</param>
        /// <param name="sizeLimit">file size limit</param>
        /// <returns>Files content by bytes array</returns>
        public static async Task<byte[]> ProcessStreamedFile(MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await section.Body.CopyToAsync(memoryStream);

                // Check if the file is empty or exceeds the size limit.
                if (memoryStream.Length == 0)
                {
                    modelState.AddModelError("File", "The file is empty.");
                }
                else if (memoryStream.Length > sizeLimit)
                {
                    var megabyteSizeLimit = sizeLimit / 1048576;
                    modelState.AddModelError("File",
                    $"The file exceeds {megabyteSizeLimit:N1} MB.");
                }
                else if (!IsValidFileExtensionAndSignature(
                    contentDisposition.FileName.Value, memoryStream, permittedExtensions))
                {
                    modelState.AddModelError("File",
                        "The file type isn't permitted or the file's " +
                        "signature doesn't match the file's extension.");
                }
                else
                {
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                var message = $"The upload failed. Please contact the Help Desk for support. Error: {ex.HResult}";
                modelState.AddModelError("File", message);
                Serilog.Log.Error(ex, message);
            }
            return Array.Empty<byte>();
        }


        private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            data.Position = 0;

            using var reader = new BinaryReader(data);
            // Uncomment the following code block if you must permit
            // files whose signature isn't provided in the _fileSignature
            // dictionary. We recommend that you add file signatures
            // for files (when possible) for all file types you intend
            // to allow on the system and perform the file signature
            // check.
            if (!_fileSignature.ContainsKey(ext))
            {
                return true;
            }

            // File signature check
            // With the file signatures provided in the _fileSignature
            // dictionary, the following code tests the input content's
            // file signature.  
            var signatures = _fileSignature[ext];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}
