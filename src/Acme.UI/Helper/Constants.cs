namespace Acme.UI.Helper
{
    public static class Constants
    {
        public const string RegexAthleteUrl = "(?<home>(?i)dashboard)/(?<section>(?i)athlete)/(?<athleteName>[a-zA-Z0-9-]+)";
        // In RegexAthleteUrl  ==> ?<xyz> is the group name and is used to access the group content in the matches e.g. (matches[0] as Match)?.Groups["home"]
        //                     ==> (?i) is making sure the content match for following subgroup is case insensitive e.g. "(?i)dashboard" would make sure that 
        //                              "dashboard" or "DASHBOARD" or anyother character case combination would match the expression as long as it is 'dashboard'
    }
}