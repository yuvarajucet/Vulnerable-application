using VulnSync.Interface;
using VulnSync.Models;

namespace VulnSync.Utility;

public class CSRFVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.CSRF,
            BugDescriptionPath = "/Views/Csrf/WhatIs.md",
            SnippetContentPath = "/Views/Csrf/{status}/Snippet/CSRF.md",
            PlayGroundLink = "/{status}/csrf"
        };
    }
}

public class UserEnumerationHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.User_Enumeration,
            BugDescriptionPath = "/Views/UserEnumeration/WhatIs.md",
            SnippetContentPath = "/Views/UserEnumeration/{status}/Snippet/UserEnumeration.md",
            PlayGroundLink = "/{status}/userenumeration"
        };
    }
}

public class InformationDisclosurehandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Information_Disclosure,
            BugDescriptionPath = "/Views/InfoDisclosure/WhatIs.md",
            SnippetContentPath = "/Views/InfoDisclosure/{status}/Snippet/InfoDisclosure.md",
            PlayGroundLink = "/{status}/infodisclosure"
        };
    }
}

public class ClickJackingVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.clickjacking,
            BugDescriptionPath = "/Views/ClickJacking/WhatIs.md",
            SnippetContentPath = "/Views/ClickJacking/{status}/Snippet/ClickJacking.md",
            PlayGroundLink = "/{status}/clickJacking"
        };
    }
}

public class ExternalAccountTakeoverHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.External_Account_Takeover,
            BugDescriptionPath = "/Views/ExternalAto/WhatIs.md",
            SnippetContentPath = "/Views/ExternalAto/{status}/Snippet/ExternalAto.md",
            PlayGroundLink = "/{status}/externalato"
        };
    }
}

public class IDORVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.IDOR,
            BugDescriptionPath = "/Views/IDOR/WhatIs.md",
            SnippetContentPath = "/Views/IDOR/{status}/Snippet/IDOR.md",
            PlayGroundLink = "/{status}/idor"
        };
    }
}

public class ResponseManipulationHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            // BugType = BugEnum.Improper_error_handling,
            BugDescriptionPath = "/Views/ResponseManipulation/WhatIs.md",
            SnippetContentPath = "/Views/ResponseManipulation/{status}/Snippet/ResponseManipulation.md",
            PlayGroundLink = "/{status}/responsemanipulation"
        };
    }
}

public class OpenRedirectHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Open_Redirect,
            BugDescriptionPath = "/Views/OpenRedirect/WhatIs.md",
            SnippetContentPath = "/Views/OpenRedirect/{status}/Snippet/OpenRedirect.md",
            PlayGroundLink = "/{status}/openredirect"
        };
    }
}

public class RoleManipulationHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Role_Manipulation,
            BugDescriptionPath = "/Views/RoleManipulation/WhatIs.md",
            SnippetContentPath = "/Views/RoleManipulation/{status}/Snippet/RoleManipulation.md",
            PlayGroundLink = "/{status}/Rolemanipulation"
        };
    }
}

public class UnauthApiVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Unauthenticated_API,
            BugDescriptionPath = "/Views/UnauthApi/WhatIs.md",
            SnippetContentPath = "/Views/UnauthApi/{status}/Snippet/UnauthApi.md",
            PlayGroundLink = "/{status}/unauthapi"
        };
    }
}

public class PathTraversalVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Path_Traversal,
            BugDescriptionPath = "/Views/PathTraversal/WhatIs.md",
            SnippetContentPath = "/Views/PathTraversal/{status}/Snippet/PathTraversal.md",
            PlayGroundLink = "/{status}/pathtraversal"
        };
    }
}

public class AccountTakeoverVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Account_Takeover,
            BugDescriptionPath = "/Views/AccountTakeover/WhatIs.md",
            SnippetContentPath = "/Views/AccountTakeover/{status}/Snippet/AccountTakeover.md",
            PlayGroundLink = "/{status}/accounttakeover"
        };
    }
}

