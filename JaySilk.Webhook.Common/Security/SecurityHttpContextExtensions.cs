using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using JaySilk.Webhook.Common.Security;

namespace JaySilk.Webhook.Common.Security
{
    public interface ISignatureVerificationResult {
        bool IsValid {get;}
        string Message {get;}
    }

    public static class SecurityHttpContextExtensions
    {
        private const int BUFFER_SIZE = 1024 * 45; // 45kb

        private class SignatureVerificationResult : ISignatureVerificationResult
        {
            public string Message { get; }
            public bool IsValid { get; }

            public SignatureVerificationResult(bool isValid, string message)
            {
                IsValid = isValid;
                Message = message;
            }

            public SignatureVerificationResult(bool isValid) : this(isValid, String.Empty) { }
        }

        public static async Task<HmacSignature> SignAsync(this HttpContext context, string secret)
        {
            // Allow reading the stream multiple times
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body,
                bufferSize: BUFFER_SIZE,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            // Reset the stream for downstream filters
            context.Request.Body.Position = 0;

            return new HmacSignature(body, secret, Encoding.UTF8);
        }

        public static async Task<ISignatureVerificationResult> VerifySignatureAsync(this HttpContext context, string expectedSignature, string secret)
        {
            try {
                var actualHmacSignature = await context.SignAsync(secret);
                var expectedHmacSignature = HmacSignature.CreateFromExisting(expectedSignature); // this can throw

                return new SignatureVerificationResult(actualHmacSignature == expectedHmacSignature); // safe compare

            } catch (FormatException ex) {
                return new SignatureVerificationResult(false, ex.Message);
            }
        }

        public static async Task<ISignatureVerificationResult> VerifySignatureFromHeaderAsync(this HttpContext context, string headerName, string secret, Func<string, string> headerValueTransformer)
        {
            if (!context.Request.Headers.ContainsKey(headerName))
                return new SignatureVerificationResult(false, "HMAC signature missing");

            var expectedSignature = headerValueTransformer(context.Request.Headers[headerName]);
            return await context.VerifySignatureAsync(expectedSignature, secret);
        }

        public static async Task<ISignatureVerificationResult> VerifySignatureFromHeaderAsync(this HttpContext context, string headerName, string secret) =>
            await VerifySignatureFromHeaderAsync(context, headerName, secret, (s) => s);
    }
}
