using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(options => Crack(options));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Crack(Options options)
        {
            IEnumerable<string> passwords = File.ReadLines(options.PasswordFile);
            IEnumerable<string> hashes = File.ReadLines(options.HashFile);

            Stopwatch totalWatch = Stopwatch.StartNew();

            foreach (string hash in hashes)
            {
                byte[] hashBytes = Convert.FromBase64String(hash);
                var hashComponents = PasswordHasher.ExtractHashComponents(hashBytes);

                if(hashComponents.CompatibilityMode != PasswordHasherCompatibilityMode.IdentityV3)
                {
                    throw new ArgumentException("This tool is only compatible with IdentityV3 hashes.");
                }

                Console.WriteLine($"\nHash: {hash}\nMode: {hashComponents.CompatibilityMode}, PRF: {hashComponents.Prf}, Iteration count: {hashComponents.IterationCount}, Salt length: {hashComponents.SaltLength}, Subkey length: {hashComponents.SubkeyLength}");

                Stopwatch hashWatch = Stopwatch.StartNew();

                int testedPasswords = 0;

                // Use all the available cores
                Parallel.ForEach(passwords, (password, state) =>
                {
                    testedPasswords++;
                    if (PasswordHasher.VerifyPasswordV3(password, hashComponents.ExpectedSubkey, hashComponents.Prf, hashComponents.IterationCount, hashComponents.SaltLength, hashComponents.Salt, hashComponents.SubkeyLength))
                    {
                        // We managed to find a match
                        Console.WriteLine($"--> Password match: {password}");
                        state.Break();
                    }
                });

                hashWatch.Stop();
                var elapsedMs = hashWatch.ElapsedMilliseconds;
                Console.WriteLine($"Time taken to test {testedPasswords} passwords for this hash: {elapsedMs}ms (≈ {testedPasswords*1000/elapsedMs} computations per second)");
            }

            totalWatch.Stop();
            var elapsedMs2 = totalWatch.ElapsedMilliseconds;
            Console.WriteLine("Total time taken: {0}ms", elapsedMs2);
        }
    }
}
