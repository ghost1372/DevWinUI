namespace DevWinUI;
public partial class InIHelper
{
    internal string FilePath { get; }

    public InIHelper(string filePath)
    {
        if (System.IO.Path.IsPathRooted(filePath))
            FilePath = filePath;
        else
            FilePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, filePath));
    }

    private string GetProductName() => ProcessInfoHelper.ProductName;

    /// <summary>
    /// Read Data Value From the Ini File
    /// </summary>
    /// <param name="key">must be unique</param>
    /// <returns></returns>
    public string ReadValue(string key)
    {
        return ReadValue(key, null);
    }

    /// <summary>
    /// Read Data Value From the Ini File
    /// </summary>
    /// <param name="key">must be unique</param>
    /// <param name="section"></param>
    /// <returns></returns>
    public string ReadValue(string key, string section)
    {
        const int MAX_LENGTH = 255;
        Span<char> buffer = stackalloc char[MAX_LENGTH];

        uint result = PInvoke.GetPrivateProfileString(section ?? GetProductName(), key, string.Empty, buffer, FilePath);

        return result > 0 ? buffer.Slice(0, (int)result).ToString() : string.Empty;
    }

    /// <summary>
    /// Write Data to the INI File
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public bool AddValue(string key, string value)
    {
        return AddValue(key, value, null);
    }

    /// <summary>
    /// Write Data to the INI File
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="section"></param>
    public bool AddValue(string key, string value, string section)
    {
        return PInvoke.WritePrivateProfileString(section ?? GetProductName(), key, value, FilePath);
    }

    /// <summary>
    /// Delete Key from INI File
    /// </summary>
    /// <param name="key"></param>
    public bool DeleteKey(string key)
    {
        return DeleteKey(key, null);
    }

    /// <summary>
    /// Delete Key from INI File
    /// </summary>
    /// <param name="key"></param>
    /// <param name="section"></param>
    public bool DeleteKey(string key, string section)
    {
        return AddValue(key, null, section);
    }

    /// <summary>
    /// Delete Section from INI File
    /// </summary>
    /// <param name="section"></param>
    public bool DeleteSection(string section)
    {
        return PInvoke.WritePrivateProfileString(section ?? GetProductName(), null, null, FilePath);
    }

    /// <summary>
    /// Check if Key Exist or Not in INI File
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsKeyExists(string key)
    {
        return IsKeyExists(key, null);
    }

    /// <summary>
    /// Check if Key Exist or Not in INI File
    /// </summary>
    /// <param name="key"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public bool IsKeyExists(string key, string section)
    {
        return !string.IsNullOrEmpty(ReadValue(key, section));
    }
}
