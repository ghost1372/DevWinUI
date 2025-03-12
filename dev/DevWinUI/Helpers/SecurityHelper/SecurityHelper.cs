using System.Security.Cryptography;

namespace DevWinUI;

public static partial class SecurityHelper
{
    /// <summary>
    /// Generates a hash of the provided string using a specified hashing algorithm and encoding type.
    /// </summary>
    /// <param name="value">The input string that will be converted into a hash.</param>
    /// <param name="hashAlgorithm">Specifies the algorithm used to compute the hash of the input string.</param>
    /// <param name="encodeType">Determines the format of the output hash, either hexadecimal or base64.</param>
    /// <returns>The resulting hash as a string in the specified encoding format.</returns>
    public static string GetHash(string value, HashAlgorithm hashAlgorithm, EncodeType encodeType = EncodeType.Hex)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        byte[] result = null;
        switch (hashAlgorithm)
        {
            case HashAlgorithm.MD5:
                result = MD5.HashData(bytes);
                break;
            case HashAlgorithm.SHA1:
                result = SHA1.HashData(bytes);
                break;
            case HashAlgorithm.SHA256:
                result = SHA256.HashData(bytes);
                break;
            case HashAlgorithm.SHA384:
                result = SHA384.HashData(bytes);
                break;
            case HashAlgorithm.SHA512:
                result = SHA512.HashData(bytes);
                break;
        }
        return encodeType == EncodeType.Hex
            ? Convert.ToHexString(result).ToUpper()
            : Convert.ToBase64String(result).ToUpper();
    }

    /// <summary>
    /// Calculates the hash of a file asynchronously using a specified hashing algorithm and encoding type.
    /// </summary>
    /// <param name="filePath">Specifies the location of the file to be hashed.</param>
    /// <param name="hashAlgorithm">Determines the algorithm used for generating the hash.</param>
    /// <param name="encodeType">Indicates the format for encoding the resulting hash value.</param>
    /// <returns>Returns the computed hash as a string in the specified encoding format.</returns>
    public static async Task<string> GetHashFromFileAsync(string filePath, HashAlgorithm hashAlgorithm, EncodeType encodeType = EncodeType.Hex)
    {
        var file = await FileHelper.GetStorageFile(filePath, PathType.Absolute);
        var stream = await file.OpenStreamForReadAsync();

        byte[] result = null;
        switch (hashAlgorithm)
        {
            case HashAlgorithm.MD5:
                result = await MD5.HashDataAsync(stream);
                break;
            case HashAlgorithm.SHA1:
                result = await SHA1.HashDataAsync(stream);
                break;
            case HashAlgorithm.SHA256:
                result = await SHA256.HashDataAsync(stream);
                break;
            case HashAlgorithm.SHA384:
                result = await SHA384.HashDataAsync(stream);
                break;
            case HashAlgorithm.SHA512:
                result = await SHA512.HashDataAsync(stream);
                break;
        }
        return encodeType == EncodeType.Hex
            ? Convert.ToHexString(result).ToUpper()
            : Convert.ToBase64String(result).ToUpper();
    }

    /// <summary>
    /// Converts a given string into its Base64 encoded representation.
    /// </summary>
    /// <param name="input">The string to be encoded in Base64 format.</param>
    /// <returns>The Base64 encoded string derived from the input.</returns>
    public static string EncryptBase64(string input)
    {
        var btArray = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(btArray, 0, btArray.Length);
    }

    /// <summary>
    /// Decrypts a Base64 encoded string into its original UTF-8 representation.
    /// </summary>
    /// <param name="encryptedString">A Base64 encoded string that needs to be decoded into its original format.</param>
    /// <returns>The original string representation of the decoded Base64 input.</returns>
    public static string DecryptBase64(string encryptedString)
    {
        var btArray = Convert.FromBase64String(encryptedString);
        return Encoding.UTF8.GetString(btArray);
    }
}
