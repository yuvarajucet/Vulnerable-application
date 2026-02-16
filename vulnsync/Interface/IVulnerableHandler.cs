using VulnSync.Models;

namespace VulnSync.Interface;

public interface IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel();
    }
}