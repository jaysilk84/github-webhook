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

        public HexString(byte[] bytes)
        {
            _bytes = bytes.ToImmutableArray();
            _string = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public override string ToString() => _string;
        public ImmutableArray<byte> GetBytes() => _bytes;

        private static byte[] HexStringToByteArray(string hex)
        {
            var numChars = hex.Length;

            if (numChars % 2 != 0)
                throw new FormatException("invalid hex format");

            var bytes = new byte[numChars / 2];
            for (var i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            
            return bytes;
        }

        public override bool Equals(object value)
        {
            var other = value as HexString;

            if (Object.ReferenceEquals(null, other)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return SafeIsEqual(this._bytes, other.GetBytes());
        }

        public static bool operator ==(HexString s1, HexString s2) => s1.Equals(s2);
        public static bool operator !=(HexString s1, HexString s2) => !(s1 == s2);
        public override int GetHashCode() => _string.GetHashCode();


        /// <summary>
        /// Constant time compare of two byte arrays. Doesn't short circut
        /// after an unequal comparison to reduce timing attacks
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>bool</returns>
        private static bool SafeIsEqual(IEnumerable<byte> a, IEnumerable<byte> b) // TODO: Change to take array, IEnumerable doesnt make sense here
        {
            if (a.Count() != b.Count())
                return false;

            var result = 0;
            for (var i = 0; i < a.Count(); i++)
                result |= a.ElementAt(i) ^ b.ElementAt(i);

            return result == 0;
        }

    }


}
