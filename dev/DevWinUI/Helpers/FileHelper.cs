namespace DevWinUI;
public partial class FileHelper
{
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
