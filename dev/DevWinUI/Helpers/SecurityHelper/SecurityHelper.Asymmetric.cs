using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
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

    public static string EncryptStringAsymmetric(string plainText, string publicKey)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair)
    {
        keyPair = GenerateAsymmetricKeyPair(AsymmetricAlgorithm.RSA_PKCS1);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    public static string EncryptStringAsymmetric(string plainText, string publicKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, asymmetricAlgorithm, EncodeType.Hex);
    }
    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        keyPair = GenerateAsymmetricKeyPair(asymmetricAlgorithm);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), asymmetricAlgorithm, EncodeType.Hex);
    }
    public static string EncryptStringAsymmetric(string plainText, string publicKey, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, EncodeType encodeType)
    {
        keyPair = GenerateAsymmetricKeyPair(AsymmetricAlgorithm.RSA_PKCS1);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    public static string EncryptStringAsymmetric(string plainText, string publicKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, publicKey, null, asymmetricAlgorithm, encodeType);
    }

    public static string EncryptStringAsymmetric(string plainText, out CryptographicKey keyPair, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        keyPair = GenerateAsymmetricKeyPair(asymmetricAlgorithm);
        return EncryptStringAsymmetricBase(plainText, null, keyPair.ExportPublicKey(), asymmetricAlgorithm, encodeType);
    }

    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, asymmetricAlgorithm, EncodeType.Hex);
    }

    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    public static string EncryptStringAsymmetric(string plainText, IBuffer publicKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return EncryptStringAsymmetricBase(plainText, null, publicKey, asymmetricAlgorithm, encodeType);
    }

    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, asymmetricAlgorithm, EncodeType.Hex);
    }

    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    public static string DecryptStringAsymmetric(string encryptedString, IBuffer privateKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, null, privateKey, asymmetricAlgorithm, encodeType);
    }

    public static string DecryptStringAsymmetric(string encryptedString, string privateKey)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, AsymmetricAlgorithm.RSA_PKCS1, EncodeType.Hex);
    }

    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, asymmetricAlgorithm, EncodeType.Hex);
    }

    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, AsymmetricAlgorithm.RSA_PKCS1, encodeType);
    }

    public static string DecryptStringAsymmetric(string encryptedString, string privateKey, AsymmetricAlgorithm asymmetricAlgorithm, EncodeType encodeType)
    {
        return DecryptStringAsymmetricBase(encryptedString, privateKey, null, asymmetricAlgorithm, encodeType);
    }

    private static CryptographicKey GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm asymmetricAlgorithm, RSAKeySize keySize)
    {
        AsymmetricKeyAlgorithmProvider algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(asymmetricAlgorithm.ToString());
        return algorithm.CreateKeyPair((uint)keySize);
    }

    public static CryptographicKey GenerateAsymmetricKeyPair()
    {
        return GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm.RSA_PKCS1, RSAKeySize.RSA512);
    }

    public static CryptographicKey GenerateAsymmetricKeyPair(AsymmetricAlgorithm asymmetricAlgorithm)
    {
        return GenerateAsymmetricKeyPairBase(asymmetricAlgorithm, RSAKeySize.RSA512);
    }

    public static CryptographicKey GenerateAsymmetricKeyPair(RSAKeySize keySize)
    {
        return GenerateAsymmetricKeyPairBase(AsymmetricAlgorithm.RSA_PKCS1, keySize);
    }

    public static CryptographicKey GenerateAsymmetricKeyPair(AsymmetricAlgorithm asymmetricAlgorithm, RSAKeySize keySize)
    {
        return GenerateAsymmetricKeyPairBase(asymmetricAlgorithm, keySize);
    }
}
