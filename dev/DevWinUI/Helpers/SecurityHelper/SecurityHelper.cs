using System.Security.Cryptography;

namespace DevWinUI;

public static partial class SecurityHelper
{
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

    public static string EncryptBase64(string input)
    {
        var btArray = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(btArray, 0, btArray.Length);
    }

    public static string DecryptBase64(string encryptedString)
    {
        var btArray = Convert.FromBase64String(encryptedString);
        return Encoding.UTF8.GetString(btArray);
    }
}
