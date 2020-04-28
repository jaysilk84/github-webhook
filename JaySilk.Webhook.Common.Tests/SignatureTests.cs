using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Xunit;
using JaySilk.Webhook.Common.Security;
using JaySilk.Webhook.Common.Mvc;

namespace JaySilk.Webhook.Common.Tests
{

    public class SignatureTests
    {
        [Fact]
        public async Task Signing_ShouldGenerateCorrectSignature()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret1";
            var expectedSignature = HmacSignature.CreateFromExisting("c842eb4acaa566b634f845417d8a4593928e9ec4");

            using var context = new TestHttpContext(body);

            // Act
            var actualSignature = await context.Instance.SignAsync(secret);

            // Assert
            Assert.Equal(expectedSignature, actualSignature);

        }

        [Fact]
        public async Task Verify_SignaturesShouldMatchWhenSignedBySameKey()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret1";
            var expectedSignature = "c842eb4acaa566b634f845417d8a4593928e9ec4";
            using var context = new TestHttpContext(body);

            // Act
            var result = await context.Instance.VerifySignatureAsync(expectedSignature, secret);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Verify_Header_SignaturesShouldMatchWhenSignedBySameKey()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret1";
            using var context = new TestHttpContext(body);
            context.AddHeader("header", "c842eb4acaa566b634f845417d8a4593928e9ec4");

            // Act
            var result = await context.Instance.VerifySignatureFromHeaderAsync("header", secret);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Verify_Header_SignaturesShouldMatchWhenSignedBySameKeyAfterTransformation()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret1";
            using var context = new TestHttpContext(body);
            context.AddHeader("header", "sha1=c842eb4acaa566b634f845417d8a4593928e9ec4");

            // Act
            var result = await context.Instance.VerifySignatureFromHeaderAsync("header", secret, x => x.Substring(5));

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Verify_Filter_SuccessReturns200()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret1";
            var next = new TestRequestDelegate();
            using var context = new TestHttpContext(body);
            context.AddHeader("header", "sha1=c842eb4acaa566b634f845417d8a4593928e9ec4");
            var middleware = new TestVerifySignatureMiddleware(next.Invoke, "header", secret, x => x.Substring(5));

            // Act
            await middleware.Invoke(context.Instance);

            // Assert
            Assert.Equal(200, context.Instance.Response.StatusCode);
            Assert.True(next.Called);
        }

                [Fact]
        public async Task Verify_Filter_FailureReturns401()
        {
            // Arrange
            var body = "happy birthday";
            var secret = "secret2";
            var next = new TestRequestDelegate();
            using var context = new TestHttpContext(body);
            context.AddHeader("header", "sha1=c842eb4acaa566b634f845417d8a4593928e9ec4");
            var middleware = new TestVerifySignatureMiddleware(next.Invoke, "header", secret, x => x.Substring(5));

            // Act
            await middleware.Invoke(context.Instance);

            // Assert
            Assert.Equal(401, context.Instance.Response.StatusCode);
            Assert.False(next.Called);
        }

        private class TestHttpContext : IDisposable
        {
            public DefaultHttpContext Instance { get; }
            private readonly MemoryStream _stream;
            private readonly StreamWriter _writer;
            public TestHttpContext(string body)
            {
                var context = new DefaultHttpContext();
                _stream = new MemoryStream();
                _writer = new StreamWriter(_stream, Encoding.UTF8);

                _writer.Write(body);
                context.Request.Body = _stream;
                _stream.Position = 0;

                Instance = context;
            }

            public void AddHeader(string key, string value) => Instance.Request.Headers.Add(key, value);

            public void Dispose()
            {
                _writer.Dispose();
                _stream.Dispose();
            }

        }

        private class TestVerifySignatureMiddleware : VerifySignatureMiddleware
        {

            public TestVerifySignatureMiddleware(RequestDelegate next, string signatureHeaderName, string secret, Func<string, string> headerValueTransformer) :
                base(next, signatureHeaderName, secret, headerValueTransformer)
            { }

        }

        private class TestRequestDelegate
        {
            private readonly int _statusCode;

            public bool Called => CalledCount > 0;
            public int CalledCount { get; private set; }

            // public TestRequestDelegate(int statusCode = 200)
            // {
            //     _statusCode = statusCode;
            // }

            public Task Invoke(HttpContext context)
            {
                CalledCount++;
                //context.Response.StatusCode = _statusCode;
                return Task.CompletedTask;
            }
        }

    }


}
