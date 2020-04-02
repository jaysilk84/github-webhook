using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace JaySilk.Webhook.Common.Security
{

    public sealed class HmacSignature
    {
        public HexString Signature { get; }

        public HmacSignature(byte[] content, byte[] secret)
        {
            Signature = GenerateSignature(content, secret);
        }

        private HmacSignature(string existingSignatureAsString)
        {
            Signature = new HexString(existingSignatureAsString);
        }

        public HmacSignature(string content, string secret, Encoding encoding) : this(encoding.GetBytes(content), encoding.GetBytes(secret)) { }
        public HmacSignature(byte[] content, string secret, Encoding encoding) : this(content, encoding.GetBytes(secret)) { }

        private HexString GenerateSignature(byte[] content, byte[] secret)
        {
            using var hmac = new HMACSHA1(secret);
            return new HexString(hmac.ComputeHash(content));
        }

        public override bool Equals(object value)
        {
            var other = value as HmacSignature;

            if (Object.ReferenceEquals(null, other)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return this.Signature == other.Signature;
        }

        public static bool operator ==(HmacSignature s1, HmacSignature s2) => s1.Equals(s2);
        public static bool operator !=(HmacSignature s1, HmacSignature s2) => !(s1 == s2);

        public override int GetHashCode() => Signature.GetHashCode();

        public static HmacSignature CreateFromExisting(string signature) => new HmacSignature(signature);
    }

}