public class RaceConditionVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Race_Condition,
            BugDescriptionPath = "/Views/RaceCondition/WhatIs.md",
            SnippetContentPath = "/Views/RaceCondition/{status}/Snippet/RaceCondition.md",
            PlayGroundLink = "/{status}/racecondition"
        };
    }
}

public class CookieManipulationVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {

            BugType = BugEnum.Cookie_Manipulation,
            BugDescriptionPath = "/Views/CookieManipulation/WhatIs.md",
            SnippetContentPath = "/Views/CookieManipulation/{status}/Snippet/CookieManipulation.md",
            PlayGroundLink = "/{status}/cookiemanipulation"
        };
    }
}

public class TwoFactorAuthenticationBypassHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Two_Factor_Authentication_Bypass,
            BugDescriptionPath = "/Views/TwoFaBusinesslogic/WhatIs.md",
            SnippetContentPath = "/Views/TwoFaBusinesslogic/{status}/Snippet/TwoFaBusinesslogic.md",
            PlayGroundLink = "/{status}/twofabusinesslogic"
        };
    }
}

public class XSSVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.XSS,
            BugDescriptionPath = "/Views/Xss/WhatIs.md",
            SnippetContentPath = "/Views/Xss/{status}/Snippet/XSS.md",
            PlayGroundLink = "/{status}/xss"
        };
    }
}

public class CSVInjectionVulnerableHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.CSV_Injection,
            BugDescriptionPath = "/Views/CSVInjection/WhatIs.md",
            SnippetContentPath = "/Views/CSVInjection/{status}/Snippet/CSVInjection.md",
            PlayGroundLink = "/{status}/csvinjection"
        };
    }
}


public class SSTIBypassHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.SSTI,
            BugDescriptionPath = "/Views/SSTI/WhatIs.md",
            SnippetContentPath = "/Views/SSTI/{status}/Snippet/SSTI.md",
            PlayGroundLink = "/{status}/ssti"
        };
    }
}

public class SqlInjectionHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            // BugType = BugEnum.Improper_error_handling,
            BugDescriptionPath = "/Views/SqlInjection/WhatIs.md",
            SnippetContentPath = "/Views/SqlInjection/{status}/Snippet/SqlInjection.md",
            PlayGroundLink = "/{status}/sqlinjection"
        };
    }
}

public class InsecureCryptoHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Insecure_Cryptography,
            BugDescriptionPath = "/Views/InsecureCrypto/WhatIs.md",
            SnippetContentPath = "/Views/InsecureCrypto/{status}/Snippet/InsecureCrypto.md",
            PlayGroundLink = "/{status}/InsecureCrypto"
        };
    }
}
public class HHInjectionHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Host_Header_Injection,
            BugDescriptionPath = "/Views/HHInjection/WhatIs.md",
            SnippetContentPath = "/Views/HHInjection/{status}/Snippet/HHInjection.md",
            PlayGroundLink = "/{status}/hhinjection"
        };
    }
}
public class SSRFHandler: IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            // BugType = BugEnum.Improper_error_handling,
            BugDescriptionPath = "/Views/SSRF/WhatIs.md",
            SnippetContentPath = "/Views/SSRF/{status}/Snippet/SSRF.md",
            PlayGroundLink = "/{status}/SSRF"
        };
    }
}



public class CORSHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.CORS_Policy,
            BugDescriptionPath = "/Views/CORS/WhatIs.md",
            SnippetContentPath = "/Views/CORS/{status}/Snippet/CORS.md",
            PlayGroundLink = "/{status}/cors"
        };
    }
}


public class IprErrHandler : IVulnerableHandler
{
    public BugDetailsModel GetBugDetails()
    {
        return new BugDetailsModel()
        {
            BugType = BugEnum.Improper_error_handling,
            BugDescriptionPath = "/Views/IprErr/WhatIs.md",
            SnippetContentPath = "/Views/IprErr/{status}/Snippet/IprErr.md",
            PlayGroundLink = "/{status}/IprErr"
        };
    }
}

