using UnityEngine;
using System.Text.RegularExpressions;

public class GoogleDriveHelper
{
    public static string ConvertToDirectDownloadLink(string rawLink)
    {
        Match match = Regex.Match(rawLink, @"(?:drive\.google\.com\/.*?\/d\/|id=)([a-zA-Z0-9_-]+)");

        /*
            Explenation of the regex pattern:
                - (?: ... )             : Non-capturing group to match URL patterns before the file ID.
                    - drive\.google\.com\/.*?\/d\/      : Matches URLs that include 'drive.google.com/.../d/' before the file ID.
                    - id=                               : Matches URLs that include 'id=' before the file ID.
                    - The '|' between them allows for either pattern to match.
                - ([a-zA-Z0-9_-]+)      : Capturing group that matches the Google Drive file ID, which can contain:
                    - Letters (a-z, A-Z)
                    - Numbers (0-9)
                    - Underscores (_) and hyphens (-)
        */

        string result = string.Empty;

        if (match.Success)
        {
            string id = match.Groups[1].Value;
            result = "https://drive.google.com/uc?export=download&id=" + id;
            //Debug.Log(result);
        }
        else
        {
            Debug.LogError("Invalid Google Drive Link");
        }

        return result;
    }
}
