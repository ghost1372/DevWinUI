using System.Security.Cryptography;

namespace DevWinUI;
public static partial class SecurityHelper
{
    /// <summary>
    /// Generates a random AES encryption key and initialization vector (IV). The key and IV are returned as byte
    /// arrays.
    /// </summary>
    /// <returns>Returns a tuple containing the generated AES key and IV.</returns>
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

    /// <summary>
    /// Encrypts a string using AES encryption and returns the result as a byte array.
    /// </summary>
    /// <param name="plainText">The text to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">The key used for the AES encryption process.</param>
    /// <param name="aes_IV">The initialization vector used to enhance the security of the encryption.</param>
    /// <returns>A byte array containing the encrypted data.</returns>
    public static byte[] EncryptStringAesToByte(string plainText, string aes_Key, string aes_IV)
    {
        return EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using AES encryption and returns the encrypted data as a byte array.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="aes_Key">Outputs the AES encryption key used for the encryption process.</param>
    /// <param name="aes_IV">Outputs the initialization vector used in the AES encryption.</param>
    /// <returns>A byte array containing the encrypted data.</returns>
    public static byte[] EncryptStringAesToByte(string plainText, out string aes_Key, out string aes_IV)
    {
        return EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using AES encryption and returns the result as a byte array.
    /// </summary>
    /// <param name="plainText">The text to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">The key used for the AES encryption process.</param>
    /// <param name="aes_IV">The initialization vector used to enhance the security of the encryption.</param>
    /// <param name="encodeType">Specifies the encoding format for the encrypted output.</param>
    /// <returns>A byte array containing the encrypted data.</returns>
    public static byte[] EncryptStringAesToByte(string plainText, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        return EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, encodeType);
    }

    /// <summary>
    /// Encrypts a string using AES encryption and returns the encrypted data as a byte array.
    /// </summary>
    /// <param name="plainText">The text to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">Outputs the AES encryption key used for the encryption process.</param>
    /// <param name="aes_IV">Outputs the initialization vector used in the AES encryption.</param>
    /// <param name="encodeType">Specifies the encoding type for the output data.</param>
    /// <returns>Returns the encrypted data as a byte array.</returns>
    public static byte[] EncryptStringAesToByte(string plainText, out string aes_Key, out string aes_IV, EncodeType encodeType)
    {
        return EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, encodeType);
    }

