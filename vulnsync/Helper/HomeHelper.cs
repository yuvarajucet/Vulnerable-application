using System.Globalization;
using VulnSync.Models;
using VulnSync.Utility;

namespace VulnSync.Helper;

public class HomeHelper
{
    private VulnerableProcessor _vulnerableProcessor;

    public HomeHelper()
    {
        _vulnerableProcessor = new VulnerableProcessor();
    }
    
    public BugDetailResponseModel GetBugDetails(string bugTypeValue, string status)
    {
        BugEnum bugType = (BugEnum)Enum.Parse(typeof(BugEnum), bugTypeValue);
        if (bugType == BugEnum.Default)
        {
            return new BugDetailResponseModel()
            {
                PlayAreaLink = "/",
                SnippetContent = new MarkDownParser("**There is no content**").Parse(),
                BugDescription = new MarkDownParser("**There is no content**").Parse(),
            };
        }
        StatusEnum statusEnum = (StatusEnum)Enum.Parse(typeof(StatusEnum), status);
        BugDetailsModel bugDetails =  _vulnerableProcessor.ProcessVulnerability(bugType);

        BugDetailResponseModel response = new BugDetailResponseModel();

        response.PlayAreaLink = bugDetails.PlayGroundLink;
        string snippetContentFullPath = bugDetails.SnippetContentPath.Replace("{status}", statusEnum.ToString().ToLower());
        string playGroundFullPath = bugDetails.PlayGroundLink.Replace("{status}", statusEnum.ToString().ToLower());
        response.SnippetContent = new MarkDownParser(this.ReadSnippetContent(snippetContentFullPath)).Parse();
        response.PlayAreaLink = playGroundFullPath;
        response.BugDescription = new MarkDownParser(this.ReadSnippetContent(bugDetails.BugDescriptionPath)).Parse();
        return response;
    }
    
    public object GetBugList()
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return Enum.GetValues(typeof(BugEnum))
            .Cast<BugEnum>()
            .Where(x => (int)x != 0)
            .Select(x => new
            {
                Value = (int)x,
                Name = x.ToString().Contains("_") ? textInfo.ToTitleCase(x.ToString().ToLower().Replace("_", " ")) : x.ToString()
            });
    }
    
    private string ReadSnippetContent(string snippetContentPath)
    {
        string currentPath = Directory.GetCurrentDirectory();
        string fullPath = currentPath + snippetContentPath;
        if (Path.Exists(fullPath))
        {
            string mdFileContent =  File.ReadAllText(fullPath);
            return new MarkDownParser(mdFileContent).Parse();
        }

        return new MarkDownParser("**There is no content**").Parse();
    }
}