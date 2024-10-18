using System.Security.Cryptography;
using System.Text;

namespace Common.Helper.Helper
{
    public static class SecurityHelper
    {
        public static string AdvancePasswordHash(string pass)
        {
            var str1 = pass.Substring(0, pass.Length / 2);
            var str2 = pass.Substring(pass.Length / 2);
            str1 = Sha1HashStringForUtf8String(str1);
            str2 = CreateMd5(str2);
            var str = str1 + str2;
            str = CreateMd5(str);
            return str;
        }

        private static string Sha1HashStringForUtf8String(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var hex in bytes.Select(b => b.ToString("x2")))
            {
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string GetSha256Hash(string input)
        {
            using var hashAlgorithm = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        public static Guid CreateCryptographicallySecureGuid()
        {
            var bytes = new byte[16];
            var rand = RandomNumberGenerator.Create();
            rand.GetBytes(bytes);
            return new Guid(bytes);
        }

        public static long CreateUniqNumber()
        {
            var guid = Guid.NewGuid();
            var guidBytes = guid.ToByteArray();
            var uniqueNumber = BitConverter.ToInt64(guidBytes, 0);

            return uniqueNumber;
        }
    }
}
