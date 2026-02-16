using VulnSync.Utility;

namespace VulnSync.Models;

public class BugDetailsModel
{
    public BugEnum BugType { get; set; }
    public string PlayGroundLink { get; set; }
    public string BugDescriptionPath { get; set; }
    public string SnippetContentPath { get; set; }
}