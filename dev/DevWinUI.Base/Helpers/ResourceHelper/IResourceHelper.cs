namespace DevWinUI;
public interface IResourceHelper
{
    List<string> GetAllResourcesKeys(string identifier = null);
    string GetString(string key);
    string GetString(string key, string language);
    string GetStringFromResource(string key, string filename);
    string GetStringFromResource(string key, string language, string filename);
}