    /// <summary>
    /// Encrypts a string using AES encryption with a specified key and initialization vector.
    /// </summary>
    /// <param name="plainText">The text to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">The key used for encrypting the text securely.</param>
    /// <param name="aes_IV">The initialization vector that adds randomness to the encryption process.</param>
    /// <returns>The encrypted string represented in hexadecimal format.</returns>
    public static string EncryptStringAes(string plainText, string aes_Key, string aes_IV)
    {
        var encrypted = EncryptStringAesBase(plainText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
        return Convert.ToHexString(encrypted);
    }

    /// <summary>
    /// Encrypts a string using AES encryption and returns the encrypted value in hexadecimal format.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="aes_Key">Outputs the AES encryption key used during the encryption process.</param>
    /// <param name="aes_IV">Outputs the initialization vector used for the AES encryption.</param>
    /// <returns>The encrypted string represented in hexadecimal format.</returns>
    public static string EncryptStringAes(string plainText, out string aes_Key, out string aes_IV)
    {
        var encrypted = EncryptStringAesBase(plainText, null, null, out aes_Key, out aes_IV, EncodeType.Hex);
        return Convert.ToHexString(encrypted);
    }

    /// <summary>
    /// Encrypts a string using AES encryption and returns the result in a specified encoding format.
    /// </summary>
    /// <param name="plainText">The text that needs to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">The key used for the AES encryption process.</param>
    /// <param name="aes_IV">The initialization vector used to enhance the security of the encryption.</param>
    /// <param name="encodeType">Specifies the format in which the encrypted string will be returned, either hexadecimal or base64.</param>
    /// <returns>The encrypted string in the specified encoding format.</returns>
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

    /// <summary>
    /// Encrypts a string using AES encryption and returns the encrypted result in a specified format.
    /// </summary>
    /// <param name="plainText">The text to be encrypted using AES encryption.</param>
    /// <param name="aes_Key">Outputs the AES encryption key used during the encryption process.</param>
    /// <param name="aes_IV">Outputs the initialization vector used for the AES encryption.</param>
    /// <param name="encodeType">Specifies the format for the encrypted output, either hexadecimal or base64.</param>
    /// <returns>The encrypted string in the specified format.</returns>
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

    /// <summary>
    /// Decrypts a string that has been encrypted using AES encryption.
    /// </summary>
    /// <param name="cipherText">The encrypted text represented in a hexadecimal format.</param>
    /// <param name="aes_Key">The key used for decrypting the encrypted text.</param>
    /// <param name="aes_IV">The initialization vector used in the decryption process.</param>
    /// <returns>The decrypted string resulting from the decryption operation.</returns>
    public static string DecryptStringAes(string cipherText, string aes_Key, string aes_IV)
    {
        return DecryptStringAesBase(Convert.FromHexString(cipherText), aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts a string using AES encryption based on the provided parameters.
    /// </summary>
    /// <param name="cipherText">The encrypted text that needs to be decrypted.</param>
    /// <param name="aes_Key">The key used for decrypting the encrypted text.</param>
    /// <param name="aes_IV">The initialization vector used in the decryption process.</param>
    /// <param name="encodeType">Specifies the encoding format of the encrypted text, either Hex or Base64.</param>
    /// <returns>The decrypted string resulting from the decryption process.</returns>
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

    /// <summary>
    /// Decrypts a byte array using AES encryption with a specified key and initialization vector.
    /// </summary>
    /// <param name="cipherText">The encrypted data that needs to be decrypted.</param>
    /// <param name="aes_Key">The key used for decrypting the encrypted data.</param>
    /// <param name="aes_IV">The initialization vector used in the decryption process.</param>
    /// <returns>The decrypted string representation of the input data.</returns>
    public static string DecryptStringAes(byte[] cipherText, string aes_Key, string aes_IV)
    {
        return DecryptStringAesBase(cipherText, aes_Key, aes_IV, out _, out _, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts a byte array using AES encryption with the provided key and initialization vector.
    /// </summary>
    /// <param name="cipherText">The encrypted data that needs to be decrypted.</param>
    /// <param name="aes_Key">The key used for decrypting the encrypted data.</param>
    /// <param name="aes_IV">The initialization vector used in the decryption process.</param>
    /// <param name="encodeType">Specifies the encoding format for the decrypted output.</param>
    /// <returns>Returns the decrypted string representation of the input data.</returns>
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

    /// <summary>
    /// Encrypts a file using AES encryption with the specified key and initialization vector.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be encrypted.</param>
    /// <param name="outputFilePath">Indicates where to save the encrypted file.</param>
    /// <param name="aes_Key">Provides the key used for the AES encryption process.</param>
    /// <param name="aes_IV">Supplies the initialization vector for the AES encryption.</param>
    public static void EncryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, EncodeType.Base64);
    }

    /// <summary>
    /// Encrypts a file using AES encryption with specified parameters.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be encrypted.</param>
    /// <param name="outputFilePath">Indicates where the encrypted file will be saved.</param>
    /// <param name="aes_Key">Provides the key used for AES encryption.</param>
    /// <param name="aes_IV">Supplies the initialization vector for the encryption process.</param>
    /// <param name="encodeType">Determines the encoding type to be used during encryption.</param>
    public static void EncryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, encodeType);
    }

    /// <summary>
    /// Encrypts a file using AES encryption and generates a key and initialization vector for decryption.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be encrypted.</param>
    /// <param name="outputFilePath">Specifies the path where the encrypted file will be saved.</param>
    /// <param name="aes_Key">Outputs the AES key used for encrypting the file.</param>
    /// <param name="aes_IV">Outputs the initialization vector used for the encryption process.</param>
    public static void EncryptFileAES(string inputFilePath, string outputFilePath, out string aes_Key, out string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, null, null, out aes_Key, out aes_IV, EncodeType.Base64);
    }

    /// <summary>
    /// Encrypts a file using AES encryption and generates a key and initialization vector.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be encrypted.</param>
    /// <param name="outputFilePath">Indicates where to save the encrypted file.</param>
    /// <param name="aes_Key">Outputs the AES encryption key used during the process.</param>
    /// <param name="aes_IV">Outputs the initialization vector used for the encryption.</param>
    /// <param name="encodeType">Determines the encoding type for the encrypted file.</param>
    public static void EncryptFileAES(string inputFilePath, string outputFilePath, out string aes_Key, out string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Encrypt, inputFilePath, outputFilePath, null, null, out aes_Key, out aes_IV, encodeType);
    }

    /// <summary>
    /// Decrypts a file using AES encryption with the specified key and initialization vector.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be decrypted.</param>
    /// <param name="outputFilePath">Indicates where to save the decrypted file.</param>
    /// <param name="aes_Key">Represents the key used for the AES decryption process.</param>
    /// <param name="aes_IV">Defines the initialization vector used in the AES decryption.</param>
    public static void DecryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV)
    {
        CryptoFileAESBase(CryptoMode.Decrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, EncodeType.Base64);
    }

    /// <summary>
    /// Decrypts a file using AES encryption with the specified parameters.
    /// </summary>
    /// <param name="inputFilePath">Specifies the path of the file to be decrypted.</param>
    /// <param name="outputFilePath">Indicates where to save the decrypted file.</param>
    /// <param name="aes_Key">Provides the key used for the AES decryption process.</param>
    /// <param name="aes_IV">Supplies the initialization vector for the AES decryption.</param>
    /// <param name="encodeType">Defines the encoding type to be used during the decryption.</param>
    public static void DecryptFileAES(string inputFilePath, string outputFilePath, string aes_Key, string aes_IV, EncodeType encodeType)
    {
        CryptoFileAESBase(CryptoMode.Decrypt, inputFilePath, outputFilePath, aes_Key, aes_IV, out _, out _, encodeType);
    }
}
