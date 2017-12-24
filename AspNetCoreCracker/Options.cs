using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace AspNetCoreCracker
{
    class Options
    {
        [Option("hashes", Required = true,
            HelpText = "The path to a file with a list of hashes to crack.")]
        public string HashFile { get; set; }

        [Option("passwords", Required = true,
            HelpText = "The path to a file with a list of passwords to hash.")]
        public string PasswordFile { get; set; }

        [Usage(ApplicationAlias = "AspNetCoreCracker")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Attempt to crack a list of hashes", new Options { HashFile = "hashes.txt", PasswordFile = "passwords.txt" });
            }
        }
    }
}
