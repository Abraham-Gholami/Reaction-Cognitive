using System;
using UnityEngine.Networking;

// One place for the backend address, and one place for the redirect handling both
// endpoints need.
//
// Routes match the Games Data API spec published at /swagger/v1/swagger.json:
//   POST /Users
//   POST /Data/apps/{appid}/users/{userid}
// The Swagger UI itself is served from https://gamesdata.cognitivetests.ir/, which is
// why BaseUrl is https - but nothing here depends on that any more. See FollowRedirect.
public static class Backend
{
    // Switch scheme or host here; both endpoints follow.
    public const string BaseUrl = "https://gamesdata.cognitivetests.ir";
    public const string AppId = "43921cf3-b5ca-4897-a2b9-4ac919e7af77";

    // How many times a POST may be re-sent to a redirect target before giving up.
    public const int MaxRedirects = 4;

    public static string UsersUrl { get { return BaseUrl + "/Users"; } }

    public static string DataUrl(string userId)
    {
        return BaseUrl + "/Data/apps/" + AppId + "/users/" + userId;
    }

    // UnityWebRequest follows a 3xx by re-issuing the request as a GET with no body.
    // For these two endpoints that is worse than not following at all: the server sees
    // a bodyless GET on a POST-only route and answers 405, so the upload "fails" with
    // nothing having been sent. Both callers set redirectLimit = 0 and use this to
    // re-send the POST, body intact, to wherever the server pointed.
    //
    // This is also what makes the http/https question moot: whichever scheme BaseUrl
    // uses, a redirect to the other one is followed properly.
    public static string FollowRedirect(UnityWebRequest request, string requestedUrl)
    {
        var code = request.responseCode;
        if (code != 301 && code != 302 && code != 303 && code != 307 && code != 308)
            return null;

        var location = request.GetResponseHeader("Location");
        if (string.IsNullOrEmpty(location)) return null;

        // Location may be relative; resolve it against the URL we asked for.
        Uri resolved;
        if (Uri.TryCreate(location, UriKind.Absolute, out resolved))
            return resolved.ToString();

        Uri baseUri;
        if (Uri.TryCreate(requestedUrl, UriKind.Absolute, out baseUri) &&
            Uri.TryCreate(baseUri, location, out resolved))
            return resolved.ToString();

        return null;
    }
}
