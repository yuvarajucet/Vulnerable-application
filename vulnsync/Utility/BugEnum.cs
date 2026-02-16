namespace VulnSync.Utility;

public enum BugEnum
{
    // Add your bugs here
    Default = 0,
    CSRF,
    User_Enumeration,
    Information_Disclosure,
    clickjacking,
    External_Account_Takeover,
    IDOR,
    Response_Manipulation,
    Open_Redirect,
    Role_Manipulation,
    Unauthenticated_API,
    Path_Traversal,
    Account_Takeover,
    Two_Factor_Authentication_Bypass,
    Cookie_Manipulation,
    Race_Condition,
    XSS,
    CSV_Injection,
    SSTI,
    Sql_Injection,
    Insecure_Cryptography,
    Host_Header_Injection,
    SSRF,
    CORS_Policy,
    Improper_error_handling,
}

public enum StatusEnum
{
    Vulnerable = 1,
    Secured
}