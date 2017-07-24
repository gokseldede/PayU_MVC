using System;
using System.Security.Cryptography;
using System.Text;

namespace PayU.PaymentHelper
{
    public static class HashHelper
    {
        public static string HashWithSignature(this string hashString, string signature)
        {
            var binaryHash = new HMACMD5(Encoding.UTF8.GetBytes(signature))
                .ComputeHash(Encoding.UTF8.GetBytes(hashString));

            var hash = BitConverter.ToString(binaryHash)
                .Replace("-", string.Empty)
                    .ToLowerInvariant();

            return hash;
        }
    }
}