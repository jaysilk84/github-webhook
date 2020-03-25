using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JaySilk.Webhook.Common.Math
{
    /// <summary>
    /// Represents a hexadecimal string. Provides access to the bytes or
    /// the string representation in lowercase
    /// </summary>
    public class HexString
    {
        private readonly ImmutableArray<byte> _bytes;
        private readonly string _string;

        public HexString(string hex)
        {
            _bytes = HexStringToByteArray(hex).ToImmutableArray();
            _string = hex.ToLower();
        }

        public HexString(byte[] bytes) {
            _bytes = bytes.ToImmutableArray();
            _string = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public override string ToString() => _string;
        public ImmutableArray<byte> GetBytes() => _bytes;

        private static byte[] HexStringToByteArray(String hex)
        {
            var numChars = hex.Length;

            if (numChars % 2 != 0)
                throw new FormatException("invalid hex format");

            var bytes = new byte[numChars / 2];
            for (var i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }


}
