using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DevWinUI_Template;
public static class Helper
{
    public static bool IsPreviewVSIX()
    {
        var asm = Assembly.GetExecutingAssembly();
        var asmDir = Path.GetDirectoryName(asm.Location);
        var manifestPath = Path.Combine(asmDir, "extension.vsixmanifest");
        bool isPreview = false;
        if (File.Exists(manifestPath))
        {
            var doc = new XmlDocument();
            doc.Load(manifestPath);
            var metaData = doc.DocumentElement.ChildNodes.Cast<XmlElement>().FirstOrDefault(x => x.Name == "Metadata");
            var identity = metaData.ChildNodes.Cast<XmlElement>().FirstOrDefault(x => x.Name == "Preview");
            var value = identity?.InnerText;
            if (!string.IsNullOrEmpty(value))
            {
                isPreview = value.ToLower().Equals("true");
            }
        }
        return isPreview;
    }
}
