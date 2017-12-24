using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AspNetCoreCracker
{
    class HashComponents
    {
        public PasswordHasherCompatibilityMode CompatibilityMode { get; set; }
        public KeyDerivationPrf Prf { get; set; }
        public int IterationCount { get; set; }
        public int SaltLength { get; set; }
        public byte[] Salt { get; set; }
        public int SubkeyLength { get; set; }
        public byte[] ExpectedSubkey { get; set; }
    }
}
