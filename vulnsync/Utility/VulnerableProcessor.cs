using VulnSync.Interface;
using VulnSync.Models;

namespace VulnSync.Utility;

public class VulnerableProcessor
{
    private readonly Dictionary<BugEnum, IVulnerableHandler> _vulnerableHandlers;

    public VulnerableProcessor()
    {
        _vulnerableHandlers = new Dictionary<BugEnum, IVulnerableHandler>
        {
            // Add your bugs with their respective handlers here
            
            { BugEnum.CSRF, new CSRFVulnerableHandler() },
            //{ BugEnum.Improper_error_handling, new ImproperErrorHandler() },
            { BugEnum.User_Enumeration, new UserEnumerationHandler() },
            { BugEnum.Information_Disclosure, new InformationDisclosurehandler() },
            { BugEnum.clickjacking, new ClickJackingVulnerableHandler() },
            { BugEnum.External_Account_Takeover, new ExternalAccountTakeoverHandler() },
            { BugEnum.IDOR, new IDORVulnerableHandler() },
            { BugEnum.Response_Manipulation, new ResponseManipulationHandler() },
            { BugEnum.Open_Redirect, new OpenRedirectHandler() },
            { BugEnum.Role_Manipulation, new RoleManipulationHandler() },
            { BugEnum.Unauthenticated_API, new UnauthApiVulnerableHandler() },
            { BugEnum.Path_Traversal, new PathTraversalVulnerableHandler() },
            { BugEnum.Account_Takeover, new AccountTakeoverVulnerableHandler() },
            { BugEnum.Two_Factor_Authentication_Bypass, new TwoFactorAuthenticationBypassHandler() },
            { BugEnum.Cookie_Manipulation, new CookieManipulationVulnerableHandler() },
            { BugEnum.Race_Condition, new RaceConditionVulnerableHandler() },
            { BugEnum.XSS, new XSSVulnerableHandler() },
            { BugEnum.CSV_Injection, new CSVInjectionVulnerableHandler() },
            { BugEnum.SSTI, new SSTIBypassHandler() },
            { BugEnum.Sql_Injection , new SqlInjectionHandler() },
            { BugEnum.Insecure_Cryptography, new InsecureCryptoHandler() },
            { BugEnum.Host_Header_Injection, new HHInjectionHandler() },
            { BugEnum.SSRF , new SSRFHandler() },
            { BugEnum.CORS_Policy, new CORSHandler() },
            { BugEnum.Improper_error_handling, new IprErrHandler() }
        };
    }

    public BugDetailsModel ProcessVulnerability(BugEnum bugType)
    {
        if (_vulnerableHandlers.TryGetValue(bugType, out var handler))
        {
            return handler.GetBugDetails();
        }

        return new BugDetailsModel();
    }
}