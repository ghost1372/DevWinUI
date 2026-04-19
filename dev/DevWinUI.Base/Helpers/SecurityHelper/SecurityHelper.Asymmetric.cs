using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace DevWinUI;
public static partial class SecurityHelper
{
    private static string EncryptStringAsymmetricBase(string plainText, string publicKey, IBuffer publicKeyBuffer, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);
        CryptographicKey key = null;

        if (publicKeyBuffer == null)
        {
            var keyBlob = encodeType == EncodeType.Hex
               ? CryptographicBuffer.DecodeFromHexString(publicKey)
               : CryptographicBuffer.DecodeFromBase64String(publicKey);
        }
        else
        {
            key = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(asymmetricAlgorithm.ToString())
                .ImportPublicKey(publicKeyBuffer);
        }

        IBuffer encryptedData = CryptographicEngine.Encrypt(key, keyBuffer, null);

        return encodeType == EncodeType.Hex
                   ? CryptographicBuffer.EncodeToHexString(encryptedData)
                   : CryptographicBuffer.EncodeToBase64String(encryptedData);
    }
    private static string DecryptStringAsymmetricBase(string encryptedString, string privateKey, IBuffer privateKeyBuffer, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        var keyBuffer = encodeType == EncodeType.Hex
            ? CryptographicBuffer.DecodeFromHexString(encryptedString)
            : CryptographicBuffer.DecodeFromBase64String(encryptedString);

        CryptographicKey key;
        if (privateKeyBuffer == null)
        {
            var keyBlob = encodeType == EncodeType.Hex
                ? CryptographicBuffer.DecodeFromHexString(privateKey)
                : CryptographicBuffer.DecodeFromBase64String(privateKey);

            key = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(asymmetricAlgorithm.ToString())
                .ImportKeyPair(keyBlob);
        }
        else
        {
            key = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(asymmetricAlgorithm.ToString())
                .ImportKeyPair(privateKeyBuffer);
        }

        IBuffer decryptedData = CryptographicEngine.Decrypt(key, keyBuffer, null);

        return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedData);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <returns>The encrypted string in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, string publicKey)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="keyPair">An output parameter that holds the generated asymmetric key pair for encryption.</param>
    /// <returns>The encrypted string in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair)
    {
        keyPair = GenerateAsymmetricKeyPair(AsymmetricAlgorithm.RSA_PKCS1);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="asymmetricAlgorithm">Specifies the algorithm to be used for the asymmetric encryption process.</param>
    /// <returns>The encrypted string in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, string publicKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, asymmetricAlgorithm, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="keyPair">An output parameter that holds the generated asymmetric key pair.</param>
    /// <param name="asymmetricAlgorithm">Specifies the asymmetric encryption algorithm to be used for encryption.</param>
    /// <returns>The encrypted string in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        keyPair = GenerateAsymmetricKeyPair(asymmetricAlgorithm);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), asymmetricAlgorithm, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="encodeType">Specifies the encoding format for the encrypted output.</param>
    /// <returns>The encrypted string in the specified encoding format.</returns>
    public static string EncryptStringAsymmetric(string plainText, string publicKey, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="keyPair">An output parameter that holds the generated asymmetric key pair.</param>
    /// <param name="encodeType">Specifies the encoding format for the encrypted string.</param>
    /// <returns>The encrypted string resulting from the asymmetric encryption process.</returns>
    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, EncodeType encodeType)
    {
        keyPair = GenerateAsymmetricKeyPair(AsymmetricAlgorithm.RSA_PKCS1);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="asymmetricAlgorithm">The algorithm that defines the method of encryption to be used for the operation.</param>
    /// <param name="encodeType">The format in which the encrypted data will be represented after the encryption process.</param>
    /// <returns>The encrypted string resulting from the asymmetric encryption process.</returns>
    public static string EncryptStringAsymmetric(string plainText, string publicKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, asymmetricAlgorithm, encodeType);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="keyPair">An output parameter that holds the generated asymmetric key pair.</param>
    /// <param name="asymmetricAlgorithm">Specifies the algorithm used for the asymmetric encryption process.</param>
    /// <param name="encodeType">Determines the encoding format for the encrypted output.</param>
    /// <returns>The encrypted string resulting from the asymmetric encryption.</returns>
    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        keyPair = GenerateAsymmetricKeyPair(asymmetricAlgorithm);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), asymmetricAlgorithm, encodeType);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <returns>The encrypted string represented in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="asymmetricAlgorithm">The algorithm that defines the method of encryption to be applied.</param>
    /// <returns>The encrypted string represented in hexadecimal format.</returns>
    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, asymmetricAlgorithm, EncodeType.Hex);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="encodeType">Specifies the encoding format for the encrypted output.</param>
    /// <returns>The encrypted string in the specified encoding format.</returns>
    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    /// <summary>
    /// Encrypts a string using asymmetric encryption.
    /// </summary>
    /// <param name="plainText">The input string that needs to be encrypted.</param>
    /// <param name="publicKey">The key used to encrypt the input string securely.</param>
    /// <param name="asymmetricAlgorithm">The algorithm that defines the encryption method to be used.</param>
    /// <param name="encodeType">The format in which the encrypted output will be represented.</param>
    /// <returns>The encrypted string resulting from the encryption process.</returns>
    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, asymmetricAlgorithm, encodeType);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The key used for decrypting the encrypted string, which must be kept secure.</param>
    /// <returns>The decrypted plain text string.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="asymmetricAlgorithm">The algorithm employed for the asymmetric decryption process.</param>
    /// <returns>The decrypted string resulting from the decryption operation.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, asymmetricAlgorithm, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="encodeType">Specifies the encoding format of the decrypted string.</param>
    /// <returns>The decrypted string resulting from the decryption process.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="asymmetricAlgorithm">The algorithm that defines the method of asymmetric decryption to be used.</param>
    /// <param name="encodeType">The format in which the decrypted string will be encoded.</param>
    /// <returns>The decrypted string resulting from the decryption process.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, asymmetricAlgorithm, encodeType);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <returns>The decrypted plain text string.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, string privateKey)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="asymmetricAlgorithm">The algorithm employed for the decryption process.</param>
    /// <returns>The decrypted string.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, asymmetricAlgorithm, EncodeType.Hex);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="encodeType">Specifies the encoding format of the encrypted string.</param>
    /// <returns>Returns the decrypted string.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    /// <summary>
    /// Decrypts an encrypted string using asymmetric decryption with a specified private key.
    /// </summary>
    /// <param name="encryptedString">The input string that has been encrypted and needs to be decrypted.</param>
    /// <param name="privateKey">The private key used to decrypt the encrypted string.</param>
    /// <param name="asymmetricAlgorithm">The algorithm employed for the decryption process.</param>
    /// <param name="encodeType">The encoding format of the encrypted string.</param>
    /// <returns>The decrypted string resulting from the decryption process.</returns>
    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, asymmetricAlgorithm, encodeType);
    }

    private static CryptographicKey GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm asymmetricAlgorithm, RSAKeySize keySize)
    {
        AsymmetricKeyAlgorithmProvider algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(asymmetricAlgorithm.ToString());
        return algorithm.CreateKeyPair((uint)keySize);
    }

    /// <summary>
    /// Generates an asymmetric key pair using RSA with a key size of 512 bits.
    /// </summary>
    /// <returns>Returns a CryptographicKey object representing the generated key pair.</returns>
    public static CryptographicKey GenerateAsymmetricKeyPair()
    {
        return GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm.RSA_PKCS1, RSAKeySize.RSA512);
    }

    /// <summary>
    /// Generates a cryptographic key pair using a specified asymmetric algorithm.
    /// </summary>
    /// <param name="asymmetricAlgorithm">Specifies the algorithm used for generating the key pair.</param>
    /// <returns>Returns a cryptographic key representing the generated key pair.</returns>
    public static CryptographicKey GenerateAsymmetricKeyPair(AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return GenerateAsymmetricKeyPairBase(asymmetricAlgorithm, RSAKeySize.RSA512);
    }

    /// <summary>
    /// Generates a cryptographic key pair using specified key size.
    /// </summary>
    /// <param name="keySize">Defines the size of the RSA key to be generated.</param>
    /// <returns>Returns a cryptographic key representing the generated key pair.</returns>
    public static CryptographicKey GenerateAsymmetricKeyPair(RSAKeySize keySize)
    {
        return GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm.RSA_PKCS1, keySize);
    }

    /// <summary>
    /// Generates a cryptographic key pair using an asymmetric algorithm and specified key size.
    /// </summary>
    /// <param name="asymmetricAlgorithm">Specifies the algorithm used for generating the asymmetric key pair.</param>
    /// <param name="keySize">Defines the size of the RSA key to be generated.</param>
    /// <returns>Returns a cryptographic key representing the generated key pair.</returns>
    public static CryptographicKey GenerateAsymmetricKeyPair(AsymmetricAlgorithm asymmetricAlgorithm, RSAKeySize keySize)
    {
        return GenerateAsymmetricKeyPairBase(asymmetricAlgorithm, keySize);
    }
}
