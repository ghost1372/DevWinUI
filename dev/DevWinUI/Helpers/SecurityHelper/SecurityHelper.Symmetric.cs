using System.Security.Cryptography;

namespace DevWinUI;
public static partial class SecurityHelper
{
    public static (byte[] Key, byte[] IV) GenerateAESKeyAndIV()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();
        return (Key: aes.Key, IV: aes.IV);
    }
    private static byte[] EncryptStringAesBase(string plainText, string aes_Key, string aes_IV, out string aes_KeyOut, out string aes_IVOut, EncodeType encodeType)
    {
        byte[] encrypted;

        using var aesAlg = GetAESSymmetricAlgorithm(aes_Key, aes_IV, out aes_KeyOut, out aes_IVOut, encodeType);
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msEncrypt = new MemoryStream())
        {
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }

        return encrypted;
    }

    public static byte[] EncryptStringAesToByte(string plainText, string aes_Key, string aes_IV)
    {
        return EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }
    public static byte[] EncryptStringAesToByte(string plainText, out string aes_Key, out string aes_IV)
    {
        return EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, EncodeType.Hex);
    }

    public static byte[] EncryptStringAesToByte(string plainText, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        return EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, encodeType);
    }
    public static byte[] EncryptStringAesToByte(string plainText, out string aes_Key, out string aes_IV, EncodeType encodeType)
    {
        return EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, encodeType);
    }

    public static string EncryptStringAes(string plainText, string aes_Key, string aes_IV)
    {
        var encrypted = EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
        return Convert.ToHexString(encrypted);
    }
    public static string EncryptStringAes(string plainText, out string aes_Key, out string aes_IV)
    {
        var encrypted = EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, EncodeType.Hex);
        return Convert.ToHexString(encrypted);
    }

    public static string EncryptStringAes(string plainText, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        var encrypted = EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, encodeType);
        if (encodeType == EncodeType.Hex)
        {
            return Convert.ToHexString(encrypted);
        }
        else
        {
            return Convert.ToBase64String(encrypted);
        }
    }
    public static string EncryptStringAes(string plainText, out string aes_Key, out string aes_IV, EncodeType encodeType)
    {
        var encrypted = EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, encodeType);
        if (encodeType == EncodeType.Hex)
        {
            return Convert.ToHexString(encrypted);
        }
        else
        {
            return Convert.ToBase64String(encrypted);
        }
    }

    private static string DecryptStringAesBase(byte[] cipherText, string aes_Key, string aes_IV, out string aes_KeyOut, out string aes_IVOut, EncodeType encodeType)
    {
        string plaintext = null;
        using var aesAlg = GetAESSymmetricAlgorithm(aes_Key, aes_IV, out aes_KeyOut, out aes_IVOut, encodeType);

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        {
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                plaintext = srDecrypt.ReadToEnd();
            }
        }

        return plaintext;
    }

    public static string DecryptStringAes(string cipherText, string aes_Key, string aes_IV)
    {
        return DecryptStringAesBase(Convert.FromHexString(cipherText), aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }

    public static string DecryptStringAes(string cipherText, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        if (encodeType == EncodeType.Hex)
        {
            var hexBytes = Convert.FromHexString(cipherText);
            return DecryptStringAesBase(hexBytes, aes_Key, aes_IV, out _, out _, encodeType);
        }
        else
        {
            var base64bytes = Convert.FromBase64String(cipherText);
            return DecryptStringAesBase(base64bytes, aes_Key, aes_IV, out _, out _, encodeType);
        }
    }

    public static string DecryptStringAes(byte[] cipherText, string aes_Key, string aes_IV)
    {
        return DecryptStringAesBase(cipherText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }

    public static string DecryptStringAes(byte[] cipherText, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        return DecryptStringAesBase(cipherText, aes_Key, aes_IV, out _, out _, encodeType);
    }

    private static System.Security.Cryptography.SymmetricAlgorithm GetAESSymmetricAlgorithm(string aes_Key, string aes_IV, out string aes_KeyOut, out string aes_IVOut, EncodeType encodeType)
    {
        System.Security.Cryptography.SymmetricAlgorithm symmetricAlgorithm;
        symmetricAlgorithm = Aes.Create();
        if (string.IsNullOrEmpty(aes_Key) || string.IsNullOrEmpty(aes_IV))
        {
            symmetricAlgorithm.GenerateIV();
            symmetricAlgorithm.GenerateKey();
        }
        else
        {
            byte[] keyBytes;
            byte[] ivBytes;
            if (encodeType == EncodeType.Hex)
            {
                keyBytes = Convert.FromHexString(aes_Key);
                ivBytes = Convert.FromHexString(aes_IV);
            }
            else
            {
                keyBytes = Convert.FromBase64String(aes_Key);
                ivBytes = Convert.FromBase64String(aes_IV);
            }

            symmetricAlgorithm.Key = keyBytes;
            symmetricAlgorithm.IV = ivBytes;
        }

        if (encodeType == EncodeType.Hex)
        {
            aes_KeyOut = Convert.ToHexString(symmetricAlgorithm.Key);
            aes_IVOut = Convert.ToHexString(symmetricAlgorithm.IV);
        }
        else
        {
            aes_KeyOut = Convert.ToBase64String(symmetricAlgorithm.Key);
            aes_IVOut = Convert.ToBase64String(symmetricAlgorithm.IV);
        }

        return symmetricAlgorithm;
    }

    private static void CryptoFileAESBase(CryptoMode cryptoMode, string inputFilePath, string outputFilePath, string aes_Key, string aes_IV, out string aes_KeyOut, out string aes_IVOut, EncodeType encodeType)
    {
        using FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
        using FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        outputStream.SetLength(0);

        //Create variables to help with read and write.
        byte[] bin = new byte[100]; //This is intermediate storage for the encryption.
        long rdlen = 0;              //This is the total number of bytes written.
        long totlen = inputStream.Length;    //This is the total length of the input file.
        int len;                     //This is the number of bytes to be written at a time.

        using var symmetricAlgorithm = GetAESSymmetricAlgorithm(aes_Key, aes_IV, out aes_KeyOut, out aes_IVOut, encodeType);

        ICryptoTransform cryptoTransform = null;
        switch (cryptoMode)
        {
            case CryptoMode.Encrypt:
                cryptoTransform = symmetricAlgorithm.CreateEncryptor();
                break;
            case CryptoMode.Decrypt:
                cryptoTransform = symmetricAlgorithm.CreateDecryptor();
                break;
        }

        using CryptoStream cryptoStream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write);
        while (rdlen < totlen)
        {
            len = inputStream.Read(bin, 0, 100);
            cryptoStream.Write(bin, 0, len);
            rdlen = rdlen + len;
        }
        cryptoStream.Close();
        outputStream.Close();
        inputStream.Close();
    }

    public static void EncryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, EncodeType.Base64);
    }

    public static void EncryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, encodeType);
    }

    public static void EncryptFileAES(string inputFilePath, string outputFilePath, out string aes_Key, out string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, null, null, out aes_Key, out aes_IV, EncodeType.Base64);
    }

    public static void EncryptFileAES(string inputFilePath, string outputFilePath, out string aes_Key, out string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, null, null, out aes_Key, out aes_IV, encodeType);
    }

    public static void DecryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Decrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, EncodeType.Base64);
    }
    public static void DecryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Decrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, encodeType);
    }
}
