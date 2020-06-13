# AspNetCoreCracker
A simple tool to attempt to crack ASP.NET Core Identity hashes using CPU power.

## Introduction
ASP.NET Core Identity's hashing algorithm version 3 has the following characteristics by default:
* PBKDF2 with HMAC-SHA256
* 128-bit salt
* 256-bit subkey
* 10000 iterations

IMHO, 10000 iterations is too low and will not stop an attacker who has found some hashes somewhere to give it a try and make an opportunistic dictionary attack to attempt to retrieve a password.

So just provide the hashes and your dictionary, maybe you'll be lucky. :smile:

This tool only supports the version 3 format. It will however extract and reuse dynamically the following characteristics from the hashes:
* Key derivation PRF
* Salt size and salt
* Subkey size and subkey
* Iteration count

Performance on Intel Core i7-2600K @ 3.4 Ghz (4 cores with HT): about 420 computations per second.

## Usage
```
USAGE:
Attempt to crack a list of hashes:
  AspNetCoreCracker --hashes hashes.txt --passwords passwords.txt

  --hashes       Required. The path to a file with a list of hashes to crack.

  --passwords    Required. The path to a file with a list of passwords to hash.

  --help             Display this help screen.

  --version          Display version information.
```

## Prerequisite
.NET Core 2.0.0 (Cross-platform)

Tested on Windows 10 x64 and Ubuntu 16.04 x64.

## Run
`dotnet run --hashes hashes.txt --passwords passwords.txt`

```
Hash: AQAAAAEAACcQAAAAECwiNBHkjeMpLO86KsmvnhqeHmjucyGV8Fl1s3oFUUOgN8fn+Pzmxs0opC+ScTYsUw==
Mode: IdentityV3, PRF: HMACSHA256, Iteration count: 10000, Salt length: 16, Subkey length: 32
--> Password match: CoreCracker2018!
Time taken to test 203 passwords for this hash: 480ms (~ 422 computations per second)
Total time taken: 483ms
```

## How to improve?
You can either increase the number of iterations or use another hashing algorithm (cf. links below).

## Resources
* https://github.com/aspnet/Identity/blob/dev/src/Microsoft.Extensions.Identity.Core/PasswordHasher.cs
* https://andrewlock.net/exploring-the-asp-net-core-identity-passwordhasher/
* https://andrewlock.net/migrating-passwords-in-asp-net-core-identity-with-a-custom-passwordhasher/
* http://www.blinkingcaret.com/2017/11/29/asp-net-identity-passwordhash/

## License

MIT License

Copyright (c) 2017-2018 Julien EHRHART

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
