using System.Collections.Generic;

namespace DevWinUI_Template.Options
{
    public class ColorsDicOption
    {
        public void ConfigColorsDic(Dictionary<string, string> replacementsDictionary, bool useHomeLandingPage)
        {
            if (useHomeLandingPage)
            {
                replacementsDictionary.Add("$HeaderCoverLight$", """<x:String x:Key="HeaderCover">/Assets/Cover/CoverLight.png</x:String>""");
                replacementsDictionary.Add("$HeaderCoverDark$", """<x:String x:Key="HeaderCover">/Assets/Cover/CoverDark.png</x:String>""");
            }
            else
            {
                replacementsDictionary.Add("$HeaderCoverLight$", "");
                replacementsDictionary.Add("$HeaderCoverDark$", "");
            }
        }
    }
}
