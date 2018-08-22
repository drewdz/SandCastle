using System;
using System.Linq;
using PCLCrypto;

namespace PE.Plugins.SecureStorage
{
    public class CryptoService
    {
		public const HashAlgorithm DefaultAlgorithm = HashAlgorithm.Md5;

        public static string HexHashString(string value, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            var hashed = HashString(value, algorithm);
            return ByteArrayToString(hashed);
        }

        public static string HexHashString(string value, byte[] salt, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            var hashed = HashString(value, salt, algorithm);
            return ByteArrayToString(hashed);
        }

        public static byte[] HashString(string value, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            return GetHash(value, new byte[0], algorithm);
        }

        public static byte[] HashString(string value, byte[] salt, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            return GetHash(value, salt, algorithm);
        }

        public static bool VerifyString(string value, byte[] hash, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            return VerifyString(value, hash, new byte[0], algorithm);
        }

        public static bool VerifyString(string value, byte[] hash, byte[] salt, HashAlgorithm algorithm = DefaultAlgorithm)
        {
            if (hash.Length == 0)
                throw new ArgumentException("Hash cannot be empty");

            return GetHash(value, salt, algorithm).SequenceEqual(hash);
        }

        public static byte[] GenerateSalt()
        {
            return Guid.NewGuid().ToByteArray();
        }

        static byte[] GetHash(string value, byte[] salt, HashAlgorithm algorithm)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String cannot be empty");
			
            if (salt.Length == 0)
                salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            var key = NetFxCrypto.DeriveBytes.GetBytes(value, salt, _iterations, _keyLengthInBytes);

            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(algorithm);
            var hash = hasher.HashData(key);

            return hash;
        }

        static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        const int _iterations = 1000;
        const int _keyLengthInBytes = 32;
    }
}
