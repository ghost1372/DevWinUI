using System.Globalization;

namespace DevWinUI;
public partial class FileHelper
{
    private static readonly string[] ReservedFileNames =
    [
        "con", "prn", "aux", "nul",
        "com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
        "lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9",
    ];
    private static bool IsAllDots(string fileName)
    {
        foreach (var c in fileName)
        {
            if (c != '.')
                return false;
        }

        return true;
    }

    /// <summary>
    /// Converts a text into a valid file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="reservedNameFormat">The reserved format to use for reserved names. If null '_{0}_' will be used.</param>
    /// <param name="reservedCharFormat">The reserved format to use for reserved characters. If null '_x{0}_' will be used.</param>
    /// <returns>
    /// A valid file name.
    /// </returns>
    public static string GetValidFileName(string fileName, string reservedNameFormat = "_{0}_", string reservedCharFormat = "_x{0}_")
    {
        if (fileName is null)
            throw new ArgumentNullException(nameof(fileName));

        if (reservedNameFormat is null)
            throw new ArgumentNullException(nameof(reservedNameFormat));

        if (reservedCharFormat is null)
            throw new ArgumentNullException(nameof(reservedCharFormat));
        if (ReservedFileNames.Contains(fileName, StringComparer.OrdinalIgnoreCase) || IsAllDots(fileName))
        {
            return string.Format(CultureInfo.InvariantCulture, reservedNameFormat, fileName);
        }

        var invalid = Path.GetInvalidFileNameChars();

        var sb = new StringBuilder(fileName.Length);
        foreach (var c in fileName)
        {
            if (Array.IndexOf(invalid, c) >= 0)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, reservedCharFormat, (short)c);
            }
            else
            {
                sb.Append(c);
            }
        }

        var s = sb.ToString();
        if (string.Equals(s, fileName, StringComparison.Ordinal))
        {
            s = fileName;
        }

        return s;
    }

    /// <summary>
    /// Converts a file size in bytes to a human-readable format with appropriate units.
    /// </summary>
    /// <param name="size">The input value represents the size in bytes to be converted.</param>
    /// <returns>A formatted string that displays the size along with the appropriate unit.</returns>
    public static string GetFileSize(long size)
    {
        string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        const string formatTemplate = "{0}{1:0.#} {2}";

        if (size == 0)
        {
            return string.Format(formatTemplate, null, 0, sizeSuffixes[0]);
        }

        var absSize = Math.Abs((double)size);
        var fpPower = Math.Log(absSize, 1000);
        var intPower = (int)fpPower;
        var iUnit = intPower >= sizeSuffixes.Length
            ? sizeSuffixes.Length - 1
            : intPower;
        var normSize = absSize / Math.Pow(1000, iUnit);

        return string.Format(
            formatTemplate,
            size < 0 ? "-" : null, normSize, sizeSuffixes[iUnit]);
    }

    /// <summary>
    /// Retrieves a StorageFile based on the specified file path and path type, supporting both relative and absolute
    /// paths.
    /// </summary>
    /// <param name="filePath">Specifies the location of the file to be retrieved.</param>
    /// <param name="pathType">Indicates whether the file path is relative to the application or an absolute path.</param>
    /// <returns>Returns the requested storage file or null if not found.</returns>
    public static async Task<StorageFile> GetStorageFile(string filePath, PathType pathType = PathType.Relative)
    {
        StorageFile file = null;
        if (PackageHelper.IsPackaged)
        {
            switch (pathType)
            {
                case PathType.Relative:
                    var sourceUri = new Uri("ms-appx:///" + filePath);
                    file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
                    break;
                case PathType.Absolute:
                    file = await StorageFile.GetFileFromPathAsync(filePath);
                    break;
            }
        }
        else
        {
            switch (pathType)
            {
                case PathType.Relative:
                    var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, filePath));
                    file = await StorageFile.GetFileFromPathAsync(sourcePath);
                    break;
                case PathType.Absolute:
                    file = await StorageFile.GetFileFromPathAsync(filePath);
                    break;
            }
        }

        return file;
    }
}
