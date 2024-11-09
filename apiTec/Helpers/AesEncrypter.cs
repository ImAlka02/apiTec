using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace apiTec.Helpers
{
    public static class AesEncrypter
    {
        private static string PrivateKey
        {
            get
            {
                var key = "3fb7fe5dbb0643caa984f53de6fffd0f";

                const string envVarName = "SECRET_KEY";

                var envKeyValue = Environment.GetEnvironmentVariable(envVarName);

                if (envKeyValue != null)
                {
                    key = envKeyValue;
                }
                return key;
            }
        }

        private static string PrivateIV
        {
            get
            {
                var IV = "OFeSNW+wWPVkqDVTyYKI3w==";

                const string envVarNameIV = "SECRET_IV";

                var envIVValue = Environment.GetEnvironmentVariable(envVarNameIV);

                if (envIVValue != null)
                {
                    IV = envIVValue;
                }
                return IV;
            }
        }
        public static string Encrypt(string plainText)
        {
            if (plainText is not { Length: > 0 })
                throw new ArgumentNullException(nameof(plainText));
            if (PrivateKey is not { Length: > 0 })
                throw new ArgumentNullException(nameof(PrivateKey));
            if (PrivateIV is not { Length: > 0 })
                throw new ArgumentNullException(nameof(PrivateIV));

            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key = CreateAesKey(PrivateKey);
                aesAlg.IV = Convert.FromBase64String(PrivateIV);
                //aesAlg.GenerateKey();
                //aesAlg.GenerateIV();

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
            //using (Aes aesAlg = Aes.Create())
            //{
            //    aesAlg.Key = Key;
            //    aesAlg.IV = IV;
            //    aesAlg.Mode = CipherMode.CBC;
            //    aesAlg.Padding = PaddingMode.PKCS7;

            //    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            //    using (MemoryStream msEncrypt = new MemoryStream())
            //    {
            //        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            //        {
            //            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            //            {
            //                swEncrypt.Write(plainText);
            //            }
            //            byte[] encrypted = msEncrypt.ToArray();
            //            return Convert.ToBase64String(encrypted);
            //        }
            //    }
            //}
        }

        // Método para desencriptar
        public static string Decrypt(string cipherText)
        {
            if (cipherText is not { Length: > 0 })
                throw new ArgumentNullException(nameof(cipherText));
            if (PrivateKey is not { Length: > 0 })
                throw new ArgumentNullException(nameof(PrivateKey));
            if (PrivateIV is not { Length: > 0 })
                throw new ArgumentNullException(nameof(PrivateIV));

            using var aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = CreateAesKey(PrivateKey);
            aesAlg.IV = Convert.FromBase64String(PrivateIV);

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            var plaintext = srDecrypt.ReadToEnd();

            return plaintext;
        }
        public static byte[] GenerateRandomPublicKey()
        {
            var iv = new byte[16]; // AES > IV > 128 bit
            iv = RandomNumberGenerator.GetBytes(iv.Length);
            return iv;
        }
        private static byte[] CreateAesKey(string inputString)

        {
            return Encoding.UTF8.GetByteCount(inputString) == 32 ? Encoding.UTF8.GetBytes(inputString) : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}
