using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;

namespace AspNetCoreCracker
{
    class PasswordHasher
    {
        public static bool VerifyPasswordV3(string password, byte[] expectedSubkey, KeyDerivationPrf prf, int iterCount, int saltSize, byte[] salt, int numBytesRequested)
        {
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }

        public static HashComponents ExtractHashComponents(byte[] hashBytes)
        {
            var metadata = new HashComponents();
            metadata.CompatibilityMode = ExtractCompatiblityMode(hashBytes);
            metadata.Prf = ExtractPrf(hashBytes);
            metadata.IterationCount = ExtractIterationCount(hashBytes);
            metadata.SaltLength = ExtractSaltLength(hashBytes);
            metadata.Salt = ExtractSalt(hashBytes, metadata.SaltLength);
            metadata.SubkeyLength = hashBytes.Length - 13 - metadata.SaltLength;
            metadata.ExpectedSubkey = new byte[metadata.SubkeyLength];
            Buffer.BlockCopy(hashBytes, 13 + metadata.SaltLength, metadata.ExpectedSubkey, 0, metadata.SubkeyLength);
            return metadata;
        }

        public static PasswordHasherCompatibilityMode ExtractCompatiblityMode(byte[] hashBytes)
        {
            // We extract the compatiblity mode from the hash
            switch (hashBytes[0])
            {
                case 0x00:
                    return PasswordHasherCompatibilityMode.IdentityV2;
                case 0x01:
                    return PasswordHasherCompatibilityMode.IdentityV3;
                default:
                    throw new ArgumentException("Unable to identify the hash format.");
            }
        }

        public static KeyDerivationPrf ExtractPrf(byte[] hashBytes)
        {
            // We extract the KeyDerivationPrf from the hash
            return (KeyDerivationPrf)ReadNetworkByteOrder(hashBytes, 1); ;
        }

        public static int ExtractIterationCount(byte[] hashBytes)
        {
            // We extract the number of iterations from the hash
            return (int)ReadNetworkByteOrder(hashBytes, 5);
        }

        public static int ExtractSaltLength(byte[] hashBytes)
        {
            // We extract the salt length from the hash
            return (int)ReadNetworkByteOrder(hashBytes, 9);
        }

        public static byte[] ExtractSalt(byte[] hashBytes, int saltLength)
        {
            // We extract the salt from the hash
            byte[] salt = new byte[saltLength];

            // The salt has a length of 16 bytes (given in the hash header)
            // and it begins at the 13th byte of the hash
            Buffer.BlockCopy(hashBytes, 13, salt, 0, saltLength);

            return salt;
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | buffer[offset + 3];
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}
