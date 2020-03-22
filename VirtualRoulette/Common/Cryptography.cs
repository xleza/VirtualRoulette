using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VirtualRoulette.Common
{
    public static class Cryptography
    {
        public static string EncryptPassword(string password) // Not using additional salt because it's a test project :)
            => Convert.ToBase64String(KeyDerivation.Pbkdf2(password, new byte[0], KeyDerivationPrf.HMACSHA1, 10000, 256 / 8));
    }
}
